using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientForLaba
{
   public class NetClient
   {
       private NetworkStream NetStream { get; set; }
       private BufferedStream BufStream { get; set; }
       const int dataArraySize = 100;
       const int streamBufferSize = 1024;
       const int numberOfLoops = 10000;


       public NetClient(NetworkStream ns, BufferedStream bs)
       {
           NetStream = ns;
           BufStream = bs;
       }

      
    }
}
