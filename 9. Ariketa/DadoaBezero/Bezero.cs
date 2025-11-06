using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DadoaBezero
{
    class Bezero
    {
        const string SERVER = "127.0.0.1";
        const int PORT = 5000;
        static string izena = "";

        public static void Main(string[] args)
        {
            Console.WriteLine("BEZEROA hasi da");
            Console.Write("Sartu izena: ");
            izena = Console.ReadLine();
            try
            {
                using TcpClient client = new();
                client.Connect(SERVER, PORT);
                using NetworkStream ns = client.GetStream();
                using StreamReader reader = new(ns, Encoding.UTF8);
                using StreamWriter writer = new(ns, Encoding.UTF8);
                {
                    writer.AutoFlush = true;

                    writer.WriteLine(izena);
                    Console.WriteLine(reader.ReadLine());

                    bool amaituta = false;
                    while (!amaituta)
                    {
                        int n = -1;
                        while (n < 0 || 100 < n)
                        {
                            Console.Write("Sartu zenbaki bat: ");
                            try { 
                                n = Int16.Parse(Console.ReadLine());
                                if (n < 0 || 100 < n)
                                    Console.WriteLine("Balio oker bat sartu da!");
                            }
                            catch { Console.WriteLine("Balio oker bat sartu da!"); }
                        }
                        writer.WriteLine(n);

                        amaituta = reader.ReadLine() == "1";
                        Console.WriteLine(reader.ReadLine());
                    }
                    Console.WriteLine("Konexioa amaitu da");
                }
            } catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}