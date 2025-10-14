using System.IO.Pipes;

Console.WriteLine("<== BEZEROA ==>");
NamedPipeClientStream client = new NamedPipeClientStream("geometria");
client.Connect();
StreamReader reader = new StreamReader(client);
StreamWriter writer = new StreamWriter(client);

int aukera = 0;
while (aukera != -1)
{
    Console.WriteLine("""
                Aukeratu poligonoa:
                1 Zirkulua
                2 Triangelua
                3 Karratua
                4 Pentagonoa
                -1 Amaitu
                """);

    if (Int32.TryParse(Console.ReadLine(), out aukera) && 4 >= aukera && aukera >= 1)
    {
        int n;
        Console.WriteLine("Sartu erradioa: ");
        while (!Int32.TryParse(Console.ReadLine(), out n))
        {
            Console.WriteLine("Error, sartu erradioa: ");
        }

        writer.WriteLine(aukera + "|" + n);
        writer.Flush();
        Console.WriteLine("Erantzuna: " + reader.ReadLine());
    }
    else if (aukera != -1)
    {
        Console.WriteLine("Ez da baliozko aukera");
    }
}
Console.WriteLine("Programa amaitu da");