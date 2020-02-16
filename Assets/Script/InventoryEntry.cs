public class InventoryEntry {

    public InventoryEntry(string item, uint count)
    {
        this.item = item;
        this.count = count;
    }

    // todo update to object once we have item classes
    public string item { get; set; }
	public uint count { get; set; }
}
