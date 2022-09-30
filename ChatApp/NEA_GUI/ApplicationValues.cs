using System.Numerics;
using System.Net.Sockets;

using Matthew_Tranmer_NEA.Generic; 
using Matthew_Tranmer_NEA.E2E;
using Matthew_Tranmer_NEA.Networking;

namespace NEA_GUI
{
    //Values to be used by the application.
    internal static class ApplicationValues
    {
        static public string username;
        static public string encryption_key;
        static public string session_token;
        static public BigInteger signed_pre_key;
        static public BigInteger identity_key;
        static public List<PreKey> pre_keys = new List<PreKey>();
        static public EncryptedSocketWrapper managment_tunnel;
        static public EllipticCurvePoint server_pk = new EllipticCurvePoint(new Coordinate(BigInteger.Parse("89630596470571539848842129232432250117878455304252638950051962197460885296971"), BigInteger.Parse("81151631255409626093048500128742932208733034782939337963036891136896518229386")), PreDefinedCurves.nist256);
    }
}
