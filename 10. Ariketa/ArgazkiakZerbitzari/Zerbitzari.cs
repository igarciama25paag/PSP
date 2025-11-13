using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Zerbitzari
{
    const int PORT = 5000;
    const string IMG_DIR = "D:\\PSP\\Ariketak\\10. Ariketa\\ImgZerbitzari";
    private static readonly List<Thread> Connections = new(5);
    private static readonly Dictionary<int, string> Images = [];

    public static void Main(string[] args)
    {
        Console.WriteLine("ZERBITZARIA hasi da");
        Images.Add(1, "mendi_argazkia.jpg");
        Images.Add(2, "hondartza_argazkia.jpg");
        Images.Add(3, "hiri_argazkia.jpg");

        try
        {
            using TcpListener listener = new(IPAddress.Any, PORT);
            {
                listener.Start();
                Console.WriteLine("Zerbitzaria entzuten dago: port " + PORT);

                while(true)
                    if (Connections.Count < Connections.Capacity)
                        Connections.Add(NewConnection(listener.AcceptTcpClient()));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Server error: " + e.ToString());
        }

        Console.WriteLine("ZERBITZARIA itxi da");
    }

    private static Thread NewConnection(TcpClient socket)
    {
        Thread connection = new(_ =>
        {
            string izena = "";
            try
            {
                using NetworkStream ns = socket.GetStream();
                using StreamReader reader = new(ns, Encoding.UTF8);
                using StreamWriter writer = new(ns, Encoding.UTF8);
                {
                    writer.AutoFlush = true;
                    string name = socket.Client.RemoteEndPoint.ToString();
                    Console.WriteLine($"Bezero berri bat konektatu da: {name}");
                    writer.WriteLine("""
                        Ongi etorri argazki zerbitzarira, aukeratu:
                        1 - Mendi argazkia
                        2 - Hondartza argazkia
                        3 - Hiri argazkia
                        4 - Irten
                        """);

                    int emaitza = 0;
                    while (emaitza != 4)
                    {
                        if(Int32.TryParse(reader.ReadLine(), out emaitza) && emaitza != 4)
                        {
                            Console.WriteLine($"{name}: {emaitza}");
                            writer.WriteLine($"'{Images[emaitza]}' aukeratu duzu");

                            byte[] imgBytes = File.ReadAllBytes(IMG_DIR +"\\"+ Images[emaitza]);
                            writer.WriteLine(imgBytes.Length);
                            ns.Write(imgBytes, 0, imgBytes.Length);

                            writer.WriteLine(Images[emaitza]);
                            Console.WriteLine($"'{Images[emaitza]}' irudia bidalita");
                        }
                    }

                    writer.WriteLine("Agur");
                    Console.WriteLine($"Bezero {izena} deskonektatu da");
                }
            }
            catch
            {
                Console.WriteLine($"Client error: {izena} disconnected");
            }
            finally { socket.Close(); }
        });
        connection.Start();
        return connection;
    }
}