using System;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using CipherUtilities;
using ClientForLab.CipherUtils;

namespace ClientForLab
{
    public class Client
    {
        private static byte[] buf;
        byte[] file;
        private readonly TcpClient client;
        private static NetworkStream stream;

        public Client(int bufsize,byte[] file)
        {
            client = new TcpClient();
            buf = new byte[bufsize];
            this.file = file;
        }

        public void Process(string server, int port)
        {
            try
            {
                client.Connect(server, port);
                StringBuilder response = new StringBuilder();
                stream = client.GetStream();
                string command = Commands.InitTransmition;
                SendCommand(command);


                PublicKey pk = GetPublicKey(response);
                PrivateKey myKey = null;

                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                bool run = true;
                while (run)
                {
                    command = ReceiveCommand();
                    switch (command)
                    {
                        case Commands.KeyRequest:
                        {
                            SendEncKeyVector(aes.Key, pk);
                            break;
                        }
                        case Commands.VectorRequest:
                        {
                            SendEncKeyVector(aes.IV, pk);
                            break;
                        }
                        case Commands.FileRequest:
                        {
                             SendEncFile(aes);
                             break;
                        }
                        case Commands.PublicKeyRequest:
                        {
                            myKey = SendPublicKey();
                            break;
                        }
                        case Commands.HashRequest:
                        {
                            SendFileHash(myKey);
                            break;
                        }
                       
                        case Commands.Handshake:
                        {
                            SendCommand(Commands.EndTransmition);
                            stream.Close();
                            client.Close();
                            run = false;
                            break;
                        }
                    }
                }

            }
            catch (SocketException e)
            {
                Console.WriteLine($@"SocketException: {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine(($@"Exception: {e.Message}"));
            }

            MessageLog("Запрос завершен...");
        }

        private void SendFileHash(PrivateKey pk)
        {
            //SHA256 mySHA256 = SHA256.Create();
            MySha256 clientSha = new MySha256();

           

            //var hashValue = mySHA256.ComputeHash(file);
            var hashValue = clientSha.computeHash(file);
            PrintByteArray(hashValue);
           
            var hash = ClientRsa.Encrypt(hashValue,pk.D,pk.N);

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            Console.WriteLine(Commands.SendHash);
            SendCommand(Commands.SendHash);
            bf.Serialize(ms, hash);
            stream.Write(ms.ToArray(), 0, ms.ToArray().Length);
            MessageLog("Хеш отправлен");
           
        }

        private PrivateKey SendPublicKey()
        {
           KeyPair kp = new KeyPair();
            StringBuilder sb = new StringBuilder();
            sb.Append(kp.PublicKey.E);
            sb.Append(",");
            sb.Append(kp.PublicKey.N);
            byte[] tosend = Encoding.UTF8.GetBytes(sb.ToString());
            SendCommand(Commands.SendPublicKey);
            stream.Write(tosend, 0, tosend.Length);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Ключ rsa отправлен");
            return kp.PrivateKey;
        }

        // Print the byte array in a readable format.
        public static void PrintByteArray(byte[] array)
        {
            int i;
            for (i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]:X2}");
                if ((i % 4) == 3) Console.Write(" ");
            }
            Console.WriteLine();
        }

        private PublicKey GetPublicKey(StringBuilder response)
        {
            do
            {
                int bytes = stream.Read(buf, 0, buf.Length);
                response.Append(Encoding.UTF8.GetString(buf, 0, bytes));
            } while (stream.DataAvailable); // пока данные есть в потоке

            MessageLog("Ключ принят");
            string parameters = response.ToString();
            BigInteger exp = BigInteger.Parse(parameters.Split(Convert.ToChar(","))[0]);
            BigInteger mod = BigInteger.Parse(parameters.Split(Convert.ToChar(","))[1]);
            return new PublicKey(exp, mod);
        }

        private void SendEncFile(AesCryptoServiceProvider aes)
        {
            byte[] encfile = ClientAes.EncryptFile(file, aes.Key, aes.IV);
            SendCommand(Commands.SendEncFile);
            stream.Write(encfile, 0, encfile.Length);
            MessageLog("Зашифрованный файл отправлен");
        }

        private void SendEncKeyVector(byte[] toEncrypt, PublicKey pk)
        {
            var listToSend = ClientRsa.Encrypt(toEncrypt, pk.E,pk.N);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            MessageLog("Отправляю ключ");
            SendCommand(toEncrypt.Length == 32 ? Commands.SendKey : Commands.SendVector);

            bf.Serialize(ms, listToSend);
            stream.Write(ms.ToArray(), 0, ms.ToArray().Length);
            MessageLog("Ключ отправлен");

        }

        public void MessageLog(string str)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + @" " + str);
        }

        public void MessageLog(FormattableString fs)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + @" " + fs);
        }

        private static string ReceiveCommand()
        {
            int received = stream.Read(buf, 0, buf.Length);
            return Encoding.UTF8.GetString(buf, 0, received);
        }

        private static void SendCommand(string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
        }

    }
}
