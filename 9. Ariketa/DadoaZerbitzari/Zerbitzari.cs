using System.Collections;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;

namespace DadoaZerbitzari
{
    record Partida(List<Thread> Jokalariak, int Zenbakia)
    {
        public string? Irabazlea { get; set; } = null;
    }

    class Zerbitzari
    {
        const int PORT = 5000;
        static readonly Partida partida = new(
            new(10),
            new Random().Next(100)
            );

        public static void Main(string[] args)
        {
            Console.WriteLine("ZERBITZARIA hasi da");

            try
            {
                using TcpListener listener = new(IPAddress.Any, PORT);
                {
                    listener.Start();
                    Console.WriteLine("Zerbitzaria entzuten dago: port " + PORT);
                    Console.WriteLine("Partida zenbakia: " + partida.Zenbakia);

                    while (partida.Irabazlea == null)
                    {
                        if(partida.Jokalariak.Count < partida.Jokalariak.Capacity)
                            partida.Jokalariak.Add(NewJokalari(listener.AcceptTcpClient()));
                    }

                    Console.WriteLine("Partida amaitu da.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Server error: " + e.ToString());
            }
        }

        private static Thread NewJokalari(TcpClient socket)
        {
            Thread newJokalari = new(_ =>
            {
                string izena = "";
                try
                {
                    using NetworkStream ns = socket.GetStream();
                    using StreamReader reader = new(ns, Encoding.UTF8);
                    using StreamWriter writer = new(ns, Encoding.UTF8);
                    {
                        writer.AutoFlush = true;
                        izena = reader.ReadLine();
                        Console.WriteLine($"Bezero berri bat konektatu da: {izena}");
                        writer.WriteLine("Ongi etorri partidara! Aukeratu zenbaki bat 0-100");

                        while (partida.Irabazlea == null) {
                            int n = Int16.Parse(reader.ReadLine());
                            Console.WriteLine($"{izena}: {n}");

                            if (n != partida.Zenbakia)
                            {
                                writer.WriteLine("0");
                                writer.WriteLine($"{n} ez da zenbaki zuzena: " +
                                    (partida.Zenbakia > n ? "Handiagoa" : "Txikiagoa"));
                            }
                            else
                            {
                                partida.Irabazlea ??= izena;
                                Console.WriteLine($"Partida {izena} irabazi du");
                            }
                        }

                        writer.WriteLine("1");
                        writer.WriteLine($"Partida {partida.Irabazlea} irabazi du. Agur {izena}!");
                        Console.WriteLine($"Bezero {izena} deskonektatu da");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Client error: {izena} disconnected");
                }
                finally { socket.Close(); }
            });
            newJokalari.Start();
            return newJokalari;
        }
    }
}