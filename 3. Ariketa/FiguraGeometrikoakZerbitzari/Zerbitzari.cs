using System.IO.Pipes;

Console.WriteLine("<== ZERBITZARIA ==>");
var server = new NamedPipeServerStream("geometria");
server.WaitForConnection();

var reader = new StreamReader(server);
var writer = new StreamWriter(server);

while (true)
{
    var que = reader.ReadLine();
    Console.WriteLine("Iritsitako galdera: " + que);

    String[] splQue = que.Split("|");
    double ans, r;
    Double.TryParse(splQue[1], out r);

    if (splQue[0].Equals("1"))
    {
        ans = Math.PI * Math.Pow(r, 2);
    } else
    {
        double n = splQue[0] switch
        {
            "2" => 3,
            "3" => 4,
            _ => 5,
        };

        double P = n * 2 * r * Math.Sin(180 / n);
        double A = r * Math.Cos(180 / n);
        ans = P * A / 2;
    }

    Console.WriteLine("Bidalitako erantzuna: " + ans);
    writer.WriteLine(ans);
    writer.Flush();
}