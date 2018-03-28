using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ServerForLab.RSAcipher;

namespace ServerForLab.CipherUtils
{
   public class KeyPair
    {
        public BigInteger P { get; set; }
        public BigInteger Q { get; set; }
        public BigInteger N { get; set; } //произведение
        public BigInteger Fi { get; set; } //ф-ия Эйлера
        public readonly BigInteger E = 65537;
        public BigInteger D { get; set; } //секретная экспонента
        public PrivateKey PrivateKey { get; set; }
        public PublicKey PublicKey { get; set; }

        //public static readonly long[] primes =
        //{
        //    3271, 3299, 3301, 3307, 3313, 3319, 3323,
        //    4099, 4111, 4127, 4129, 4133, 4139, 4153, 4157, 4159, 4177, 4201, 4211
        //};

        public KeyPair()
        {
            //Random r = new Random();
            //P = primes[r.Next(primes.Length)];
            //Q = primes[r.Next(primes.Length)];
            RSACryptoServiceProvider rcsp = new RSACryptoServiceProvider();
            RSAParameters rsap = rcsp.ExportParameters(true);
            P = new BigInteger(rsap.P.Reverse().Concat(new byte[] { 0 }).ToArray());
            Q = new BigInteger(rsap.Q.Reverse().Concat(new byte[] { 0 }).ToArray());
            GenerateKeys();
        }

        private void GenerateKeys()
        {
            N = P * Q;
            Fi = (P - 1) * (Q - 1);
            //Gcd(E, Fi, out long x, out long y);
            //BigInteger Dd = x < 0 ? x + Fi : x;
            CalculateD();
            D = D < 0 ? D + Fi : D;
            PublicKey = new PublicKey(E, N);
            PrivateKey = new PrivateKey(D, N);
        }

        private long Gcd(long a, long b, out long x, out long y)
        {
            if (b < a)
            {
                var t = a;
                a = b;
                b = t;
            }

            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }

            long gcd = Gcd(b % a, a, out x, out y);

            long newY = x;
            long newX = y - (b / a) * x;

            x = newX;
            y = newY;
            return gcd;
        }

        private void CalculateD()
        {
            BigInteger tmp_D;

            for (int k = 2; ; k++)
            {
                D = (Fi * k + 1) / E;
                tmp_D = (Fi * k + 1) % E;

                // We verify that d is integral, so in this case tmp_D must be 0
                if (tmp_D == 0)
                {
                    break;
                }
            }
        }
    }
}
