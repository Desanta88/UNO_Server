using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UNO_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int porta = 8000;
            int nClient = 0;
            string username = "";
            TcpClient Client = new TcpClient();
            TcpListener server = new TcpListener(porta);
            server.Start();
            NetworkStream ns;
            while (nClient < 2)
            {
                Byte[] ReceiveBytes = new Byte[Client.ReceiveBufferSize];
                Client=server.AcceptTcpClient();
                ns=Client.GetStream();
                ns.Read(ReceiveBytes,0,Client.SendBufferSize);
                Byte[] SendBytes = Encoding.ASCII.GetBytes("sei connesso");
                ns.Write(SendBytes,0,Client.ReceiveBufferSize);
                nClient++;
                var port = ((IPEndPoint)Client.Client.RemoteEndPoint).Port;

                //HandleClient h = new HandleClient();
                //h.StartClient(username, Client);
            }


        }
    }
    public class HandleClient
    {
        public string Username;
        public TcpClient client;

        public void StartClient(string u, TcpClient c)
        {
            this.Username = u;
            this.client = c;
            Thread th = new Thread(Partita);
            th.Start();
        }
        public void Partita()
        {

        }
    }
}
