using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using RSA = ServerForLab.RSAcipher.RSA;

namespace ServerForLab
{
    class Program
    {
        private const int port = 8888;
        private static byte[] buf = new byte[1024];
        private static RSA rsa;
        private static List<BigInteger> key, iv;
        static void Main(string[] args)
        {
            TcpListener server = null;
            
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);

                // запуск слушателя
                server.Start();

                while (true)
                {
                    Console.WriteLine("Ожидание подключений... ");

                    // получаем входящее подключение
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " Подключен клиент. Выполнение запроса...");
                    File.Delete(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png");
                    File.Delete(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png");

                    // получаем сетевой поток для чтения и записи
                    NetworkStream stream = client.GetStream();

                    //// сообщение для отправки клиенту
                    //string response = "Привет мир";
                    //// преобразуем сообщение в массив байтов
                    //byte[] data = Encoding.UTF8.GetBytes(response);

                    //// отправка сообщения
                    //stream.Write(data, 0, data.Length);
                    //Console.WriteLine("Отправлено сообщение: {0}", response);
                    while (true)
                    {
                        string command = ReceiveCommand(stream);
                        if (command.Contains("Отправляю ключ"))
                        {
                            command = "Отправляю ключ";
                        }
                        //Console.Beep();
                        switch (command)
                        {
                            case "Собираюсь отправить файл":
                            {
                                rsa = new RSA();
                                StringBuilder sb = new StringBuilder();
                                sb.Append(rsa.PublicKey.E);
                                sb.Append(",");
                                sb.Append(rsa.PublicKey.N);
                                byte[] tosend = Encoding.UTF8.GetBytes(sb.ToString());
                                stream.Write(tosend, 0, tosend.Length);
                                Console.WriteLine(DateTime.Now.ToLongTimeString()+" Ключ rsa отправлен");
                                SendCommand(stream, "Давай сюда файл");
                                break;
                            }
                            case "Отправляю зашифрованный файл":
                            {
                                int count = 0;
                                do
                                {
                                    int bytes = stream.Read(buf, 0, buf.Length);
                                    count += bytes;
                                    using (FileStream fstream = new FileStream(
                                        @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png",
                                        FileMode.Append))
                                    {
                                        fstream.Write(buf, 0, buf.Length);
                                    }

                                } while (stream.DataAvailable); // пока данные есть в потоке

                                Console.WriteLine(DateTime.Now.ToLongTimeString()+" Получено байтов: {0}", count);
                                Console.WriteLine(DateTime.Now.ToLongTimeString()+" Зашифрованный файл получен");
                                SendCommand(stream, "Давай сюда ключ");
                                    break;
                            }
                            case "Отправляю ключ":
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
                                key = (List<BigInteger>)bf.Deserialize(mstream);
                                Console.WriteLine(DateTime.Now.ToLongTimeString()+" Получено байтов: {0}", count);
                                Console.WriteLine(DateTime.Now.ToLongTimeString()+" Зашифрованный ключ получен");
                                SendCommand(stream,"Ну и вектор гони");
                                break;
                            }
                            case "Отправляю вектор":
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
                                iv = (List<BigInteger>)bf.Deserialize(mstream);
                                Console.WriteLine(DateTime.Now.ToLongTimeString() + " Получено байтов: {0}", count);
                                Console.WriteLine(DateTime.Now.ToLongTimeString() + " Зашифрованный вектор получен");
                                Process();
                               
                                SendCommand(stream, "Спасибо");
                                break;
                            }
                            case "Конец передачи":
                            {
                                client.Close();
                                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png");
                                Console.ReadKey();
                                break;
                            }
                        }
                    }

                    //do
                //{
                //    int bytes = stream.Read(buf, 0, buf.Length);
                //    using (FileStream fstream = new FileStream(@"C:\Users\kir73\OneDrive\Рабочий стол\DecHomer", FileMode.Append))
                //    {
                //        fstream.Write(buf,0,buf.Length);
                //    }

                //} while (stream.DataAvailable); // пока данные есть в потоке
                //Console.WriteLine("Файл записан");
                // закрываем подключение

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

        public static void Process()
        {
            var deckey = RSA.Decrypt(key, rsa.PrivateKey);
            var deciv = RSA.Decrypt(iv, rsa.PrivateKey);
            var encfile = File.ReadAllBytes(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png");
            var decfile = AES.DecryptFile(encfile, deckey, deciv);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + $" Файл длиной {decfile.Length} расшифрован");
            File.WriteAllBytes(@"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png",decfile);

            
        }

        public static void SendCommand(NetworkStream ns, string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            ns.Write(data, 0, data.Length);
        }

        public static string ReceiveCommand(NetworkStream ns)
        {
            int received = ns.Read(buf, 0, buf.Length);
            return Encoding.UTF8.GetString(buf,0,received);
        }
    }
}


    




