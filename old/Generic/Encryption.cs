using System.Text;
using System.Numerics;

namespace Matthew_Tranmer_NEA.Generic
{
    //Encryption using a stream cipher.
    class Encryption
    {
        //Produce multiple random generators and seed them with the key.
        private static Random[] createGenerators(byte[] key, int key_length_bits)
        {
            //Calculate the number of bytes needed to represent the key.
            int key_length_bytes = (int)MathF.Ceiling((float)key_length_bits / 8);

            //Pad the key if necessary.
            if (key.Length < key_length_bytes)
            {
                byte[] padded_key = new byte[key_length_bytes];
                Array.Copy(key, padded_key, key.Length);

                key = padded_key;
            }

            //Each generator produces a 32 bit integer.
            int total_generators = key_length_bits / 32;
            Random[] random_generators = new Random[total_generators];

            //Produce the generators seeded by the key.
            for (int i = 0; i < total_generators; i++)
            {
                byte[] seed_bytes = new byte[4];
                for (int b = 0; b < 4; b++)
                {
                    seed_bytes[b] = key[i * 4 + b];
                }

                int seed = BitConverter.ToInt32(seed_bytes);
                random_generators[i] = new Random(seed);
            }

            return random_generators;
        }

        //Produce the pesudo-random bytes from the seeded generators.
        private static byte[] getRandomBytes(Random[] generators)
        {
            //Combine all random generators using a hash.
            string hash = "ABCDEFG";
            foreach (Random random in generators)
            {
                hash = Hash.combinedHash(hash, random.Next().ToString()); 
            }

            //Convert the hash hex string to a byte array.
            BigInteger hash_int = BigInteger.Parse(hash, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] hash_bytes = hash_int.ToByteArray();

            //Calculate the number of total bytes needed to represent the hex string.
            int bytes_needed = (int)MathF.Ceiling((float)hash.Length * 4) / 8;

            //Pad the array if necessary.
            if (hash_bytes.Length < bytes_needed)
            {
                byte[] padded_hash_bytes = new byte[bytes_needed];
                Array.Copy(hash_bytes, padded_hash_bytes, hash_bytes.Length);
                hash_bytes = padded_hash_bytes;
            }

            return hash_bytes;
            
        }

        //Produce ciphertext with the plaintext and the key.
        public static byte[] encrypt (string plain_text, string key, int key_length_bits=256)
        {
            //Produce the generators seeded with the key.
            Random[] generators = createGenerators(Encoding.UTF8.GetBytes(key), key_length_bits);

            const int pad_length = 32;
            byte[] pad = new byte[0];
            byte[] text_bytes = Encoding.UTF8.GetBytes(plain_text);

            //Perform XORs on ciphertext and one-time-pad produced from the generators.
            for (int i = 0; i < text_bytes.Length; i++)
            {
                int pad_index = i % pad_length;
                if(pad_index == 0)
                {
                    pad = getRandomBytes(generators);
                }

                text_bytes[i] = (byte)(text_bytes[i] ^ pad[pad_index]);
            }

            return text_bytes;
        }

        //Produce plaintext from the ciphertext and the key.
        public static string decrypt(byte[] cipher_text, string key, int key_length_bits=256)
        {
            //Produce the generators seeded with the key.
            Random[] generators = createGenerators(Encoding.UTF8.GetBytes(key), key_length_bits);

            const int pad_length = 32;
            byte[] pad = new byte[0];

            //Perform XORs on ciphertext and one-time-pad produced from the generators.
            for (int i = 0; i < cipher_text.Length; i++)
            {
                //If more bytes are needed.
                int pad_index = i % pad_length;
                if (pad_index == 0)
                {
                    pad = getRandomBytes(generators);
                }

                //Decrypt ciphertext one byte at a time.
                cipher_text[i] = (byte)(cipher_text[i] ^ pad[pad_index]);
            }

            return Encoding.UTF8.GetString(cipher_text);
        }
    }
}
