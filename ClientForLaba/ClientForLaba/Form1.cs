using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerForLab;
using ServerForLab.RSAcipher;
using RSA = ServerForLab.RSAcipher.RSA;

namespace ClientForLaba
{
    public partial class Form1 : Form
    {
        const int port = 8888;
        const string address = "127.0.0.1";
        private static byte[] file;
        private static MemoryStream ms = new MemoryStream();
       private static BinaryWriter bw =new BinaryWriter(ms);
        private static BinaryFormatter bf = new BinaryFormatter();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (openFD.ShowDialog() == DialogResult.OK)
            {
                string filename = openFD.FileName;
                file = File.ReadAllBytes(filename);

            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessageFromSocket(11000);
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

        public void SendMessageFromSocket(int port)
        {
            // Соединяемся с удаленным устройством
            
            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            //Console.Write("Введите сообщение: ");
            //string message = Console.ReadLine();

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            //byte[] msg = Encoding.UTF8.GetBytes(message);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                //byte[] encrypted = AES.EncryptFile(file, aes.Key, aes.IV);
                //sender.Send(encrypted);
                //int bytesRec = sender.Receive(ms.GetBuffer());
                //ConsoleLog.Text += String.Format("\nОтвет от сервера: получено {0}\n\n", Encoding.UTF8.GetString(ms.GetBuffer(), 0, bytesRec));

                //sender.Send(File.ReadAllBytes("C:\\Users\\kir73\\OneDrive\\Рабочий стол\\Homer.png"));

                RSA rsa = new RSA();
                var enckey = RSA.Encrypt(aes.Key, rsa.PublicKey);
                bf.Serialize(ms, enckey);
                sender.Send(ms.ToArray());
                //int bytesRec = sender.Receive(ms.GetBuffer());
                //ConsoleLog.Text += String.Format("\nОтвет от сервера: получено {0}\n\n", Encoding.UTF8.GetString(ms.GetBuffer(), 0, bytesRec));
                //ms.Position = 0;

                //var enciv = RSA.Encrypt(aes.IV, rsa.PublicKey);
                //bf.Serialize(ms, enciv);
                //sender.Send(ms.ToArray());
                //bytesRec = sender.Receive(ms.GetBuffer());
                //ConsoleLog.Text += String.Format("\nОтвет от сервера: получено {0}\n\n", Encoding.UTF8.GetString(ms.GetBuffer(), 0, bytesRec));




                //bw.Write(aes.Key);
                //bw.Write(aes.IV);
                //byte[] enclist = new byte[ms.Length];
                //ms.Position = 0;
                //ms.Read(enclist, 0, enclist.Length);
                //ms.Close();
            }
            // Получаем ответ от сервера
            

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            //if (message.IndexOf("<TheEnd>") == -1)
            //SendMessageFromSocket(port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public void TcpClient()
        {
            string userName = "keer";
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();

                //while (true)
                //{
                    ConsoleLog.Text += userName + ": ";
                    // ввод сообщения
                    string message = ConsoleLog.Text + "\n";
                    message = String.Format("{0}: {1}", userName, message);
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    // отправка сообщения
                    stream.Write(data, 0, data.Length);

                    // получаем ответ
                    data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable && bytes!=0);

                    message = builder.ToString();
                    ConsoleLog.Text += String.Format("Сервер: {0}", message);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpClient();
        }

        public byte[] ToBinary(string param)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(param);
            bw.Flush();
            ms.Flush();
            return ms.ToArray();
        }
    }
}


        
    

