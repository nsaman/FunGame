using System.Collections.Generic;

public abstract class Item
{
    protected string preFab;
    protected bool stackable;
    protected string tag;
    protected float weight;
    protected bool craftable;
    protected Dictionary<string, uint> craftCost;

    public string PreFab
    {
        get
        {
            return preFab;
        }
    }

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

    public bool Craftable
    {
        get
        {
            return craftable;
        }
    }

    public Dictionary<string, uint> CraftCost
    {
        get
        {
            return new Dictionary<string, uint>(craftCost);
        }
    }
}
