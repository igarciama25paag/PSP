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
        private TcpClient client;
        private NetworkStream? Stream;
        private StreamReader? Reader;
        private StreamWriter? Writer;

        public delegate void ILogSent(string log);
        public ILogSent? LogSentEvent;
        public delegate void IMessageArrived(string mezua);
        public IMessageArrived? MessageArrivedEvent;

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
                LogBerria("Zerbitzarira konektatuta");

                Stream = client.GetStream();
                Reader = new StreamReader(Stream);
                Writer = new StreamWriter(Stream)
                {
                    AutoFlush = true
                };

                Writer.WriteLine(Izena);

                CreateReceiverThread();
            }
            catch { BezeroaItxi(); }
        }

        private void CreateReceiverThread()
        {
            new Thread(() =>
            {
                try
                {
                    while (alive)
                    {
                        MessageArrivedEvent?.Invoke(Reader?.ReadLine());
                    }
                }
                catch { BezeroaItxi(); }
            }).Start();
        }

        private void LogBerria(string log)
        {
            LogSentEvent?.Invoke(log);
        }

        public void MezuaBidali(string mezua)
        {
            Writer?.WriteLine(mezua);
        }

        public void BezeroaItxi()
        {
            alive = false;
            LogBerria("Konexioa amaitu da");
            client?.Close();
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();
        }
    }
}
