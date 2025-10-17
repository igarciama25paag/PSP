class Edurnezuri
{
    public readonly static Object janLocker = new();
    public static void Main()
    {
        Ipotx[] ipotxak =
        {
            new("Dok"),
            new("Irribarri"),
            new("Mutur"),
            new("Lohi"),
            new("Tinko"),
            new("Hala"),
            new("Txiki")
        };

        // Edurnezuri bizitza
        while (true)
        {
            Zerbitzatu();
            Paseatu();
        }
    }

    public static void Zerbitzatu()
    {
        lock (janLocker)
        {
            Console.WriteLine("EDURNEZURI ipotxak zerbitzatu");
            Monitor.PulseAll(janLocker);
        }
    }

    public static void Paseatu()
    {
        Console.WriteLine("EDURNEZURI 5000 segunduz paseatzen");
        Thread.Sleep(5000);
    }
}

static class Mahaia
{
    public static Ipotx[] exerlekuak = new Ipotx[4];
    public readonly static Object eseriLocker = new();
}

class Ipotx
{
    private string Izena { get; set; }
    private readonly Random random = new();

    public Ipotx(string izena)
    {
        Izena = izena;

        // Ipotx bizitza
        new Thread(() =>
        {
            while (true)
            {
                Lanera();
                ExeriEtaJan();
            }
        }).Start();
    }

    void Lanera()
    {
        int time = random.Next(2000, 5000);
        Console.WriteLine("Lanean " + time + " segunduz: " + Izena);
        Thread.Sleep(time);
    }

    void ExeriEtaJan()
    {
        // Eserlekua bilatu
        int n = -1;
        while (n == -1)
        {
            lock (Mahaia.eseriLocker)
            {
                n = Array.IndexOf(Mahaia.exerlekuak, null);
                // Eserleku bat libre geratzea itxaron
                if (n == -1) Monitor.Wait(Mahaia.eseriLocker);
                // Eseri
                else Mahaia.exerlekuak[n] = this;
            }
        }

        // Zerbitzatzen itxaroten
        lock (Edurnezuri.janLocker)
        {
            Console.WriteLine("Zerbitzatzeari itxaroten: " + Izena);
            Monitor.Wait(Edurnezuri.janLocker);
        }

        // Jaten
        int time = random.Next(2000, 5000);
        Console.WriteLine("Jaten " + time + " segunduz: " + Izena);
        Thread.Sleep(time);

        // Eserlekua libre utzi
        lock (Mahaia.eseriLocker)
        {
            Mahaia.exerlekuak[Array.IndexOf(Mahaia.exerlekuak, this)] = null;
            Monitor.PulseAll(Mahaia.eseriLocker);
        }
    }
}