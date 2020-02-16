using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour {
    
    public Dictionary<string,List<InventoryEntry>> inventory;

    public float weight;

	// Use this for initialization
	void Awake() {
		inventory = new Dictionary<string, List<InventoryEntry>>();
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void setTotalWeight()
    {
        float weight = 0;
        foreach (KeyValuePair<string, List<InventoryEntry>> keyValuePair in inventory)
        {
            foreach (InventoryEntry inventoryEntry in keyValuePair.Value)
                weight += inventoryEntry.item.Weight * inventoryEntry.count;
        }

        this.weight = weight;
    }

    public List<InventoryEntry> getEntriesOfTypes(ICollection<string> tags)
    {
        List<InventoryEntry> returnList = new List<InventoryEntry>();

        foreach (string tag in tags)
        {
            if (inventory.ContainsKey(tag))
                returnList.AddRange(inventory[tag]);
        }

        return returnList;
    }

    public uint getTotalCountByTypes(ICollection<string> types)
    {
        uint count = 0;
        foreach (string type in types)
            count += getTotalCountByType(type);

        return count;
    }

    public uint getTotalCountByType(string type)
    {
        if (!inventory.ContainsKey(type))
            return 0;

        uint count = 0;
        foreach(InventoryEntry inventoryEntry in inventory[type])
        {
              count += inventoryEntry.count;
        }
        return count;
    }

    public InventoryEntry findFirstEntryOfTypes(ICollection<string> keys)
    {
        foreach (string key in keys)
        {
            InventoryEntry entry = findFirstEntry(key);
            if (entry != null)
                return entry;
        }

        return null;
    }

    public InventoryEntry findFirstEntry(string key)
    {
        return findFirstEntry(this, key);
    }

    public static InventoryEntry findFirstEntry(Inventory inventory, string key)
    {
        if (inventory.inventory.ContainsKey(key) && inventory.inventory[key].Count > 0)
            return inventory.inventory[key][0];
        else
            return null;
    }

    public bool remove(string tag, uint amount)
    {
        if (!inventory.ContainsKey(tag))
            return false;

        uint leftToRemove = amount;
        foreach (InventoryEntry inventoryEntry in inventory[tag])
        {
            uint realRemove = System.Math.Min(inventoryEntry.count, leftToRemove);
            inventoryEntry.count -= realRemove;
            leftToRemove -= realRemove;
        }

        setTotalWeight();

        return leftToRemove == 0;
    }

    public static void transferAllOfTypes(Inventory sendingInventory, Inventory receivingInventory, ICollection<string> types)
    {
        foreach (string type in types)
            transferAllOfType(sendingInventory, receivingInventory, type);
    }

    public static void transferAllOfType(Inventory sendingInventory, Inventory receivingInventory, string type)
    {
        transferType(sendingInventory, receivingInventory, type, sendingInventory.getTotalCountByType(type));
    }

    public static void transferTypes(Inventory sendingInventory, Inventory receivingInventory, ICollection<string> types, uint count)
    {
        uint leftToTransfer = count;

        foreach (string type in types)
            leftToTransfer -= transferType(sendingInventory, receivingInventory, type, leftToTransfer);
    }

    public static uint transferType(Inventory sendingInventory, Inventory receivingInventory, string type, uint count)
    {
        uint leftToTransfer = count;
        List<InventoryEntry> sendingEntries = sendingInventory.getOrCreate(type);

        List<InventoryEntry> checkRemove = new List<InventoryEntry>();

        // todo add logic here that would prevent overfilling max inventory sizes, max slot sizes
        foreach (InventoryEntry sendingEntry in sendingEntries)
        {
            uint realTransfer = System.Math.Min(leftToTransfer, sendingEntry.count);
            sendingEntry.count -= realTransfer;
            receivingInventory.getOrCreateEntry(type, sendingEntry.item).count += realTransfer;
            leftToTransfer -= realTransfer;

            checkRemove.Add(sendingEntry);
        }

        foreach (InventoryEntry inventoryEntry in checkRemove)
        {
            if (inventoryEntry.count == 0)
                sendingInventory.inventory[type].Remove(inventoryEntry);
        }

        sendingInventory.setTotalWeight();
        receivingInventory.setTotalWeight();

        return count - leftToTransfer;
    }

    public List<InventoryEntry> getOrCreate(string type)
    {
        if (inventory.ContainsKey(type))
            return inventory[type];
        else
        {
            inventory.Add(type, new List<InventoryEntry>());
            return inventory[type];
        }
    }

    public InventoryEntry getOrCreateEntry(string type, Item item)
    {
        if (!inventory.ContainsKey(type))
            inventory.Add(type, new List<InventoryEntry>());

        if (inventory[type].Count == 0)
            inventory[type].Add(new InventoryEntry(item, 0));

        return inventory[type][0];
    }

    public bool containsAny(ICollection<string> types)
    {
        return Inventory.containsAny(this, types);
    }

    public static bool containsAny(Inventory inventory, ICollection<string> types)
    {
        foreach (string type in types)
        {
            if (inventory.getTotalCountByType(type) > 0)
                return true;
        }

        return false;
    }
}
