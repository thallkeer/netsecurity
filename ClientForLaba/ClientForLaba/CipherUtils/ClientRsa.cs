using System.Collections.Generic;
using System.Numerics;
using ServerForLab.RsaCipher;

namespace ClientForLaba.CipherUtils
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
