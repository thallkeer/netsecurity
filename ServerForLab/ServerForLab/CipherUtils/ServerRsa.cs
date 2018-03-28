using System.Collections.Generic;
using System.Numerics;
using ServerForLab.RSAcipher;

namespace ServerForLab.CipherUtils
{
    public class ServerRsa
    {
        public static byte[] Decrypt(List<BigInteger> deckey, PrivateKey pk)
        {
            byte[] key = new byte[deckey.Count];
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
