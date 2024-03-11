using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UNO_Server
{
    internal class Program
    {
        public static string GetCardCodes(int nc)
        {
            string[] simboli = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "r", "s", "p2" };
            string[] colori = new string[] { "y", "g", "b", "re", "p4", "cc" };
            string codes = "";
            Random r = new Random();
            for (int i = 0; i < nc; i++)
            {
                int nSimbolo = r.Next(0, simboli.Length);
                int nColore = r.Next(0, colori.Length);
                if (colori[nColore] == "p4" || colori[nColore] == "cc")
                {
                    codes += colori[nColore]+":"+ colori[nColore];
                    if (i < nc - 1)
                    {
                        codes += ";";
                    }
                }
                else
                {
                    codes += colori[nColore] + ":" + simboli[nSimbolo];
                    if (i < nc - 1)
                    {
                        codes += ";";
                    }
                }
            }
            return codes;
        }
        public static string GetNeutralCard()
        {
            bool hasColor = false;
            string card="";
            while (hasColor == false)
            {
                card = GetCardCodes(1);
                if (card.Contains("p4")== false && card.Contains("cc") ==false)
                    hasColor = true;
            }
            return card;
        }
        static void Main(string[] args)
        {
            int porta = 8000;
            int nClient = 0;
            string username = "";
            TcpClient Client = new TcpClient();
            //TcpListener server = new TcpListener(porta);
            Server server = new Server(porta);
            server.Start();
            //Console.WriteLine("server in ascolto");
            NetworkStream ns;
            Dictionary<int, TcpClient> giocatori = new Dictionary<int, TcpClient>();
            Dictionary<int, string> usernames = new Dictionary<int, string>();
            HandleClient h=new HandleClient();
            int clientId = 0;
            /*while (nClient <= 2)
            {
                if (nClient != 2)
                {
                    clientId++;
                    Byte[] ReceiveBytes = new Byte[Client.ReceiveBufferSize];
                    Client = server.AcceptTcpClient();
                    giocatori.Add(clientId, Client);
                    ns = Client.GetStream();
                    ns.Read(ReceiveBytes, 0, Client.ReceiveBufferSize);
                    string ricevuto = Encoding.ASCII.GetString(ReceiveBytes).Replace("\0", "");
                    Console.WriteLine($"connessione del giocatore:{ricevuto}");
                    username = ricevuto;
                    usernames.Add(clientId, username);
                    Byte[] SendBytes = Encoding.ASCII.GetBytes("sei connesso");
        
                   
                    var port = ((IPEndPoint)Client.Client.RemoteEndPoint).Port;

                    h = new HandleClient();
                    nClient++;
                }
                if(nClient == 2)
                {
                    h.setPlayers(giocatori);
                    foreach (var client in giocatori)
                    {
                        Byte[] risposta = Encoding.ASCII.GetBytes("start");
                        if (client.Value.Connected == false)
                            server.AcceptTcpClient();
                        NetworkStream netStream = client.Value.GetStream();
                        netStream.Write(risposta, 0, risposta.Length);
                        h.StartClient(username, client.Value, client.Key,server);
                        

                    }
                    Random rId = new Random();
                    int PrimoTurno = rId.Next(1, 3);
                    Byte[] carta = Encoding.ASCII.GetBytes(GetNeutralCard());
                    foreach (var client in giocatori)
                    {
                        NetworkStream n;
                        n = client.Value.GetStream();
                        n.Write(carta, 0, carta.Length);
                        if (PrimoTurno == client.Key)
                        {
                            foreach(var c in giocatori)
                            {
                                n = c.Value.GetStream();
                                Byte[] answer = Encoding.ASCII.GetBytes(usernames[PrimoTurno]);
                                server.AcceptTcpClient();
                                n.Write(answer, 0, answer.Length);
                                n.Close();
                            }
         
                        }

                    }
                }
            }*/
            
            //Console.WriteLine("Raggiunto il numero di client massimi");
            //server.Stop();
            //Console.WriteLine("Chiuso server");


        }
    }
    public class HandleClient
    {
        public string Username;
        public TcpClient client;
        public int clientId;
        public Dictionary<int, TcpClient> players;
        public TcpListener Server;

        public void StartClient(string u, TcpClient c,int id,TcpListener s)
        {
            this.Username = u;
            this.client = c;
            clientId = id;
            Server = s;
            Thread th = new Thread(Partita);
            th.Start();
        }
        public void setPlayers(Dictionary<int,TcpClient> p)
        {
            players = p;
        }
        public void Partita()
        {
            bool connesso = true;
            Byte[] BytesDalClient=new Byte[client.ReceiveBufferSize];
            Byte[] BytesDalServer= new Byte[client.ReceiveBufferSize];
            string rispostaClient = "";
            string rispostaServer = "";

            while (connesso)
            {
                if (client.Connected == false)
                    Server.AcceptTcpClient();
                NetworkStream networkStream = client.GetStream();
                networkStream.Read(BytesDalClient, 0, client.ReceiveBufferSize);
                rispostaClient = Encoding.ASCII.GetString(BytesDalClient).Replace("\0","");
                Console.WriteLine($"codice ricevuto:{rispostaClient} da utente:{this.Username}");
                if (rispostaClient.Contains("et"))
                {
                    rispostaClient=rispostaClient.Replace("et", "yt");
                    sendToOtherPlayer(rispostaClient, clientId);
                }

            }
        }
        public void sendToOtherPlayer(string messaggio,int idSender)
        {
            foreach(var c in players)
            {
                if (idSender != c.Key)
                {
                    TcpClient OtherPlayer = c.Value;
                    Byte[] risposta =Encoding.ASCII.GetBytes(messaggio) ;
                    NetworkStream ns = OtherPlayer.GetStream();
                    ns.Write(risposta,0,risposta.Length);
                    ns.Close();

                }
                
            }
        }

    }
}
