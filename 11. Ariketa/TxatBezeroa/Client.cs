using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TxatBezeroa
{
    public class Client
    {
        private const int PORT = 5000;
        private TcpClient? client;
        private NetworkStream? Stream;
        private StreamReader? Reader;
        private StreamWriter? Writer;

        public delegate void ILogSent(string log);
        public ILogSent? LogSentEvent;
        public delegate void IMessageArrived(string mezua);
        public IMessageArrived? MessageArrivedEvent;
        public delegate void IConnected();
        public IConnected? ConnectedEvent;
        public delegate void IDisconnected();
        public IDisconnected? DisconnectedEvent;

        private string Izena;
        private bool alive = false;

        public void Konektatu(string ip, string izena)
        {
            client = new();
            Izena = izena;
            alive = true;
            try
            {
                client.Connect(ip, PORT);

                Stream = client.GetStream();
                Reader = new StreamReader(Stream);
                Writer = new StreamWriter(Stream) { AutoFlush = true };

                Writer.WriteLine(Izena);

                LogBerria("Zerbitzarira konektatuta");
                ConnectedEvent?.Invoke();

                CreateConnectionChecker();
                CreateReceiverThread();
            }
            catch { BezeroaItxi("Ezin izan da zerbitzaria atzitu"); }
        }

        private void CreateConnectionChecker()
        {
            new Thread(() =>
            {
                while (alive)
                {
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                        BezeroaItxi("Konexioa amaitu da");
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
                            MessageArrivedEvent?.Invoke(mezua);
                        }
                    }
                }
                catch { BezeroaItxi("Konexioa amaitu da"); }
            }).Start();
        }

        private void LogBerria(string log) => LogSentEvent?.Invoke(log);

        public void MezuaBidali(string mezua)
        {
            try { Writer?.WriteLine(mezua); }
            catch { BezeroaItxi("Konexioa amaitu da"); }
        }

        public void BezeroaItxi(string? log)
        {
            alive = false;
            client?.Close();
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();
            DisconnectedEvent?.Invoke();
            if(log != null) LogBerria(log);
        }
    }
}
