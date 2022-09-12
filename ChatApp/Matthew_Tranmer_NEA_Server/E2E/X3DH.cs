using System.Numerics;
using Matthew_Tranmer_NEA.Generic;

namespace Matthew_Tranmer_NEA.E2E
{
    static class X3DH
    {
        //Calculate the shared secret if you have initiated the conversation.
        public static string calculateSecretSender(BigInteger private_sender_ID, EllipticCurvePoint public_reciever_signed_prekey, BigInteger private_ephemeral_key, EllipticCurvePoint public_reciever_ID, EllipticCurvePoint public_one_time_prekey, Span<byte> pre_key_signature)
        {
            if (!EllipticCurveCryptography.verifyDSAsignature(public_reciever_ID, public_reciever_signed_prekey.getCoordinate().x.ToString(), pre_key_signature))
            {
                throw new Exception("Pre keys have been tampered with");
            }

            string component1 = EllipticCurveCryptography.diffieHellman(private_sender_ID, public_reciever_signed_prekey);
            string component2 = EllipticCurveCryptography.diffieHellman(private_ephemeral_key, public_reciever_ID);
            string component3 = EllipticCurveCryptography.diffieHellman(private_ephemeral_key, public_reciever_signed_prekey);
            string component4 = EllipticCurveCryptography.diffieHellman(private_ephemeral_key, public_one_time_prekey);

            //Produce shared key from the components.
            string shared_key = Hash.combinedHash(component1, component2);
            shared_key = Hash.combinedHash(shared_key, component3);
            shared_key = Hash.combinedHash(shared_key, component4);
            return shared_key;
        }

        //Calculate the shared secret if you haven't initiated the conversation.
        public static string calculateSecretReciever(BigInteger private_reciever_signed_prekey, EllipticCurvePoint public_sender_ID, BigInteger private_reciever_ID, EllipticCurvePoint public_ephemeral_key, BigInteger private_one_time_prekey)
        {
            string component1 = EllipticCurveCryptography.diffieHellman(private_reciever_signed_prekey, public_sender_ID);
            string component2 = EllipticCurveCryptography.diffieHellman(private_reciever_ID, public_ephemeral_key);
            string component3 = EllipticCurveCryptography.diffieHellman(private_reciever_signed_prekey, public_ephemeral_key);
            string component4 = EllipticCurveCryptography.diffieHellman(private_one_time_prekey, public_ephemeral_key);

            //Produce shared key from the components.
            string shared_key = Hash.combinedHash(component1, component2);
            shared_key = Hash.combinedHash(shared_key, component3);
            shared_key = Hash.combinedHash(shared_key, component4);
            return shared_key;
        }
    }
}
