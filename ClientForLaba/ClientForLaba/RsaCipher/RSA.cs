using System;
using System.Collections.Generic;
using System.Numerics;

namespace ServerForLab.RsaCipher
{
    public class RSA
    {
        public long P { get; set; }
        public long Q { get; set; }
        public long N { get; set; } //произведение
        public long Fi { get; set; } //ф-ия Эйлера
        public const long E = 257; //открытая экспонента
        public long D { get; set; } //секретная экспонента
        public PrivateKey PrivateKey { get; set; }
        public PublicKey PublicKey { get; set; }

        private static readonly long[] primes =
        {
            3271, 3299, 3301, 3307, 3313, 3319, 3323,
            4099, 4111, 4127, 4129, 4133, 4139, 4153, 4157, 4159, 4177, 4201, 4211
        };

        public RSA()
        {
            Random r = new Random();
            P = primes[r.Next(primes.Length)];
            Q = primes[r.Next(primes.Length)];
            GenerateKeys();

           
           

        }

        public void GenerateKeys()
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

            for (int k = 2;; k++)
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

        public static List<BigInteger> Encrypt(byte[] aeskey, PublicKey pk)
        {
            BigInteger bi = new BigInteger();
            List<BigInteger> encrypted = new List<BigInteger>();
            

            for (var index = 0; index < aeskey.Length; index++)
            {
                var elem = aeskey[index];
                bi = elem;
                bi = BigInteger.ModPow(bi, pk.E, pk.N);
                encrypted.Add(bi);
            }

            return encrypted;
        }

        public static byte[] Decrypt(List<BigInteger> deckey, PrivateKey pk)
        {
            byte[] key = new byte[32];
            BigInteger bi = new BigInteger();
            for (int i = 0; i < key.Length; i++)
            {
                bi = BigInteger.ModPow(deckey[i], pk.D, pk.N);
                key[i] = (byte)bi;
            }

            return key;
        }




    }
}
