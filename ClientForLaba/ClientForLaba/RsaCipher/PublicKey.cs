using System;

namespace ServerForLab.RsaCipher
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
