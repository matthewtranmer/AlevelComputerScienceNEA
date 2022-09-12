using System.Numerics;
using System.Globalization;

namespace Matthew_Tranmer_NEA.Generic
{
    static class EllipticCurveCryptography
    {
        //Produce a shared secret key.
        public static string diffieHellman(BigInteger private_key, EllipticCurvePoint public_key)
        {
            //Make a copy of the point.
            EllipticCurvePoint point_copy = new EllipticCurvePoint(public_key);

            //Multiply the copy of the point by the private key.
            point_copy.multiplyPoint(private_key);
            string shared_key = Hash.generateHash(point_copy.getCoordinate().x.ToString());

            return shared_key;
        }

        //Convert a hash to an int with the bit length less than or equal to the order.
        private static BigInteger hashToInt(string hash, BigInteger order)
        {
            //Convert the hexadecimal hash string to an integer.
            BigInteger int_hash = BigInteger.Parse("0" + hash, NumberStyles.AllowHexSpecifier);
            //Get the amount of bits needed to represent the order.
            int order_bit_length = (int)order.GetBitLength();

            //If we need to truncate the hash integer.
            if (int_hash.GetBitLength() > order_bit_length)
            {
                //Get the number of bytes needed to represent the hash int.
                int hash_byte_count = int_hash.GetByteCount(isUnsigned: true);
                //Calculate how many extra bytes the int hash has over the order.
                int byte_difference = hash_byte_count - order.GetByteCount(isUnsigned: true);

                //Convert the int hash to a byte array and remove the unnecessary full bytes
                Span<byte> bytes = int_hash.ToByteArray(isUnsigned: true, isBigEndian: true);
                bytes = bytes.Slice(byte_difference, bytes.Length - byte_difference);

                //Calculate how many bits are needed to store the largest byte that makes up the order.
                int smallest_bits_needed = (int)Math.Floor(Math.Log(order.ToByteArray(isBigEndian: true, isUnsigned: true)[0], 2)+1);

                //Mask the extra bits to truncate the int hash
                byte mask = byte.MaxValue;
                mask = (byte)(mask >> 8 - smallest_bits_needed);
                bytes[0] = (byte)(bytes[0] & mask);

                //Convert the truncated bytes back into an int
                int_hash = new BigInteger(bytes, isUnsigned:true, isBigEndian: true);
            }

            return int_hash;
        }

        //Sign the given data using the private key. Returns the signature.
        public static Span<byte> generateDSAsignature(KeyPair key_pair, string data)
        {
            DomainParameters domain_parameters = key_pair.getPublicComponent().getDomainParameters();

            //Create a hash of the given data.
            string hash = Hash.generateHash(data);
            //Convert the hash to an int which has a bit length less than or equal to the order.
            BigInteger int_hash = hashToInt(hash, domain_parameters.order);

            BigInteger ephemeral_key = 0;
            BigInteger r = 0;
            BigInteger s = 0;

            while (s == 0)
            {
                while (r == 0)
                {
                    ephemeral_key = MathBI.randomInteger(domain_parameters.order - 1);
                    EllipticCurvePoint ephemeral_point = new EllipticCurvePoint(domain_parameters);
                    ephemeral_point.multiplyPoint(ephemeral_key);

                    r = MathBI.mod(ephemeral_point.getCoordinate().x, domain_parameters.order);
                }

                s = int_hash + (r * key_pair.getPrivateComponent());
                s = s * MathBI.modularMultiplicativeInverse(ephemeral_key, domain_parameters.order);
                s = MathBI.mod(s, domain_parameters.order);
            }

            //Get bytes needed to represent the largest possible value.
            int length = domain_parameters.order.GetByteCount(isUnsigned: true);
            byte[] signature = new byte[length * 2];

            //Convert signature to bytes.
            r.ToByteArray(isBigEndian: false, isUnsigned: true).CopyTo(signature, 0);
            s.ToByteArray(isBigEndian: false, isUnsigned: true).CopyTo(signature, length);

            return new Span<byte>(signature);
        }

        //Verifies the signature aginst the public key.
        public static bool verifyDSAsignature(EllipticCurvePoint public_key, string data, Span<byte> signature)
        {
            //Make sure public key is valid.
            if (!public_key.validatePoint())
            {
                return false;
            }

            DomainParameters domain_parameters = public_key.getDomainParameters();

            //Create a hash of the given data.
            string hash = Hash.generateHash(data);
            //Convert the hash to an int which has a bit length less than or equal to the order.
            BigInteger int_hash = hashToInt(hash, domain_parameters.order);

            //Decode signature.
            int length = domain_parameters.order.GetByteCount(isUnsigned: true);
            BigInteger r = new BigInteger(signature.Slice(0, length), isUnsigned: true);
            BigInteger s = new BigInteger(signature.Slice(length, length), isUnsigned: true);

            //Make sure r is valid.
            if (r >= domain_parameters.order || r < 1)
            {
                return false;
            }

            //Make sure s is valid.
            if (s >= domain_parameters.order || s < 1)
            {
                return false;
            }

            BigInteger inverse_s = MathBI.modularMultiplicativeInverse(s, domain_parameters.order);
            BigInteger u1 = MathBI.mod(int_hash * inverse_s, domain_parameters.order);
            BigInteger u2 = MathBI.mod(r * inverse_s, domain_parameters.order);

            EllipticCurvePoint point1 = new EllipticCurvePoint(domain_parameters);
            point1.multiplyPoint(u1);

            EllipticCurvePoint point2 = new EllipticCurvePoint(public_key);
            point2.multiplyPoint(u2);
            point1.addPoint(point2);

            //Make sure newly generated point is valid.
            if (!point1.validatePoint())
            {
                return false;
            }

            BigInteger x = MathBI.mod(point1.getCoordinate().x, domain_parameters.order);
            r = MathBI.mod(r, domain_parameters.order);

            return r == x;
        }
    }
}
