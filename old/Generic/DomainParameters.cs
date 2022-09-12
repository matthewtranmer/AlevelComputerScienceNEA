using System.Numerics;

namespace Matthew_Tranmer_NEA.Generic
{
    class DomainParameters
    {
        public BigInteger modulus;
        public BigInteger a;
        public BigInteger b;
        public BigInteger order;
        public Coordinate generator;

        public DomainParameters(BigInteger modulus, BigInteger a, BigInteger b, BigInteger order, Coordinate generator)
        {
            this.modulus = modulus;
            this.a = a;
            this.b = b;
            this.order = order;
            this.generator = generator;
        }

        public DomainParameters(BigInteger modulus, BigInteger a, BigInteger b, BigInteger order, BigInteger x, BigInteger y)
        {
            this.modulus = modulus;
            this.a = a;
            this.b = b;
            this.order = order;
            generator = new Coordinate(x, y);
        }
    }
}
