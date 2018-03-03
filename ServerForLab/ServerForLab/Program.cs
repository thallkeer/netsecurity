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
using ServerForLab.RSAcipher;
using System.Threading;

namespace ServerForLab
{
    class Program
    {
        private const int port = 8888;
        private static TcpListener listener;
        private static byte[] buf = new byte[1000000];
        private static MemoryStream ms = new MemoryStream();//(new byte[256], 0, 256, true, true);
        private static BinaryReader br =new BinaryReader(ms);
        private static BinaryFormatter bf = new BinaryFormatter();
        
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение

                    Socket handler = sListener.Accept();
                    

                    
                    //int received = handler.Receive(buf);
                    //handler.Send(Encoding.Default.GetBytes(received.ToString()));
                    //byte[] file = buf;

                    // br.Read(file,0,file.Length);
                    
                   
                    int received = handler.Receive(buf);
                    //File.WriteAllBytes("C:\\Users\\kir73\\OneDrive\\Рабочий стол\\ABC.png", buf);
                    BinaryWriter bw = new BinaryWriter(ms);
                    bw.Write(buf);
                    ms.Position = 0;
                    List<BigInteger> Key = (List<BigInteger>) bf.Deserialize(ms);
                   


                    //handler.Send(Encoding.Default.GetBytes(received.ToString()));
                    received = handler.Receive(buf);
                    bw = new BinaryWriter(ms);
                    bw.Write(buf);
                    ms.Position = 0;
                    List<BigInteger> IV = (List<BigInteger>)bf.Deserialize(ms);



                    //var decrypted = AES.DecryptFile(, Key, IV);
                    //File.WriteAllBytes("C:\\Users\\kir73\\OneDrive\\Рабочий стол\\DecHomer.png", decrypted);

                    //BinaryFormatter bf = new BinaryFormatter();
                    //List<BigInteger> forrsa = (List<BigInteger>)bf.Deserialize(ms);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }


            }

    //try
    //{
    //listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
    //listener.Start();
    //Console.WriteLine("Ожидание подключений...");

    //while (true)
    //{
    //    TcpClient client = listener.AcceptTcpClient();
    //    ClientObject clientObject = new ClientObject(client);

    //    // создаем новый поток для обслуживания нового клиента
    //    Thread clientThread = new Thread(clientObject.Process);
    //    clientThread.Start();
    //}
    //}
    //catch (Exception ex)
    //{
    //Console.WriteLine(ex.Message);
    //}
    //finally
    //{
    //listener?.Stop();
    //}
   
}
