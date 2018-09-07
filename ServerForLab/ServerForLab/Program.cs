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
using ServerForLab.CipherUtils;


namespace ServerForLab
{
    class Program
    {
        
        static void Main(string[] args)
        {
           Server server = new Server();
           server.StartServer();
        }
    }
}


    




