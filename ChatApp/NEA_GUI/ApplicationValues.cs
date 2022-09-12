using System.Numerics;
using System.Net.Sockets;

using Matthew_Tranmer_NEA.E2E;

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
        static public Socket managment_tunnel;
    }
}
