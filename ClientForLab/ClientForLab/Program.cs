using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClientForLab.CipherUtils;

namespace ClientForLab
{
    class Program
    {
        private const string filename = "C:\\Users\\kir73\\OneDrive\\Рабочий стол\\Homer.png";
        
        static void Main(string[] args)
        {
            byte[] file = File.ReadAllBytes(filename);
            MySha256 sha = new MySha256();
            PrintByteArray(sha.computeHash(file));

            SHA256 sha256 = SHA256.Create();
            PrintByteArray(sha256.ComputeHash(file));
            do
            {
                Console.WriteLine("Для начала нажмите Enter");
            } while (Console.ReadKey().Key != ConsoleKey.Enter);
            try
            {
                Client client = new Client(1024, file);
                client.Process("127.0.0.1", 8888);
            }
            catch (SocketException exc)
            {
                Console.WriteLine($@"{exc.Message} Error code: {exc.ErrorCode}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format(ex.ToString()));
            }
            finally
            {
                Console.ReadLine();
            }

        }

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

    }
}
