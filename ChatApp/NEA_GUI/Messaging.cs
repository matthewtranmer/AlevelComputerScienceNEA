using System.Numerics;
using Matthew_Tranmer_NEA.Generic;
using Matthew_Tranmer_NEA.E2E;

namespace NEA_GUI
{
    internal class Messaging
    {
        private static string recieveInitialMessage(string sender, Dictionary<string, string>? response)
        {
            string pre_key_identifier = response!["prekey_identity"];

            PreKey? selected_pre_key = null;
            foreach (PreKey pre_key in ApplicationValues.pre_keys)
            {
                if (pre_key.identifier == pre_key_identifier)
                {
                    selected_pre_key = pre_key;
                    break;
                }
            }

            if (selected_pre_key == null)
            {
                throw new Exception("Pre Key Was Not Found");
            }

            Span<byte> encoded_public_sender_ID = Convert.FromBase64String(response["public_sender_ID"]);
            EllipticCurvePoint public_sender_ID = new EllipticCurvePoint(encoded_public_sender_ID, PreDefinedCurves.nist256);

            Span<byte> encoded_public_ephemeral_key = Convert.FromBase64String(response["public_ephemeral_key"]);
            EllipticCurvePoint public_ephemeral_key = new EllipticCurvePoint(encoded_public_ephemeral_key, PreDefinedCurves.nist256);

            string root_key = X3DH.calculateSecretReciever(
                ApplicationValues.signed_pre_key,
                public_sender_ID,
                ApplicationValues.identity_key,
                public_ephemeral_key,
                selected_pre_key.key.getPrivateComponent()
            );

            byte[] encrypted_message = Convert.FromBase64String(response["encrypted_message"]);

            Span<byte> encoded_public_ratchet_key = Convert.FromBase64String(response["public_ratchet_key"]);
            EllipticCurvePoint public_ratchet_key = new EllipticCurvePoint(encoded_public_ratchet_key, PreDefinedCurves.nist256);

            Ratchet root_chain = new Ratchet(root_key);
            string input = EllipticCurveCryptography.diffieHellman(ApplicationValues.signed_pre_key, public_ratchet_key);
            string message_chain_root = root_chain.next(input);

            Ratchet message_chain = new Ratchet(message_chain_root);
            string encryption_key = message_chain.next("1");

            string plaintext_message = Encryption.decrypt(encrypted_message, encryption_key);

            string user_encrypted_message = Convert.ToBase64String(Encryption.encrypt(plaintext_message, ApplicationValues.encryption_key));
            string encrypted_root_chain = Convert.ToBase64String(Encryption.encrypt(message_chain_root, ApplicationValues.encryption_key));



            Dictionary<string, string> APIrequest = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\upload_old_message" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient", ApplicationValues.username },
                { "sender", sender },
                { "message_type", "initial_message" },
                { "encrypted_message", user_encrypted_message },
                { "private_chain_key", encrypted_root_chain },
                { "time_sent", response["time_sent"] }
            };

            (_, bool fatal_error) = API.apiRequest(APIrequest);
            if (fatal_error) throw new Exception("Fatal Error");

