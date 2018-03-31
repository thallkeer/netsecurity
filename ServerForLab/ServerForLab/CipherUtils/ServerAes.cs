using System;
using System.IO;
using System.Security.Cryptography;

namespace ServerForLab.CipherUtils
{
   public static class ServerAes
    {
        public static byte[] DecryptFile(byte[] cipheredFile, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipheredFile == null || cipheredFile.Length <= 0)
                throw new ArgumentNullException("cipheredFile");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            byte[] decrypted = new byte[cipheredFile.Length];
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipheredFile))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        //using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        using (BufferedStream bs = new BufferedStream(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting 
                            //stream

                            csDecrypt.Read(decrypted, 0, decrypted.Length);
                            csDecrypt.Close();

                        }
                    }
                }

            }
            return decrypted;
        }
    }
}
