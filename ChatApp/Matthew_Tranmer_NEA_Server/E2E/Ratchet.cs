using System.Text;
using Matthew_Tranmer_NEA.Generic;

namespace Matthew_Tranmer_NEA.E2E
{
    public class Ratchet
    {
        string chain_key;

        public Ratchet(string root_key)
        {
            chain_key = root_key;
        }

        //Generate the next message key in the chain.
        public string next(string input)
        {
            //Generate hash of the chain key and the input.
            string hash = Hash.combinedHash(chain_key, input);

            //Convert hash to bytes.
            byte[] hash_bytes = Encoding.UTF8.GetBytes(hash);
            byte first_byte = hash_bytes[0];

            //Generate chain key from hash.
            hash_bytes[0] = (byte)(first_byte ^ 0xD1);
            chain_key = Hash.generateHash(Encoding.UTF8.GetString(hash_bytes));

            //Generate message key from hash.
            hash_bytes[0] = (byte)(first_byte ^ 0x53);
            string message_key = Hash.generateHash(Encoding.UTF8.GetString(hash_bytes));

            return message_key;
        }

        public string getCurrentChainKey()
        {
            return chain_key;
        }
    }
}
