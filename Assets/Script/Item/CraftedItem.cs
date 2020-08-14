public abstract class CraftedItem : Item
{
    protected float quality;
    protected bool crafted;

    public abstract CraftedItem craft(float quality);
}
