using System.IO;
using System.Net.Sockets;
using System.Text;

class Bezero
{
    const string SERVER = "127.0.0.1";
    const int PORT = 5000;
    private static readonly string? ImgDir = 
        Directory.GetParent(Environment.CurrentDirectory)?
        .Parent?
        .Parent?
        .Parent?
        .FullName + "\\";

    public static void Main(string[] args)
    {
        Console.WriteLine("BEZEROA hasi da");

        Console.Write("Sartu zure izena: ");
        string izena = Console.ReadLine();
        string userPath = $"{ImgDir}{izena}_irudiak";
        if (!Directory.Exists(userPath))
        {
            Directory.CreateDirectory(userPath);
            Console.WriteLine($"""
                Carpeta berri bat sortu da '{izena}'-(r)entzat:
                {userPath}
                """);
        }
        else Console.WriteLine($"Ongi entorri {izena}");

        try
        {
            using TcpClient client = new();
            client.Connect(SERVER, PORT);
            using NetworkStream ns = client.GetStream();
            using StreamReader reader = new(ns, Encoding.UTF8);
            using StreamWriter writer = new(ns, Encoding.UTF8);
            {
                writer.AutoFlush = true;
                byte[] bytes = new byte[client.ReceiveBufferSize];

                for (int i = 0; i < 5; i++)
                    Console.WriteLine(reader.ReadLine());

                int result = 0;
                while (result != 4)
                {
                    Console.Write("Aukera: ");
                    if (Int32.TryParse(Console.ReadLine(), out result))
                    {
                        writer.WriteLine(result);
                        Console.WriteLine(reader.ReadLine());

                        byte[] imgBytes = new byte[Int32.Parse(reader.ReadLine())];

                        int newBytes = -1;
                        for (int i = 0; i < imgBytes.Length && newBytes != 0; i += newBytes)
                            newBytes = ns.Read(imgBytes, i, imgBytes.Length - i);

                        string fileName = reader.ReadLine();
                        File.WriteAllBytes(userPath + "\\" + fileName, imgBytes);
                        Console.WriteLine("Image download: " + userPath + "\\" + fileName);
                    }
                    else Console.WriteLine("Error: formatu ezegokia");
                }

                Console.WriteLine(reader.ReadLine());
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }

        Console.WriteLine("Konexioa amaitu da");
    }
}