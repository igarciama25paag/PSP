using System.Data;
using System.IO.Pipes;

class Zerbitzari
{
    public static void Main()
    {
        Task.Factory.StartNew(() =>
        {
            NamedPipeServerStream server = new NamedPipeServerStream("Kalkulagailua");
            server.WaitForConnection();
            StreamReader reader = new StreamReader(server);
            StreamWriter writer = new StreamWriter(server);

            while (true)
            {
                var que = reader.ReadLine();
                var ans = new DataTable().Compute(que, "");

                Console.WriteLine("Question recived: " + que);
                Console.WriteLine("Answer sent: " + ans);

                writer.WriteLine(ans);
                writer.Flush();
            }
        }).Wait();
    }
}