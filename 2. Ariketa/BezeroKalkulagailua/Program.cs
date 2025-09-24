using System.IO.Pipes;

class Bezero
{
    public static void Main()
    {
        NamedPipeClientStream client = new NamedPipeClientStream("Kalkulagailua");
        client.Connect();
        StreamReader reader = new StreamReader(client);
        StreamWriter writer = new StreamWriter(client);

        int aukera = 0;
        while (aukera != -1)
        {
            Console.WriteLine("""
                    Aukeratu eragiketa:
                    1 Gehiketa
                    2 Kenketa
                    3 Biderketa
                    4 Zatiketa
                    -1 Amaitu
                    """);

            if (Int32.TryParse(Console.ReadLine(), out aukera) && 4 >= aukera && aukera >= 1)
            {
                string eragiketa = aukera switch
                {
                    1 => "+",
                    2 => "-",
                    3 => "*",
                    _ => "/"
                };

                int n1 = aukeratuZenbakia("1. zenbakia");
                int n2 = aukeratuZenbakia("2. zenbakia");

                writer.WriteLine(n1 + eragiketa + n2);
                writer.Flush();
                Console.WriteLine("Erantzuna: " + reader.ReadLine());

            }
            else if (aukera != -1)
            {
                Console.WriteLine("Ez da baliozko aukera");
            }
        }
        Console.WriteLine("Programa amaitu da");
    }

    static int aukeratuZenbakia(String context)
    {
        int n;
        Console.WriteLine("Sartu " + context + ": ");
        while (!Int32.TryParse(Console.ReadLine(), out n))
        {
            Console.WriteLine("Error, sartu " + context + ": ");
        }
        return n;
    }
}