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
            Paseatu();
            Zerbitzatu();
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
    private static Ipotx[] exerlekuak = new Ipotx[4];
    public readonly static Object eseriLocker = new();

    public static int EserlekurikLibre() => Array.IndexOf(exerlekuak, null);
    public static void Eseri(Ipotx ipotxa, int exerlekua) => exerlekuak[exerlekua] = ipotxa;
    public static void Altsatu(Ipotx ipotxa) => exerlekuak[Array.IndexOf(exerlekuak, ipotxa)] = null;
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
                n = Mahaia.EserlekurikLibre();
                // Eserleku bat libre geratzea itxaron
                if (n == -1) Monitor.Wait(Mahaia.eseriLocker);
                // Eseri
                else Mahaia.Eseri(this, n);
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
            Mahaia.Altsatu(this);
            Monitor.Pulse(Mahaia.eseriLocker);
        }
    }
}