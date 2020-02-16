public class InventoryEntry {
    
    public Item item { get; set; }
    public uint count { get; set; }

    public InventoryEntry(Item item, uint count)
    {
        this.item = item;
        this.count = count;
    }

}
