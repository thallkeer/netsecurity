using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClientForLaba.CipherUtils;
using ServerForLab;
using ServerForLab.RsaCipher;

namespace ClientForLaba
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

                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                bool run = true;
                while (run)
                {
                    command = ReceiveCommand();
                    switch (command)
                    {
                        case Commands.FileRequest:
                        {
                             SendEncFile(aes);
                             break;
                        }
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
                //MessageLog($@"SocketException: {e}");
            }
            catch (Exception e)
            {
                //MessageLog($@"Exception: {e.Message}");
            }

            //MessageLog("Запрос завершен...");
        }

        private PublicKey GetPublicKey(StringBuilder response)
        {
            do
            {
                int bytes = stream.Read(buf, 0, buf.Length);
                response.Append(Encoding.UTF8.GetString(buf, 0, bytes));
            } while (stream.DataAvailable); // пока данные есть в потоке

            //MessageLog("Ключ принят");
            string parameters = response.ToString();
            BigInteger exp = BigInteger.Parse(parameters.Split(Convert.ToChar(","))[0]);
            BigInteger mod = BigInteger.Parse(parameters.Split(Convert.ToChar(","))[1]);
            return new PublicKey(exp, mod);
        }

        private void SendEncFile(AesCryptoServiceProvider aes)
        {
            int bytes = 0;
            byte[] buffer = new byte[1024];
            int count = 0;

            using (FileStream fstream = new FileStream(
                @"C:\Users\kir73\OneDrive\Рабочий стол\Homer.png",
                FileMode.Open))
            {
                    bytes = fstream.Read(buffer, 0, buffer.Length);
                    if (bytes < 1024)
                    {
                        Array.Resize(ref buffer, bytes);
                    }
                    byte[] encfile = ClientAes.EncryptFile(buffer, aes.Key, aes.IV);
                    SendCommand(Commands.SendEncFile);
                    stream.Write(encfile, 0, encfile.Length);
                    stream.Flush();
                    buffer = new byte[1024];
            }
                
               
            //MessageLog("Зашифрованный файл отправлен");
        }

        private void SendEncKeyVector(byte[] toEncrypt, PublicKey pk)
        {
            var listToSend = ClientRsa.Encrypt(toEncrypt, pk);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            //MessageLog("Отправляю ключ");
            SendCommand(toEncrypt.Length == 32 ? Commands.SendKey : Commands.SendVector);

            bf.Serialize(ms, listToSend);
            stream.Write(ms.ToArray(), 0, ms.ToArray().Length);
            //MessageLog("Ключ отправлен");

        }

        //public void MessageLog(string str)
        //{
        //    ConsoleLog.Text += DateTime.Now.ToLongTimeString() + @" " + str + "\n";
        //}

        //public void MessageLog(FormattableString fs)
        //{
        //    ConsoleLog.Text += DateTime.Now.ToLongTimeString() + @" " + fs + "\n";
        //}

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
