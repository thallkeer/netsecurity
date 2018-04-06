using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ClientForLab.CipherUtils
{
   public static class ClientAes
    {
        public static byte[] EncryptFile(byte[] file, byte[] key, byte[] iv)
        {
            byte[] encrypted;
            byte[] length = BitConverter.GetBytes(file.Length);
            byte[] toEncrypt = new byte[file.Length+length.Length];
            Array.Copy(length,toEncrypt,length.Length);
            Array.Copy(file,0,toEncrypt,length.Length,file.Length);
            
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BufferedStream swEncrypt = new BufferedStream(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }
    }
}
