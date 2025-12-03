using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TxatAurreratua.Util
{
    public class Server
    {
        private const int PORT = 5000;
        private TcpListener? listener;
        public bool alive = false;

        private readonly object BezeroakLock = new();

        public delegate void ILogSent(string log);
        public ILogSent? LogSentEvent;
        public delegate void IMessageSent(string mezua);
        public IMessageSent? MessageSentEvent;
        public delegate void IClientConnected(ServersideClient bezero);
        public IClientConnected? ClientConnectedEvent;
        public delegate void IClientDisconnected(ServersideClient bezero);
        public IClientDisconnected? ClientDisconnectedEvent;

        private readonly List<ServersideClient> Bezeroak = [];

        public void Piztu()
        {
            alive = true;
            new Thread(() =>
            {
                try
                {
                    listener = new(IPAddress.Any, PORT);
                    listener.Start();
                    LogBerria($"ZERBITZARIA hasi da PORT:{PORT}");

                    while (alive)
                    {
                        BezeroBerriaItxaron(listener);
                    }
                }
                catch { LogBerria("Zerbitzari errorea"); Itzali(); }
                finally { listener?.Stop(); Bezeroak.Clear(); }
            }).Start();
        }

        public void Itzali()
        {
            alive = false;
            lock (BezeroakLock)
            {
                listener?.Stop();
                Bezeroak.Clear();
            }
            LogBerria("ZERBITZARIA itzalia da");
        }

        private void BezeroBerriaItxaron(TcpListener listener)
        {
            try
            {
                var bezeroBerria = new ServersideClient(this, listener.AcceptTcpClient());
                if (bezeroBerria != null)
                {
                    lock (BezeroakLock)
                    {
                        Bezeroak.Add(bezeroBerria);
                        ClientConnectedEvent?.Invoke(bezeroBerria);
                    }
                }
            }
            catch (SocketException) { }
            catch (Exception) { LogBerria("Bezero konexio errorea"); }
        }

        public void LogBerria(string log)
        {
            LogSentEvent?.Invoke($"[{DateTime.Now.ToShortTimeString()}] {log}");
        }

        public void MezuBerria(string mezua)
        {
            MessageSentEvent?.Invoke($"[{DateTime.Now.ToShortTimeString()}] {mezua}");
        }

        public void SendEveryone(string mezua)
        {
            MezuBerria(mezua);
            lock (BezeroakLock)
            {
                foreach (var bezero in Bezeroak)
                    bezero.Send($"[{DateTime.Now.ToShortTimeString()}] {mezua}");
            }
        }
    }
}
