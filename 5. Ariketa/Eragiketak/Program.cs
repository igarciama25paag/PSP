static class Eragiketak
{
    public static void Main(string[] args)
    {
        Thread thr1 = new Thread(() => Eragiketak.batu(3, 5));
        Thread thr2 = new Thread(() => Eragiketak.bidertu(4, 6));
        Thread thr3 = new Thread(() => Eragiketak.bidertu(7, 8));

        thr1.Start();
        thr2.Start();

        thr1.Join();
        thr2.Join();

        thr3.Start();
        thr3.Join();
    }

    public static void batu(int x, int y)
    {
        Console.WriteLine("Batu: " + x + " + " + y);
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(x + y);
            Thread.Sleep(300);
        }
    }

    public static void bidertu(int x, int y)
    {
        Console.WriteLine("Bidertu: " + x + " * " + y);
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(x * y);
            Thread.Sleep(1000);
        }
    }
}