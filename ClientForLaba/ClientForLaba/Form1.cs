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
using ClientForLaba.CipherUtils;
using ServerForLab;
using ServerForLab.RsaCipher;


namespace ClientForLaba
{
    public partial class Form1 : Form
    {
        private byte[] file;
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
                Client client = new Client(1024,file);
                client.Process("127.0.0.1", 8888);
            }
            catch (SocketException exc)
            {
                //ConsoleLog.Text = $@"{exc.Message} Error code: {exc.ErrorCode}.";
            }
            catch (Exception ex)
            {
                //ConsoleLog.Text = String.Format(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
        

       

       

    }

}


        
    

