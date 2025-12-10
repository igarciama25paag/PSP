using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TxatAurreratua.Util
{
    public class ServersideClient
    {
        private readonly Server Server;
        private readonly TcpClient Client;
        private readonly NetworkStream Stream;
        private readonly StreamReader Reader;
        private readonly StreamWriter Writer;
        private readonly string Izena;
        private bool alive;

        public ServersideClient(Server zerbitzari, TcpClient bezero)
        {
            Server = zerbitzari;
            Client = bezero;
            Stream = bezero.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream) { AutoFlush = true };
            Izena = Reader.ReadLine() ?? "null";

            alive = true;
            Server.LogBerria($"Bezero berria '{Izena}'");

            CreateConnectionChecker();
            CreateReceiverThread();
        }

        private void CreateConnectionChecker()
        {
            new Thread(() =>
            {
                while(alive)
                {
                    if (Client.Client.Poll(0, SelectMode.SelectRead))
                        CloseClient();
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private void CreateReceiverThread()
        {
            new Thread(() =>
            {
                try
                {
                    while (alive)
                    {
                        var mezua = Reader?.ReadLine();
                        if (mezua != null)
                        {
                            Server.SendEveryone($"{Izena}: {mezua}");
                        }
                    }
                }
                catch { CloseClient(); }
            }).Start();
        }

        public void Send(string mezua)
        {
            try { Writer?.WriteLine(mezua); }
            catch { CloseClient(); }
        }

        public void CloseClient()
        {
            alive = false;
            Client?.Close();
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();
            lock(Server.BezeroakLock)
            {
                Server.Bezeroak.Remove(this);
            }
            Server.ClientDisconnectedEvent?.Invoke(this);
            Server.LogBerria($"'{Izena}' bezeroa deskonektatu da");
        }

        public override string? ToString() => Izena;
    }
}
