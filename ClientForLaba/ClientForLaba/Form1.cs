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
using ServerForLab.RsaCipher;
using RSA = ServerForLab.RsaCipher.RSA;


namespace ClientForLaba
{
    public partial class Form1 : Form
    {
        private static byte[] buf = new byte[1024];
        byte[] file;
        

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
                SendMessageFromSocket("127.0.0.1", 8888);
            }
            catch (SocketException exc)
            {
                ConsoleLog.Text = String.Format("{0} Error code: {1}.", exc.Message, exc.ErrorCode);
            }
            catch (Exception ex)
            {
                ConsoleLog.Text = String.Format(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
        

        public void SendMessageFromSocket(string server,int port)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);

               
                StringBuilder response = new StringBuilder();
                NetworkStream stream = client.GetStream();

                string command = "Собираюсь отправить файл";
                SendCommand(stream,command);

                PublicKey pk = Step1(stream, response);
               
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                bool run=true;
                while (run)
                {
                    command = ReceiveCommand(stream);
                    switch (command)
                    {
                        case "Давай сюда файл":
                        {
                            Step2(stream, aes);
                            break;
                        }
                        case "Давай сюда ключ":
                        {
                            Step3(stream, aes, pk);
                            break;
                        }
                        case "Ну и вектор гони":
                        {
                            Step4(stream, aes, pk);
                            break;
                        }
                        case "Спасибо":
                        {
                            SendCommand(stream, "Конец передачи");
                            stream.Close();
                            client.Close();
                            run = false;
                            break;
                        }
                    }
                }
                //do
                //{
                //    int bytes = stream.Read(data, 0, data.Length);
                //    response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                //} while (stream.DataAvailable); // пока данные есть в потоке
                //MessageLog(response.ToString());


                //stream.Write(file,0,file.Length);
                //MessageLog("Файл отправлен");

                // Закрываем потоки
               
            }
            catch (SocketException e)
            {
                MessageLog($@"SocketException: {e}");
            }
            catch (Exception e)
            {
                MessageLog($@"Exception: {e.Message}");
            }

            MessageLog("Запрос завершен...");
        }

        public PublicKey Step1(NetworkStream stream,StringBuilder response)
        {
            do
            {
                int bytes = stream.Read(buf, 0, buf.Length);
                response.Append(Encoding.UTF8.GetString(buf, 0, bytes));
            } while (stream.DataAvailable); // пока данные есть в потоке
            Thread.Sleep(1000);
            MessageLog("Ключ принят");
            string parameters = response.ToString();
            long exp = Convert.ToInt64(parameters.Split(Convert.ToChar(","))[0]);
            long mod = Convert.ToInt64(parameters.Split(Convert.ToChar(","))[1]);
            return new PublicKey(exp, mod);
        }

        public void Step2(NetworkStream stream, AesCryptoServiceProvider aes)
        {
            byte[] encfile = AES.EncryptFile(file, aes.Key, aes.IV);
            SendCommand(stream, "Отправляю зашифрованный файл");
            stream.Write(encfile, 0, encfile.Length);
            Thread.Sleep(1000);
            MessageLog("Зашифрованный файл отправлен");
        }

        public void Step3(NetworkStream stream, AesCryptoServiceProvider aes, PublicKey pk)
        {
            var keylist = RSA.Encrypt(aes.Key, pk);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            Thread.Sleep(1000);
            MessageLog("Отправляю ключ");
            SendCommand(stream, "Отправляю ключ");
            bf.Serialize(ms, keylist);
            stream.Write(ms.ToArray(),0,ms.ToArray().Length);
            Thread.Sleep(1000);
            MessageLog("Ключ отправлен");
           
        }

        public void Step4(NetworkStream stream, AesCryptoServiceProvider aes, PublicKey pk)
        {
            var ivlist = RSA.Encrypt(aes.IV, pk);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            MessageLog("Отправляю вектор");
            SendCommand(stream, "Отправляю вектор");
            ms.Position = 0;
            bf.Serialize(ms, ivlist);
            stream.Write(ms.ToArray(), 0, ms.ToArray().Length);
            MessageLog("Вектор отправлен");
        }



        public void MessageLog(string str)
        {
            ConsoleLog.Text += DateTime.Now.ToLongTimeString()+ @" " + str + "\n";
        }

        public void MessageLog(FormattableString fs)
        {
            ConsoleLog.Text += DateTime.Now.ToLongTimeString() + @" "+ fs + "\n";
        }

        public static string ReceiveCommand(NetworkStream ns)
        {
            int received = ns.Read(buf, 0, buf.Length);
            return Encoding.UTF8.GetString(buf, 0, received);
        }

        public static void SendCommand(NetworkStream ns, string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            ns.Write(data, 0, data.Length);
        }

    }

}


        
    

