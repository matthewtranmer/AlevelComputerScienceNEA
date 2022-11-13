using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using Matthew_Tranmer_NEA.Generic;
using System.Numerics;

namespace Matthew_Tranmer_NEA.Networking
{
    class EncryptedSocketWrapper
    {
        Socket raw_socket;
        string encryption_key;

        public EncryptedSocketWrapper(Socket raw_socket, BigInteger private_key)
        {
            this.raw_socket = raw_socket;
            encryption_key = recvHandshakeSigned(private_key);
        }

        public EncryptedSocketWrapper(Socket raw_socket, EllipticCurvePoint public_key)
        {
            this.raw_socket = raw_socket;
            encryption_key = sendHandshakeSigned(public_key);
        }

        //Send dictionary encoded using JSON.
        private void sendJSON(Dictionary<string, string> data)
        {
            //Serialize the dictionary using JSON.
            string json_payload = JsonSerializer.Serialize(data);
            sendArbitrary(Encoding.UTF8.GetBytes(json_payload));
        }

        //Recieve dictionary encoded using JSON.
        private Dictionary<string, string> recvJSON()
        {
            //Recieve the signature and public key from the other party.
            string recieved_data = Encoding.UTF8.GetString(recvArbitary());
            Dictionary<string, string> deserialized_payload = JsonSerializer.Deserialize<Dictionary<string, string>>(recieved_data)!;

            return deserialized_payload;
        }

        //Generate the secret using a diffie hellman when sending.
        private string generateSecretSend(BigInteger private_key, string other_party_public_key_encoded)
        {
            EllipticCurvePoint other_party_public_key = new EllipticCurvePoint(Convert.FromBase64String(other_party_public_key_encoded), PreDefinedCurves.nist256);
            string shared_secret = EllipticCurveCryptography.diffieHellman(private_key, other_party_public_key);

            return shared_secret;
        }

        //Generate the secret using a diffie hellman when recieving.
        private string generateSecretRecv(BigInteger private_key, string other_party_public_key_encoded)
        {
            //Produce a shared secret encryption key.
            EllipticCurvePoint other_party_public_key = new EllipticCurvePoint(Convert.FromBase64String(other_party_public_key_encoded), PreDefinedCurves.nist256);
            string shared_secret = EllipticCurveCryptography.diffieHellman(private_key, other_party_public_key);

            return shared_secret;
        }

        //Used to generate a shared encryption key for the reciever which includes a digital signature.
        private string recvHandshakeSigned(BigInteger private_signature_key)
        {
            //Recieve the data from the other party.
            Dictionary<string, string> recieved_data = recvJSON();

            //Generate a key pair to generate a shared secret.
            KeyPair key_pair = new KeyPair(PreDefinedCurves.nist256);
            string public_key = Convert.ToBase64String(key_pair.getPublicComponent().compressPoint());

            //Create a signature of the compressed public key.
            string signature = Convert.ToBase64String(EllipticCurveCryptography.generateDSAsignature(new KeyPair(private_signature_key, PreDefinedCurves.nist256), public_key));

            Dictionary<string, string> payload = new Dictionary<string, string>()
            {
                { "public_key", public_key },
                { "signature", signature }
            };

            //Send the public key to the other party.
            sendJSON(payload);

            //Generate the encryption key.
            return generateSecretRecv(key_pair.getPrivateComponent(), recieved_data["public_key"]);
        }

        //Used to generate a shared encryption key for the sender.
        private string sendHandshakeSigned(EllipticCurvePoint public_signature_key)
        {
            //Create a key pair which will be used to generate a shared secret.
            KeyPair key_pair = new KeyPair(PreDefinedCurves.nist256);
            string public_key = Convert.ToBase64String(key_pair.getPublicComponent().compressPoint());

            Dictionary<string, string> payload = new Dictionary<string, string>()
            {
                { "public_key", public_key },
            };

            sendJSON(payload);

            //Recieve the other parties public key to generate a shared secret.
            Dictionary<string, string> deserialized_payload = recvJSON();

            //Verify if the signature is correct.
            bool is_signature_correct = EllipticCurveCryptography.verifyDSAsignature(public_signature_key, deserialized_payload["public_key"], Convert.FromBase64String(deserialized_payload["signature"]));
            if (!is_signature_correct)
            {
                throw new IOException("Invalid Signature");
            }

            //Return shared encryption key.
            return generateSecretSend(key_pair.getPrivateComponent(), deserialized_payload["public_key"]);
        }
        

        //Sends a message with a content length header.
        private void sendArbitrary(byte[] buffer)
        {
            //Encode the length of the buffer as a byte array.
            byte[] content_length = BitConverter.GetBytes(buffer.Length);

            //Buffer with content length header.
            byte[] data = new byte[content_length.Length + buffer.Length];
            Array.Copy(content_length, data, content_length.Length);
            Array.Copy(buffer, 0, data, content_length.Length, buffer.Length);

            //Send the buffer length then the actual data.
            raw_socket.Send(data);
        }

        //Recieves a message with a content length header.
        private byte[] recvArbitary()
        {
            //Recieve the encoded content length.
            byte[] content_length_bytes = new byte[4];
            raw_socket.Receive(content_length_bytes);

            //Decode the content length.
            int content_length = BitConverter.ToInt32(content_length_bytes);
            //Create a buffer of the size of the content length.
            byte[] payload = new byte[content_length];
            //Recieve the data into the buffer.
            raw_socket.Receive(payload);

            return payload;
        }

        //Encrypts the given data and sends it.
        private void sendEncrypted(byte[] buffer, string encryption_key)
        {
            //Encrypt the data with the key.
            byte[] encrypted_data = Encryption.encrypt(Convert.ToBase64String(buffer), encryption_key);

            //Send the encrypted data to the other party.
            sendArbitrary(encrypted_data);
        }

        //Recieves encrypted data and decrypts it.
        private byte[] recieveEncrypted(string encryption_key)
        {
            //Recieve the encrypted data
            byte[] encrypted_data = recvArbitary();

            //Return the decrypted data.
            return Convert.FromBase64String(Encryption.decrypt(encrypted_data, encryption_key));
        }

        //Sends an encrypted message.
        public void send(byte[] buffer)
        {
            sendEncrypted(buffer, encryption_key);
        }

        //Recieves an encrypted message.
        public byte[] recieve()
        {
            return recieveEncrypted(encryption_key);
        }
    }
}
