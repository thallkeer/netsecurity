using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerForLab.RSAcipher;

namespace ServerForLab
{
    public class NetServer
    {
        private Socket clientSocket;
        private bool running = true;
        private PublicKey publicKey;
        private PrivateKey privateKey;
        private MemoryStream msin;
        private MemoryStream msout;

        public NetServer(Socket socket, PublicKey pk, PrivateKey prk)
        {
            clientSocket = socket;
            publicKey = pk;
            privateKey = prk;
        }

        public void run()
        {
            try
            {
                msin = new MemoryStream();
                msout = new MemoryStream();
                BinaryReader br = new BinaryReader(msin);
                BinaryWriter bw = new BinaryWriter(msout);
                byte[] buf = new byte[1024];
                int receive;
                while (running)
                {
                    int received = clientSocket.Receive(buf);
                    string command = Encoding.UTF8.GetString(buf,0,received);
                    switch (command)
                    {
                        case "запрос ключей":
                        {
                            StreamWriter sw = new StreamWriter(msout);
                            sw.AutoFlush = true;
                            sw.WriteLine(publicKey.E);
                            sw.WriteLine(publicKey.N);
                            clientSocket.Send(msout.ToArray());
                            
                            break;
                        }
                        case "отправляю файл":
                        {
                            BufferedStream bs = new BufferedStream(msin);
                            byte[] buffile = new byte[1024];
                            FileStream fs = new FileStream("C:\\Users\\kir73\\OneDrive\\Рабочий стол\\DecHomer.png",FileMode.Append);
                            
                            
                            while (clientSocket.Available > 0)
                            {
                                clientSocket.Receive(buffile,1024,SocketFlags.None);
                                fs.Write(buffile,0,buffile.Length);
                            }
                            break;
                        }
                        default:
                        {

                            Thread.Sleep(1000);
                            break;
                        }
                        
                    }
                }
                msin.Close();
                msout.Close();
                clientSocket.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
            }
        }
    }

}
