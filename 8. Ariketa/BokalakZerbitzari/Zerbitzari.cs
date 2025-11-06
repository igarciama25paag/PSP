using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace BokalakZerbitzari
{
    class Zerbitzari
    {
        private static readonly int PORT = 5000;

        public static void Main(string[] args)
        {
            Console.WriteLine("ZERBITZARIA hasi da");

            TcpListener listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Any, PORT);
                listener.Start();
                Console.WriteLine("Zerbitzaria entzuten dago: port " + PORT);

                while (true)
                {
                    TcpClient client = null;
                    NetworkStream ns = null;
                    StreamReader reader = null;
                    StreamWriter writer = null;

                    try
                    {
                        client = listener.AcceptTcpClient();
                        ns = client.GetStream();
                        reader = new StreamReader(ns, Encoding.UTF8);
                        writer = new StreamWriter(ns, Encoding.UTF8);
                        writer.AutoFlush = true;

                        Console.WriteLine("Bezero batek konektatu da: " + client.Client.RemoteEndPoint);

                        string line;
                        while ((line = reader.ReadLine()) != null
                            && !line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Jaso: " + line);
                            writer.WriteLine("Server-ek jasota: " + line);
                        }
                        Console.WriteLine("Jaso: " + line);
                        writer.WriteLine("Agur!");

                        Console.WriteLine("Bezeroarekin konexioa amaitu da.");
                    }
                    catch (Exception exClient)
                    {
                        Console.WriteLine("Bezeroaren errorea: " + exClient.Message);
                    }
                    finally
                    {
                        writer?.Close();
                        reader?.Close();
                        ns?.Close();
                        client?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errorea: " + ex.Message);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                    Console.WriteLine("Zerbitzaria gelditu da.");
                }
            }
        }
    }
}