using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Matthew_Tranmer_NEA.Generic;

namespace Matthew_Tranmer_NEA.E2E
{
    internal class PreKey
    {
        public string identifier { get; }
        public KeyPair key { get; }

        public PreKey(KeyPair key)
        {
            identifier = Guid.NewGuid().ToString();
            this.key = key;
        }
        public PreKey(KeyPair key, string identifier)
        {
            this.key = key;
            this.identifier = identifier;
        }
    }
}
