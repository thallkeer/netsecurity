using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ServerForLab.CipherUtils;

namespace ServerForLab
{
   public class Server
    {
        private const int port = 8888;
        private IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        private static byte[] buf = new byte[1024];
        private static KeyPair pair;
        private static List<BigInteger> key, iv;
        private TcpListener server;
        private static NetworkStream stream;

        public Server()
        {
            server = new TcpListener(localAddr, port);
        }

        public void StartServer()
        {
            try
            {
                // запуск слушателя
                server.Start();
                File.Delete(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png");
                File.Delete(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png");
                while (true)
                {
                    Console.WriteLine("Ожидание подключений... ");

                    // получаем входящее подключение
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " Подключен клиент. Выполнение запроса...");
                 
                    // получаем сетевой поток для чтения и записи
                    stream = client.GetStream();

                    while (true)
                    {
                        string command = ReceiveCommand();
                        switch (command)
                        {
                            case Commands.InitTransmition:
                            {
                                SendPublicKey();
                                SendCommand(Commands.KeyRequest);
                                break;
                            }
                            case Commands.SendKey:
                            {
                                key = ReceiveEncKeyVector();
                                SendCommand(Commands.VectorRequest);
                                break;
                            }
                            case Commands.SendVector:
                            {
                                iv = ReceiveEncKeyVector();
                                //DecryptFile();
                                SendCommand(Commands.FileRequest);
                                break;
                            }
                            case Commands.SendEncFile:
                            {
                                var deckey = ServerRsa.Decrypt(key, pair.PrivateKey);
                                var deciv = ServerRsa.Decrypt(iv, pair.PrivateKey);
                                ReceiveEncFile(deckey,deciv);
                                SendCommand(Commands.Handshake);
                                break;
                            }
                            case Commands.EndTransmition:
                            {
                                client.Close();
                                System.Diagnostics.Process.Start(
                                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                                    @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png");
                                Console.ReadKey();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server?.Stop();
            }
        }

        private void SendPublicKey()
        {
            pair = new KeyPair();
            StringBuilder sb = new StringBuilder();
            sb.Append(pair.PublicKey.E);
            sb.Append(",");
            sb.Append(pair.PublicKey.N);
            byte[] tosend = Encoding.UTF8.GetBytes(sb.ToString());
            stream.Write(tosend, 0, tosend.Length);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Ключ rsa отправлен");
           
        }

        private List<BigInteger> ReceiveEncKeyVector()
        {
            int count = 0;
            MemoryStream mstream = new MemoryStream();
            do
            {
                int bytes = stream.Read(buf, 0, buf.Length);
                count += bytes;
                mstream.Write(buf, 0, buf.Length);
            } while (stream.DataAvailable); // пока данные есть в потоке

            BinaryFormatter bf = new BinaryFormatter();
            mstream.Position = 0;
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Получено байтов: {0}", count);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Зашифрованный ключ/вектор получен");
            return (List<BigInteger>)bf.Deserialize(mstream);
        }

        private void ReceiveEncFile(byte[] key, byte[] iv)
        {
            int count = 0;
            byte[] tmp;
            do
            {
                int bytes = stream.Read(buf, 0, buf.Length);
                if (bytes < 1024)
                {
                    Array.Resize(ref buf,bytes);
                }
                count += bytes;

                using (FileStream fstream = new FileStream(
                    @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png",
                    FileMode.Append))
                {
                    tmp = ServerAes.DecryptFile(buf, key, iv);
                    fstream.Write(tmp, 0, tmp.Length);
                }
                buf = new byte[1024];
               

            } while (stream.DataAvailable); // пока данные есть в потоке

            //byte[] abc = File.ReadAllBytes(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png");
            //decfile = ServerAes.DecryptFile(abc, key, iv);
            //File.WriteAllBytes(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png", decfile);

            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Получено байтов: {0}", count);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Зашифрованный файл получен");
            
        }

        private static void DecryptFile()
        {
            var deckey = ServerRsa.Decrypt(key, pair.PrivateKey);
            var deciv = ServerRsa.Decrypt(iv, pair.PrivateKey);
            var encfile = File.ReadAllBytes(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png");
            var decfile = ServerAes.DecryptFile(encfile, deckey, deciv);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + $" Файл длиной {decfile.Length} расшифрован");
            File.WriteAllBytes(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png", decfile);
        }

        private static void SendCommand(string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
        }

        private static string ReceiveCommand()
        {
            int received = stream.Read(buf, 0, buf.Length);
            return Encoding.UTF8.GetString(buf, 0, received);
        }
    }
}
