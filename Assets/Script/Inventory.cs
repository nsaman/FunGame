using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour {

    // todo update tcco object once we have item classes
    public List<InventoryEntry> inventory;

    public uint weight;

	// Use this for initialization
	void Awake() {
		inventory = new List<InventoryEntry>();
	}
	
	// Update is called once per frame
	void Update () {
        weight = getTotalWeight();
    }

    public uint getTotalWeight()
    {
        long weight = inventory.Sum(item => item.count);
        return System.Convert.ToUInt32(weight);
    }

    public uint getTotalCountByType(string type)
    {
        uint count = 0;
        foreach(InventoryEntry inventoryEntry in inventory)
        {
            if (inventoryEntry.item.Equals(type))
            {
                count += inventoryEntry.count;
            }
        }
        return count;
    }

    public List<InventoryEntry> getInventoryEntriesMatchingType(string type)
    {
        List<InventoryEntry> inventoryEntries = new List<InventoryEntry>();
        foreach (InventoryEntry inventoryEntry in inventory)
        {
            if (inventoryEntry.item.Equals(type))
            {
                inventoryEntries.Add(inventoryEntry);
            }
        }

        if (inventoryEntries.Count == 0)
        {
            InventoryEntry newEntry = new InventoryEntry(type, 0);
            inventory.Add(newEntry);
            inventoryEntries.Add(newEntry);
        }

        return inventoryEntries;
    }

    public List<InventoryEntry> findEntriesOfTypes(ICollection<string> keys)
    {
        List<InventoryEntry> returnEntries = new List<InventoryEntry>();

        foreach (string key in keys)
        {
           foreach (InventoryEntry inventoryEntry in inventory)
            {
                if (inventoryEntry.item.Equals(key))
                    returnEntries.Add(inventoryEntry);
            }
        }

        return returnEntries;
    }

    public InventoryEntry findFirstEntryOfTypes(ICollection<string> keys)
    {
        foreach (string key in keys)
        {
            InventoryEntry entry = findFirstEntry(this.inventory, key);
            if (entry != null)
                return entry;
        }

        return null;
    }

    public InventoryEntry findFirstEntry(string key)
    {
        return findFirstEntry(this.inventory, key);
    }

    public static InventoryEntry findFirstEntry(ICollection<InventoryEntry> inventoryEntries, string key)
    {
        foreach (InventoryEntry ie in inventoryEntries)
        {
            if (ie.item.Equals(key))
            {
                return ie;
            }
        }

        return null;
    }

    public void remove(string tag, uint amount)
    {
        // todo fix rollover logic as needed
        foreach (InventoryEntry inventoryEntry in inventory)
        {
            if (inventoryEntry.item.Equals(tag))
            {
                inventoryEntry.count -= amount;
                return;
            }
        }
    }

    public static void transferUpToOfTypes(Inventory sendingInventory, Inventory receivingInventory, uint maxTransferCount, ICollection<string> types)
    {
        uint currentTransfered = 0; 

        foreach (string type in types)
        {
            // if there end up being multiple stacks, fix that here
            currentTransfered += Inventory.transferMultiple(sendingInventory.getInventoryEntriesMatchingType(type), receivingInventory.getInventoryEntriesMatchingType(type)[0], maxTransferCount - currentTransfered);
        }
    }

    public static uint transferMultiple(ICollection<InventoryEntry> sendingEntries, InventoryEntry receivingEntry, uint amount)
    {
        uint currentTransfered = 0;

        foreach(InventoryEntry sendingEntry in sendingEntries)
        {
            currentTransfered += transfer(sendingEntry, receivingEntry, amount - currentTransfered);
        }

        return currentTransfered;
    }

    public static uint transfer(InventoryEntry sendingEntry, InventoryEntry receivingEntry, uint amount)
    {
        uint realAmount = System.Math.Min(sendingEntry.count, amount);

        sendingEntry.count -= realAmount;
        receivingEntry.count += realAmount;

        return realAmount;
    }

    public static void transferAllOfTypes(Inventory sendingInventory, Inventory receivingInventory, ICollection<string> types)
    {
        foreach (string type in types)
            transferAllOfType(sendingInventory, receivingInventory, type);
    }

    public static void transferAllOfType(Inventory sendingInventory, Inventory receivingInventory, string type)
    {
        uint totalTransferred = sendingInventory.getTotalCountByType(type);
        if (totalTransferred > 0)
        {
            List<InventoryEntry> sendingEntries = sendingInventory.getInventoryEntriesMatchingType(type);
            List<InventoryEntry> receivingEntries = receivingInventory.getInventoryEntriesMatchingType(type);

            // todo add logic here that would prevent overfilling max inventory sizes, max slot sizes

            foreach (InventoryEntry sendingEntry in sendingEntries)
            {
                receivingEntries[0].count += totalTransferred;
                sendingInventory.inventory.Remove(sendingEntry);
            }
        }
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
