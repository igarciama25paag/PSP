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

        public ServersideClient(Server zerbitzari, TcpClient bezero)
        {
            Server = zerbitzari;
            Client = bezero;
            Stream = bezero.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream)
            {
                AutoFlush = true
            };
            Izena = Reader.ReadLine() ?? "null";

            Server.LogBerria($"Bezero berria '{Izena}'");
            CreateReceiverThread();
        }

        private void CreateReceiverThread()
        {
            new Thread(() =>
            {
                try
                {
                    while (Server.alive)
                    {
                        var mezua = Reader.ReadLine();
                        if (mezua != null)
                        {
                            if (mezua == "/disconnect")
                                CloseClient();
                            else
                                Server.SendEveryone($"{Izena}: {mezua}");
                        }
                    }
                    Send("Zerbitzaria itzali da");
                }
                catch { Server.LogBerria($"{Izena} bezero errorea"); }
                finally { CloseClient(); }
            }).Start();
        }

        public void Send(string mezua) => Writer?.WriteLine(mezua);

        public void CloseClient()
        {
            Server.BezeroaEzabatu(this);
            Server.ClientDisconnectedEvent?.Invoke(this);
            Server.LogBerria($"'{Izena}' bezeroa deskonektatu da");
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();
        }

        public override string? ToString() => Izena;
    }
}
