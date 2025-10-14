class Hariak
{
    public static void Main(String[] args)
    {
        Thread trd1 = new Thread(Hariak.aimar);
        Thread trd2 = new Thread(Hariak.nerea);
        Thread trd3 = new Thread(Hariak.jurgi);
        trd1.Start();
        trd2.Start();
        trd3.Start();
        trd1.Join();
        trd2.Join();
        trd3.Join();
    }

    public static void aimar()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Aimar");
            Thread.Sleep(300);
        }
    }
    public static void nerea()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Nerea");
            Thread.Sleep(1000);
        }
    }
    public static void jurgi()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Jurgi");
            Thread.Sleep(500);
        }
    }
}