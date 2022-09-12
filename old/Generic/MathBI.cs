using System.Numerics;

namespace Matthew_Tranmer_NEA.Generic
{
    static class MathBI
    {
        //Returns the remainder after division, including negative values.
        public static BigInteger mod(BigInteger value, BigInteger modulus)
        {
            BigInteger number = value % modulus;
            if (number < 0)
            {
                number += modulus;
            }

            return number;
        }

        //Returns the modular multiplicative inverse. Modulus must be prime.
        public static BigInteger modularMultiplicativeInverse(BigInteger value, BigInteger modulus)
        {
            return BigInteger.ModPow(value, modulus - 2, modulus);
        }

        //Returns the modular square root. Must satisfy modulus mod 4 == 3.
        public static BigInteger modularSquareRoot(BigInteger value, BigInteger modulus)
        {
            BigInteger sqrt_exponent = (modulus + 1) / 4;
            return BigInteger.ModPow(value, sqrt_exponent, modulus);
        }

        public static BigInteger randomInteger(BigInteger max)
        {
            //Get amount of bits needed to represent the number.
            int length = Convert.ToInt32(Math.Ceiling(BigInteger.Log(max, 2)));
            byte[] buffer = new byte[length];

            //Fill the buffer with random values.
            System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);

            //Create a BigInteger from the values.
            BigInteger number = new BigInteger(buffer, true);

            //Make sure the number is less than the max.
            number = number % max;

            //Make sure the number is not zero. 
            if (number < 1)
            {
                number = 1;
            }

            return number;
        }
    }
}
