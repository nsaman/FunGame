public class RandomSingleton
{
    private static RandomSingleton instance = null;
    private static readonly object padlock = new object();
    private System.Random rnd;

    RandomSingleton()
    {
        rnd = new System.Random();
    }

    public static RandomSingleton Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new RandomSingleton();
                }
                return instance;
            }
        }
    }

    public int Next()
    {
        return rnd.Next();
    }
    public int Next(int maxValue)
    {
        return rnd.Next(maxValue);
    }
    public int Next(int minValue, int maxValue)
    {
        return rnd.Next(minValue, maxValue);
    }
    public void NextBytes(byte[] buffer)
    {
        rnd.NextBytes(buffer);
    }
    public double NextDouble()
    {
        return rnd.NextDouble();
    }
}
