using System.Numerics;

namespace Matthew_Tranmer_NEA.Generic
{
    class KeyPair
    {
        private BigInteger private_component;
        private EllipticCurvePoint public_component;

        //Create a random key pair.
        public KeyPair(DomainParameters parameters)
        {
            private_component = MathBI.randomInteger(parameters.order - 1);

            //Create public component.
            public_component = new EllipticCurvePoint(parameters);
            public_component.multiplyPoint(private_component);
        }

        //Create a key pair with a specified private component.
        public KeyPair(BigInteger private_component, DomainParameters parameters)
        {
            this.private_component = private_component;

            //Create public component.
            public_component = new EllipticCurvePoint(parameters);
            public_component.multiplyPoint(private_component);
        }

        public BigInteger getPrivateComponent()
        {
            return private_component;
        }

        public EllipticCurvePoint getPublicComponent()
        {
            return public_component;
        }


    }
}
