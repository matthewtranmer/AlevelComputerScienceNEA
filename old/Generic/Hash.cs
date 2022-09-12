using System.Text;

namespace Matthew_Tranmer_NEA.Generic
{
    static class Hash
    {
        //Perform a bitwise left circular shift. 
        private static uint leftCircularShift(uint operand, int shift)
        {
            return operand << shift | operand >> 32 - shift;
        }

        //Split data into block specified by the block size in bytes.
        private static byte[,] generateBlocks(string data, int block_size)
        {
            //Separate string into array of bytes.
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            //Calculate the total number of blocks there will be.
            int total_blocks = (int)Math.Ceiling((double)bytes.Length / block_size);

            //Create 2D array for the blocks.
            byte[,] blocks = new byte[total_blocks, block_size];

            //Seaparate string into blocks.
            int block_count = 0;
            for (int block_index = 0; block_index < total_blocks; block_index++)
            {
                //Number of iterations needed to create the block.
                int iterations = block_size;

                //If total block will be less than the block size.
                if (bytes.Length - block_index * block_size < block_size)
                {
                    //Calculate total number of bytes that will go into the block.
                    iterations = bytes.Length - bytes.Length / block_size * block_size;

                    //Convert the number of iterations to bytes for use in padding.
                    byte[] iterations_byte = BitConverter.GetBytes(iterations);

                    //Add padding to empty bytes.
                    int count = 0;
                    for (int x = iterations; x < block_size; x++)
                    {
                        //Set empty bytes to repeating bits of the number of iterations.
                        blocks[block_index, x] = iterations_byte[count % iterations_byte.Length];
                        count++;
                    }
                }

                //Set blocks to bytes of string.
                for (int x = 0; x < iterations; x++)
                {
                    blocks[block_index, x] = bytes[block_count];
                    block_count++;
                }
            }

            return blocks;
        }

        //Derive words from the block array at the given index.
        private static uint[] deriveWords(byte[,] blocks, int index, int word_count)
        {
            uint[] words = new uint[word_count];

            //Turn 512 bit block into 16, 32 bit words.
            for (int i = 0; i < blocks.GetLength(1) / 4; i++)
            {
                byte[] bytes = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    bytes[j] = blocks[index, i * 4 + j];
                }

                //Convert byte to uint.
                words[i] = BitConverter.ToUInt32(bytes, 0);
            }

            //Derive an extra 64 words from block values.
            for (int i = blocks.GetLength(1) / 4; i < word_count; i++)
            {
                words[i] = leftCircularShift(words[i - 3] ^ words[i - 8] ^ words[i - 14] ^ words[i - 16], 1);
            }

            return words;
        }

        public static byte[] generateHashBytes(string data)
        {
            const int word_count = 80;

            //Separate data into 512 bit blocks.
            byte[,] blocks = generateBlocks(data, 64);

            //Set intial internal state values.
            uint[] state_values = new uint[] {
                0_456_123_633U,
                1_000_562_423U,
                4_162_745_346U,
                2_245_357_991U,
                1_148_425_784U,
                2_702_461_119U,
                3_953_332_674U,
                2_894_046_873U
            };

            //Iterate over blocks.
            for (int block_count = 0; block_count < blocks.GetLength(0); block_count++)
            {
                //Create 80 words.
                uint[] words = deriveWords(blocks, block_count, word_count);

                uint[] round_values = new uint[state_values.Length];
                for (int i = 0; i < state_values.Length; i++)
                {
                    round_values[i] = state_values[i];
                }

                for (int i = 0; i < word_count; i++)
                {
                    //Perform mod 2^32 addition, XORs and other permutations to remove data.
                    //                               A                 B                 C                 D                 E                 F                 G                 H                    
                    uint end_addition = round_values[0] + round_values[1] + round_values[2] + round_values[3] + round_values[4] + round_values[5] + round_values[6] + round_values[7] + words[i] + 834_934_501;

                    //           F                 E                 F
                    round_values[5] = round_values[4] ^ round_values[5];
                    //           E                                   D
                    round_values[4] = leftCircularShift(round_values[3], 4);
                    //           G                 F                 G
                    round_values[6] = round_values[4] ^ round_values[6];
                    //           H                 F                 C
                    round_values[7] = round_values[5] ^ round_values[2];
                    //           D                                   B                     C 
                    round_values[3] = leftCircularShift(round_values[1], 9) ^ round_values[2];
                    //           B                 A                 B
                    round_values[1] = round_values[0] ^ round_values[1];
                    //           C                                   B
                    round_values[2] = leftCircularShift(round_values[1], 15);
                    //           A
                    round_values[0] = end_addition;
                }

                //Add round values onto total internal state values.
                for (int i = 0; i < state_values.Length; i++)
                {
                    state_values[i] += round_values[i];
                }
            }

            //Combine internal state to produce hash.
            byte[] hash = new byte[state_values.Length*4];
            for (int i = 0; i < state_values.Length; i++)
            {
                byte[] state_bytes = BitConverter.GetBytes(state_values[i]);
                for (int x = 0; x < state_bytes.Length; x++)
                {
                    hash[i*4 + x] = state_bytes[x];
                }
            }

            return hash;
        }

        public static string generateHash(string data)
        {
            byte[] hash = generateHashBytes(data);
            return Convert.ToHexString(hash);
        }

        //Generate a hash of two values.
        public static string combinedHash(string input1, string input2)
        {
            //Convert input1 to byte array.
            byte[] input_1_bytes = Encoding.UTF8.GetBytes(input1);

            byte first_byte = input_1_bytes[0];

            //Generate hash with inner pad.
            input_1_bytes[0] = (byte)(first_byte ^ 0x61);
            string ipad = Encoding.UTF8.GetString(input_1_bytes);
            string hash = generateHash(ipad + input2);

            //Generate hash with outer pad.
            input_1_bytes[0] = (byte)(first_byte ^ 0x3D);
            string opad = Encoding.UTF8.GetString(input_1_bytes);
            hash = generateHash(hash + opad);

            return hash;
        }

        //Generates a password hash with a salt.
        public static string generatePasswordHash(string password, int rounds)
        {
            //Create random string for the salt.
            const int salt_length = 5;
            byte[] salt_bytes = new byte[salt_length];
            System.Security.Cryptography.RandomNumberGenerator.Fill(salt_bytes);
            string salt = Convert.ToBase64String(salt_bytes);

            //Create hash.
            string hash = password + salt;
            for (int i = 0; i < MathF.Pow(2, rounds); i++)
            {
                hash = generateHash(hash);
            }

            return hash + salt;
        }

        //Generates a password hash with a salt.
        public static bool verifyPasswordHash(string hash, string password, int rounds)
        {
            //Separate the salt from the hash.
            string salt = hash.Substring(64);
            string password_hash = hash.Substring(0, 64);

            //Generate hash.
            string generated_hash = password + salt;
            for (int i = 0; i < MathF.Pow(2, rounds); i++)
            {
                generated_hash = generateHash(generated_hash);
            }

            //Compare generated hash produced from the inputed password with the given hash.
            if (generated_hash != password_hash)
            {
                return false;
            }

            return true;
        }
    }
}
