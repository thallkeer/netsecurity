﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ServerForLab.CipherUtils;

namespace ServerForLab
{
   public class Server
    {
        private const int port = 8888;
        private IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        private static string EncFilePath = @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\EncFile.png";
        private static string DecFilePath = @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png";
        private static byte[] buf = new byte[1024];
        private static byte[] hash;
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
                File.Delete(EncFilePath);
                File.Delete(DecFilePath);
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
                                SendCommand(Commands.HashRequest);
                                break;
                            }
                            case Commands.SendHash:
                            {
                                ReceiveHash();
                                SendCommand(Commands.Handshake);
                                break;
                            }
                            case Commands.EndTransmition:
                            {
                                client.Close();
                                if (CheckHash(hash))
                                {
                                    Console.WriteLine("Хеш совпадает!");
                                    System.Diagnostics.Process.Start(
                                        @"C:\Program Files(x86)\Google\Chrome\Application\chrome.exe",
                                        @"C:\Users\kir73\source\repos\netsecurity\ServerForLab\DecFile.png");
                                }
                                else
                                {
                                    Console.WriteLine("Ошибка, хеш не совпадает!");
                                }

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
            do
            {
                int bytes = stream.Read(buf, 0, buf.Length);
                count += bytes;

                using (FileStream fstream = new FileStream(
                    EncFilePath,
                    FileMode.Append))
                {
                    fstream.Write(buf, 0, bytes);//buf.length
                }

            } while (stream.DataAvailable); // пока данные есть в потоке

            byte[] abc = File.ReadAllBytes(EncFilePath);
            byte[] decfile = ServerAes.DecryptFile(abc, key, iv);
            byte[] length = new byte[4];
            Array.Copy(decfile,0,length,0,4);
            var arrlength = BitConverter.ToInt32(length,0);
            byte[] tmpDecFile = new byte[arrlength];
            Array.Copy(decfile,4,tmpDecFile,0,tmpDecFile.Length);
            File.WriteAllBytes(DecFilePath, tmpDecFile);

            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Получено байтов: {0}", count);
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " Зашифрованный файл получен");
            
        }

        private static void ReceiveHash()
        {
            MemoryStream mstream = new MemoryStream();
            do
            {
                stream.Read(buf, 0, buf.Length);
                mstream.Write(buf, 0, buf.Length);
            } while (stream.DataAvailable); // пока данные есть в потоке

            BinaryFormatter bf = new BinaryFormatter();
            mstream.Position = 0;
            var encHash = (List<BigInteger>) bf.Deserialize(mstream);
            hash = ServerRsa.Decrypt(encHash,pair.PrivateKey);
        }

        private static bool CheckHash(byte[] decryptedHash)
        {
            SHA256 mySHA256 = SHA256.Create();
            byte[] decryptedFile =
                File.ReadAllBytes(DecFilePath);
            //Console.WriteLine("Файл");
            //PrintByteArray(decryptedFile);
            var hashValue = mySHA256.ComputeHash(decryptedFile);
            Console.WriteLine("Хеш расшифрованного файла");
            PrintByteArray(hashValue);
            Console.WriteLine("Полученный хеш");
            PrintByteArray(decryptedHash);
            for (int i = 0; i < decryptedHash.Length; i++)
            {
                if (decryptedHash[i] != hashValue[i]) return false;
            }

            return true;
        }

        private static void PrintByteArray(byte[] array)
        {
            int i;
            for (i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]:X2}");
                if ((i % 4) == 3) Console.Write(" ");
            }
            Console.WriteLine();
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
