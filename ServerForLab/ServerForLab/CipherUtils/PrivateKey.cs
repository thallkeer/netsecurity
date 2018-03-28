using System.Numerics;

namespace ServerForLab.RSAcipher
{
    public class PrivateKey
    {
        public BigInteger D { get; }
        public BigInteger N { get; }

        public PrivateKey(BigInteger d, BigInteger n)
        {
            this.D = d;
            this.N = n;
        }
    }
}
