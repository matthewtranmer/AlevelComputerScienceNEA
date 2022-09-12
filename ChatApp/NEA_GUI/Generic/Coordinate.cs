using System.Numerics;

namespace Matthew_Tranmer_NEA.Generic
{
    class Coordinate
    {
        protected BigInteger x;
        protected BigInteger y;

        public Coordinate(BigInteger x, BigInteger y)
        {
            setCoordinate(x, y);
        }

        public Coordinate(Coordinate coordinate)
        {
            setCoordinate(coordinate);
        }

        public void setCoordinate(BigInteger x, BigInteger y)
        {
            this.x = x;
            this.y = y;
        }

        public void setCoordinate(Coordinate coordinate)
        {
            x = coordinate.x;
            y = coordinate.y;
        }

        public (BigInteger x, BigInteger y) getCoordinate()
        {
            return (x, y);
        }
    }
}