            return plaintext_message;
        }

        private static string recieveFirstInChainMessage(string sender, Dictionary<string, string>? response)
        {
            byte[] encrypted_message = Convert.FromBase64String(response!["encrypted_message"]);

            Span<byte> encoded_public_ratchet_key = Convert.FromBase64String(response["public_ratchet_key"]);
            EllipticCurvePoint public_ratchet_key = new EllipticCurvePoint(encoded_public_ratchet_key, PreDefinedCurves.nist256);

            byte[] encrypted_private_ratchet_key = Convert.FromBase64String(response["private_ratchet_key"]);
            byte[] encoded_private_ratchet_key = Convert.FromBase64String(Encryption.decrypt(encrypted_private_ratchet_key, ApplicationValues.encryption_key));
            BigInteger private_ratchet_key = new BigInteger(encoded_private_ratchet_key);

            byte[] encrypted_chain_key = Convert.FromBase64String(response["private_chain_key"]);
            string decrypted_chain_key = Encryption.decrypt(encrypted_chain_key, ApplicationValues.encryption_key);

            Ratchet root_chain = new Ratchet(decrypted_chain_key);
            string input = EllipticCurveCryptography.diffieHellman(private_ratchet_key, public_ratchet_key);

            string message_chain_root = root_chain.next(input);

            Ratchet message_chain = new Ratchet(message_chain_root);
            string encryption_key = message_chain.next("1");

            string plaintext_message = Encryption.decrypt(encrypted_message, encryption_key);

            string user_encrypted_message = Convert.ToBase64String(Encryption.encrypt(plaintext_message, ApplicationValues.encryption_key));
            string encrypted_root_chain = Convert.ToBase64String(Encryption.encrypt(message_chain_root, ApplicationValues.encryption_key));

            Dictionary<string, string> APIrequest = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\upload_old_message" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient", ApplicationValues.username },
                { "sender", sender },
                { "message_type", "first_in_chain" },
                { "encrypted_message", user_encrypted_message },
                { "private_chain_key", encrypted_root_chain },
                { "time_sent", response["time_sent"] }
            };

            (_, bool fatal_error) = API.apiRequest(APIrequest);
            if (fatal_error) throw new Exception("Fatal Error");

            return plaintext_message;
        }

        private static string recieveChainMessage(string sender, Dictionary<string, string>? response)
        {
            byte[] encrypted_message = Convert.FromBase64String(response!["encrypted_message"]);

            byte[] encrypted_chain_key = Convert.FromBase64String(response["private_chain_key"]);
            string decrypted_chain_key = Encryption.decrypt(encrypted_chain_key, ApplicationValues.encryption_key);

            int sequence_count = Convert.ToInt32(response["sequence_count"]);

            string encryption_key = "";
            Ratchet message_chain = new Ratchet(decrypted_chain_key);
            for (int i = 1; i < sequence_count + 1; i++)
            {
                encryption_key = message_chain.next(Convert.ToString(i));
            }

            string plaintext_message = Encryption.decrypt(encrypted_message, encryption_key);
            string user_encrypted_message = Convert.ToBase64String(Encryption.encrypt(plaintext_message, ApplicationValues.encryption_key));

            Dictionary<string, string> APIrequest = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\upload_old_message" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient", ApplicationValues.username },
                { "sender", sender },
                { "message_type", "chain_message" },
                { "encrypted_message", user_encrypted_message },
                { "time_sent", response["time_sent"] }
            };

            (_, bool fatal_error) = API.apiRequest(APIrequest);
            if (fatal_error) throw new Exception("Fatal Error");

            return plaintext_message;
        }

        public static void messageReady(MainForm form, Dictionary<string, string> request)
        {
            string sender = request["sender"];

            Dictionary<string, string> APIrequest = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\recieve_message" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "sender", sender }
            };

            (Dictionary<string, string>? response, bool fatal_error) = API.apiRequest(APIrequest);
            if (fatal_error) throw new Exception("Fatal Error");

            string message = "";

            switch (response?["message_type"])
            {
                case "initial_message":
                    message = recieveInitialMessage(sender, response);
                    break;

                case "first_in_chain":
                    message = recieveFirstInChainMessage(sender, response);
                    break;

                case "chain_message":
                    message = recieveChainMessage(sender, response);
                    break;
            }

            if (form.current_recipient == sender)
            {
                form.Invoke(() => form.createMessageBox(message, sender));
            }
            else
            {
                form.Invoke(() => form.createNotification($"{sender} has sent you a message."));
            }
        }

        public static (Dictionary<string, string>? response, bool fatal_error) sendInitialMessage(string message, string recipient, string public_signed_pre_key, string public_reciever_ID, string public_one_time_prekey, string prekey_identity, string pre_key_signature)
        {
            KeyPair ephemeral_key = new KeyPair(PreDefinedCurves.nist256);

            EllipticCurvePoint public_signed_pre_key_decoded = new EllipticCurvePoint(Convert.FromBase64String(public_signed_pre_key), PreDefinedCurves.nist256);
            EllipticCurvePoint public_reciever_ID_decoded = new EllipticCurvePoint(Convert.FromBase64String(public_reciever_ID), PreDefinedCurves.nist256);
            EllipticCurvePoint public_one_time_prekey_decoded = new EllipticCurvePoint(Convert.FromBase64String(public_one_time_prekey), PreDefinedCurves.nist256);
            Span<byte> signature_decoded = Convert.FromBase64String(pre_key_signature);

            string root_key = X3DH.calculateSecretSender(
                ApplicationValues.identity_key,
                public_signed_pre_key_decoded,
                ephemeral_key.getPrivateComponent(),
                public_reciever_ID_decoded,
                public_one_time_prekey_decoded,
                signature_decoded
            );

            KeyPair ratchet_key = new KeyPair(PreDefinedCurves.nist256);

            Ratchet root_chain = new Ratchet(root_key);
            string input = EllipticCurveCryptography.diffieHellman(ratchet_key.getPrivateComponent(), public_signed_pre_key_decoded);
            string message_chain_root = root_chain.next(input);

            Ratchet message_chain = new Ratchet(message_chain_root);
            string encryption_key = message_chain.next("1");

            string encrypted_message = Convert.ToBase64String(Encryption.encrypt(message, encryption_key));

            string encoded_ratchet_private_key = Convert.ToBase64String(ratchet_key.getPrivateComponent().ToByteArray());
            string encrypted_ratchet_private_key = Convert.ToBase64String(Encryption.encrypt(encoded_ratchet_private_key, ApplicationValues.encryption_key));

            string encoded_ratchet_public_key = Convert.ToBase64String(ratchet_key.getPublicComponent().compressPoint());
            string encoded_ephemeral_public_key = Convert.ToBase64String(ephemeral_key.getPublicComponent().compressPoint());

            string encrypted_chain_key = Convert.ToBase64String(Encryption.encrypt(message_chain_root, ApplicationValues.encryption_key));

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\send_message" }, //
                { "message_type", "initial_message" }, //
                { "username", ApplicationValues.username }, //
                { "token", ApplicationValues.session_token }, //
                { "recipient_username", recipient }, //
                { "encrypted_message", encrypted_message }, //
                { "encrypted_ratchet_private_key", encrypted_ratchet_private_key }, //
                { "public_ratchet_key", encoded_ratchet_public_key }, //
                { "public_ephemeral_key", encoded_ephemeral_public_key },
                { "encrypted_chain_key", encrypted_chain_key }, //
                { "prekey_identity", prekey_identity }
            };

            return API.apiRequest(request);
        }

        private static (Dictionary<string, string>? response, bool fatal_error) sendFirstMessageInChain(string message, string recipient, string private_chain_key, string public_ratchet_key)
        {
            KeyPair ratchet_key = new KeyPair(PreDefinedCurves.nist256);

            EllipticCurvePoint decoded_public_ratchet_key = new EllipticCurvePoint(Convert.FromBase64String(public_ratchet_key), PreDefinedCurves.nist256);

            byte[] decoded_chain_key = Convert.FromBase64String(private_chain_key);
            string decrypted_chain_key = Encryption.decrypt(decoded_chain_key, ApplicationValues.encryption_key);

            Ratchet root_chain = new Ratchet(decrypted_chain_key);
            string input = EllipticCurveCryptography.diffieHellman(ratchet_key.getPrivateComponent(), decoded_public_ratchet_key);
            string message_chain_root = root_chain.next(input);

            Ratchet message_chain = new Ratchet(message_chain_root);
            string encryption_key = message_chain.next("1");

            string encrypted_message = Convert.ToBase64String(Encryption.encrypt(message, encryption_key));

            string encrypted_chain_key = Convert.ToBase64String(Encryption.encrypt(message_chain_root, ApplicationValues.encryption_key));

            string encoded_ratchet_private_key = Convert.ToBase64String(ratchet_key.getPrivateComponent().ToByteArray());
            string encrypted_ratchet_private_key = Convert.ToBase64String(Encryption.encrypt(encoded_ratchet_private_key, ApplicationValues.encryption_key));

            string encoded_public_ratchet_key = Convert.ToBase64String(ratchet_key.getPublicComponent().compressPoint());

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\send_message" },
                { "message_type", "first_in_chain" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient_username", recipient },
                { "encrypted_message", encrypted_message },
                { "encrypted_chain_key", encrypted_chain_key },
                { "encrypted_ratchet_private_key", encrypted_ratchet_private_key },
                { "public_ratchet_key", encoded_public_ratchet_key }
            };

            return API.apiRequest(request);
        }

        private static (Dictionary<string, string>? response, bool fatal_error) sendMessageInChain(string message, string recipient, string private_chain_key, string string_sequence_count)
        {
            byte[] decoded_chain_key = Convert.FromBase64String(private_chain_key);
            string decrypted_chain_key = Encryption.decrypt(decoded_chain_key, ApplicationValues.encryption_key);

            int sequence_count = Convert.ToInt32(string_sequence_count) + 1;

            string encryption_key = "";
            Ratchet message_chain = new Ratchet(decrypted_chain_key);
            for (int i=1; i<sequence_count+1; i++)
            {
                encryption_key = message_chain.next(Convert.ToString(i));
            }

            string encrypted_message = Convert.ToBase64String(Encryption.encrypt(message, encryption_key));

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\send_message" },
                { "message_type", "chain_message" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient_username", recipient },
                { "encrypted_message", encrypted_message },
                { "sequence_count", Convert.ToString(sequence_count) }
            };

            return API.apiRequest(request);
        }

        public static (Dictionary<string, string>? response, bool fatal_error) sendMessage(string message, string recipient)
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\request_message_send" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient_username", recipient }
            };

            (Dictionary<string, string>? response, bool fatal_error) = API.apiRequest(request);
            if (fatal_error)
            {
                return (response, fatal_error);
            }

            switch (response!["message_send_type"])
            {
                case "initial_message":
                    sendInitialMessage(message, recipient, response["public_signed_pre_key"], response["public_reciever_ID"], response["public_one_time_prekey"], response["prekey_identity"], response["pre_key_signature"]);
                    break;

                case "first_in_chain":
                    sendFirstMessageInChain(message, recipient, response["private_chain_key"], response["public_ratchet_key"]);
                    break;

                case "chain_message":
                    sendMessageInChain(message, recipient, response["private_chain_key"], response["sequence_count"]);
                    break;
            }

            string encrypted_message = Convert.ToBase64String(Encryption.encrypt(message, ApplicationValues.encryption_key));

            request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\upload_old_message" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "sender", ApplicationValues.username },
                { "recipient", recipient },
                { "encrypted_message", encrypted_message }
            };

            (response, fatal_error) = API.apiRequest(request);
            if (fatal_error)
            {
                return (response, fatal_error);
            }

            return (response, fatal_error);
        }
    }
}
