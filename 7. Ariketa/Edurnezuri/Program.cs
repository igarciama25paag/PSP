class Edurnezuri
{
    public static void main(string[] args)
    {
        Aulki[] mahaia = { new Aulki(), new Aulki(), new Aulki(), new Aulki() };
        while (true)
        {
            Thread.Sleep(3000);
        }
    }
}

class Ipotx
{
    private string izena { get; set; }
    private Thread bizitza;

    Ipotx(string izena)
    {
        this.izena = izena;
        bizitza = new Thread(() =>
        {
            while (true)
            {

            }
        });
    }

    void jan(Aulki aulkia)
    {
        Thread.Sleep(1000);
        aulkia.ipotxa = null;
    }
}

class Aulki
{
    public Ipotx ipotxa { get; set; }
    private Object okupatuta;
}