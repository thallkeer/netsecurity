using System.Collections.Generic;
using System.Numerics;

namespace ServerForLab.CipherUtils {
	public static class ServerRsa {
		public static byte[] Decrypt (List<BigInteger> deckey, BigInteger exponent, BigInteger modulus) {
			byte[] key = new byte[deckey.Count];
			BigInteger bi = new BigInteger ();
			for (int i = 0; i < key.Length; i++) {
				bi = BigInteger.ModPow (deckey[i], exponent, modulus);
				key[i] = (byte) bi;
			}

			return key;
		}

	}
}