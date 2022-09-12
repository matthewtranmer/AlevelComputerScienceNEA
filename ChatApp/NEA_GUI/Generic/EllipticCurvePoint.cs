using System.Numerics;

namespace Matthew_Tranmer_NEA.Generic
{
    class EllipticCurvePoint : Coordinate
    {
        private DomainParameters domain_parameters;

        //Create an ECpoint set to the given coordinates
        public EllipticCurvePoint(Coordinate coordinate, DomainParameters domain_parameters) : base(coordinate)
        {
            this.domain_parameters = domain_parameters;
        }

        //Create an ECpoint set to the coordinates of the given ECpoint
        public EllipticCurvePoint(EllipticCurvePoint point) : base(point.x, point.y)
        {
            domain_parameters = point.domain_parameters;
        }

        //Create an ECpoint set to the coordinates of the generator.
        public EllipticCurvePoint(DomainParameters domain_parameters) : base(domain_parameters.generator)
        {
            this.domain_parameters = domain_parameters;
        }

        //Create an ECpoint set to the coordinates of the compressed point.
        public EllipticCurvePoint(Span<byte> compressed_point, DomainParameters domain_parameters) : base(domain_parameters.generator)
        {
            this.domain_parameters = domain_parameters;

            Coordinate coordinate = decompressPoint(compressed_point);
            setCoordinate(coordinate);
        }

        //Takes the x coordinate and the sign bit of a point then solves the equation
        //y^2 = x^3 + ax + b to extract the y coordinate.
        private Coordinate decompressPoint(Span<byte> compressed_point)
        {
            //Extract the sign bit which is held in the last byte.
            byte sign = compressed_point[compressed_point.Length - 1];

            //Extract the x coordinate which is held in the first bytes exept the last.
            BigInteger x = new BigInteger(compressed_point.Slice(0, compressed_point.Length - 1), true, true);

            //Calculate y squared.
            BigInteger y_squared = BigInteger.Pow(x, 3) + domain_parameters.a * x + domain_parameters.b;

            //The square root has two solutions, and the sign bit allows us to choose which solution it is.
            BigInteger y = MathBI.modularSquareRoot(y_squared, domain_parameters.modulus);

            //first solution test.
            if (y % 2 != sign)
            {
                y = domain_parameters.modulus - y;
            }

            return new Coordinate(x, y);
        }

        //Produce a compressed representation of a point.
        public Span<byte> compressPoint()
        {
            //Convert y coordinate to a byte array.
            byte[] y_bytes = y.ToByteArray(true, true);

            //Get least significant bit of the y array.
            int last_byte = y_bytes[y_bytes.Length - 1];
            byte sign = (byte)(last_byte & 1);

            //Convert x coordinate to a byte array.
            byte[] x_bytes = x.ToByteArray(true, true);

            //Append the x bytes and the sign bit to produce the compressed point.
            Span<byte> compressed_point = new byte[x_bytes.Length + 1];
            x_bytes.CopyTo(compressed_point);
            compressed_point[compressed_point.Length - 1] = sign;

            return compressed_point;
        }

        public DomainParameters getDomainParameters()
        {
            return domain_parameters;
        }

        //Calculate the inverse of the intersect of a line through the given two points.
        private Coordinate calculateInverseIntersect(Coordinate first, Coordinate second, BigInteger gradient)
        {
            //Get values from given coordinates
            var first_values = first.getCoordinate();
            var second_values = second.getCoordinate();

            //Calculate inverse intersect
            BigInteger x = MathBI.mod(BigInteger.Pow(gradient, 2) - first_values.x - second_values.x, domain_parameters.modulus);
            BigInteger y = MathBI.mod(gradient * (first_values.x - x) - first_values.y, domain_parameters.modulus);

            return new Coordinate(x, y);
        }

        //Double the point.
        public Coordinate doublePoint()
        {
            //Calculate gradient of tangent on the point.
            BigInteger gradient = 3 * BigInteger.Pow(x, 2) + domain_parameters.a;
            gradient *= MathBI.modularMultiplicativeInverse(2 * y, domain_parameters.modulus);
            gradient = MathBI.mod(gradient, domain_parameters.modulus);

            //Calculate the inverse intersect.
            Coordinate intersect = calculateInverseIntersect(this, this, gradient);

            //Set the current point to the doubled point.
            setCoordinate(intersect);
            return intersect;
        }

        //Add point with given point.
        public Coordinate addPoint(Coordinate coordinate)
        {
            //Get values from given coordinate.
            var values = coordinate.getCoordinate();

            if (values.x == x)
            {
                return doublePoint();
            }

            //Calculate gradient of line through points.
            BigInteger gradient = values.y - y;
            gradient *= MathBI.modularMultiplicativeInverse(values.x - x, domain_parameters.modulus);
            gradient = MathBI.mod(gradient, domain_parameters.modulus);

            //Calculate the inverse intersect.
            Coordinate intersect = calculateInverseIntersect(this, coordinate, gradient);

            //Set the current point to the added point.
            setCoordinate(intersect);
            return intersect;
        }

        //Multiplies the current point by the given multiplier.
        public Coordinate multiplyPoint(BigInteger multiplier)
        {
            //Point that will be doubled.
            EllipticCurvePoint series_point = new EllipticCurvePoint(this);

            bool first_point = true;
            //Binary series.
            while (multiplier > 0)
            {
                if (multiplier % 2 == 1)
                {
                    if (first_point)
                    {
                        setCoordinate(series_point);
                        first_point = false;
                    }
                    else
                    {
                        addPoint(series_point);
                    }
                }

                series_point.doublePoint();
                multiplier /= 2;
            }

            return new Coordinate(x, y);
        }

        //Ensures the coordinate is a valid point on the curve.
        public bool validatePoint()
        {
            //Calculates what y squared should be from the domain parameters and the x value.
            BigInteger y_squared = MathBI.mod(BigInteger.Pow(x, 3) + domain_parameters.a * x + domain_parameters.b, domain_parameters.modulus);
            //Calculates y squared by squaring the y value.
            BigInteger real_y_squared = MathBI.mod(BigInteger.Pow(y, 2), domain_parameters.modulus);

            if (y_squared != real_y_squared)
            {
                return false;
            }

            return true;
        }
    }
}
