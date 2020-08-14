using System;

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
    public double NextNormalDistribution(double mean, double stdDev)
    { 
        double u1 = 1.0 - NextDouble(); 
        double u2 = 1.0 - NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); 
        return mean + stdDev * randStdNormal;
    }
}
