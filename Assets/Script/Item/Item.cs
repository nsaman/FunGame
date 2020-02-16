public abstract class Item {
    protected bool stackable;
    protected string tag;
    protected float weight;

    public bool Stackable
    {
        get
        {
            return stackable;
        }
    }

    public string Tag
    {
        get
        {
            return tag;
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
