using System.Collections.Generic;
using System.Numerics;

namespace ClientForLab.CipherUtils
{
    public static class ClientRsa
    {
        public static List<BigInteger> Encrypt(byte[] aeskey, PublicKey pk)
        {
            BigInteger bi;
            List<BigInteger> encrypted = new List<BigInteger>();

            foreach (var elem in aeskey)
            {
                bi = elem;
                bi = BigInteger.ModPow(bi, pk.E, pk.N);
                encrypted.Add(bi);
            }

            return encrypted;
        }
    }
}
