using System;
using System.Numerics;

namespace ServerForLab.RSAcipher
{
    [Serializable]
    public class PublicKey
    {
        public long E { get; }
        public long N { get; }

        public PublicKey(long e, long n)
        {
            this.E = e;
            this.N = n;
        }

    }
}
