public abstract class Item {
    protected bool stackable;
    protected float weight;

    public bool Stackable
    {
        get
        {
            return stackable;
        }
    }

    public float Weight
    {
        get
        {
            return weight;
        }
    }
}
