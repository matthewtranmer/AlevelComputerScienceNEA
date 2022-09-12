using System.Numerics;
using System.Globalization;

namespace Matthew_Tranmer_NEA.Generic
{
    static class PreDefinedCurves
    {
        public static DomainParameters nist256 = new DomainParameters(
            modulus: BigInteger.Parse("0ffffffff00000001000000000000000000000000ffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier),
            a: BigInteger.Parse("0ffffffff00000001000000000000000000000000fffffffffffffffffffffffc", NumberStyles.AllowHexSpecifier),
            b: BigInteger.Parse("05ac635d8aa3a93e7b3ebbd55769886bc651d06b0cc53b0f63bce3c3e27d2604b", NumberStyles.AllowHexSpecifier),
            order: BigInteger.Parse("0ffffffff00000000ffffffffffffffffbce6faada7179e84f3b9cac2fc632551", NumberStyles.AllowHexSpecifier),
            generator: new Coordinate(
                x: BigInteger.Parse("06b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296", NumberStyles.AllowHexSpecifier),
                y: BigInteger.Parse("04fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5", NumberStyles.AllowHexSpecifier)
            )
        );
    }
}
