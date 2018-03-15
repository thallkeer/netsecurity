

namespace ServerForLab.RsaCipher
{
    public class PrivateKey
    {
        public long D { get; }
        public long N { get; }

        public PrivateKey(long d, long n)
        {
            this.D = d;
            this.N = n;
        }
    }
}
