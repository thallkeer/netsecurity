using System.Collections.Generic;
using System.Numerics;

namespace ClientForLab.CipherUtils
{
    public static class ClientRsa
    {
        public static List<BigInteger> Encrypt(byte[] aeskey, BigInteger expon, BigInteger modulus)
        {
            BigInteger bi;
            List<BigInteger> encrypted = new List<BigInteger>();

            foreach (var elem in aeskey)
            {
                bi = elem;
                bi = BigInteger.ModPow(bi, expon, modulus);
                encrypted.Add(bi);
            }

            return encrypted;
        }
    }
}
