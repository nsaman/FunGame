using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour {
    
    public Dictionary<string,Dictionary<Item,uint>> inventory;

    public float weight;

	// Use this for initialization
	void Awake() {
		inventory = new Dictionary<string, Dictionary<Item,uint>>();
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void setTotalWeight()
    {
        float weight = 0;
        foreach (KeyValuePair<string, Dictionary<Item, uint>> keyValuePair in inventory)
        {
            foreach (KeyValuePair<Item, uint> itemsKeyValuePair in keyValuePair.Value)
                weight += itemsKeyValuePair.Key.Weight * itemsKeyValuePair.Value;
        }

        this.weight = weight;
    }

    public Dictionary<Item, uint> getEntriesOfTypes(ICollection<string> tags)
    {
        Dictionary<Item, uint> returnDictionary = new Dictionary<Item, uint>();

        foreach (string tag in tags)
        {
            if (inventory.ContainsKey(tag))
                inventory[tag].ToList().ForEach(x => returnDictionary.Add(x.Key, x.Value));
        }

        return returnDictionary;
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
        foreach(KeyValuePair<Item, uint> inventoryEntry in inventory[type])
        {
              count += inventoryEntry.Value;
        }
        return count;
    }

    public KeyValuePair<Item, uint> findFirstEntryOfTypes(ICollection<string> keys)
    {
        foreach (string key in keys)
        {
            KeyValuePair<Item, uint> entry = findFirstEntry(key);
            if (entry.Key != null)
                return entry;
        }

        return default(KeyValuePair<Item, uint>);
    }

    public KeyValuePair<Item, uint> findFirstEntry(string key)
    {
        return findFirstEntry(this, key);
    }

    public static KeyValuePair<Item, uint> findFirstEntry(Inventory inventory, string key)
    {
        if (inventory.inventory.ContainsKey(key) && inventory.inventory[key].Count > 0)
            return inventory.inventory[key].ToList()[0];
        else
            return default(KeyValuePair<Item, uint>);
    }

    public bool remove(string tag, uint amount)
    {
        if (!inventory.ContainsKey(tag))
            return false;

        uint leftToRemove = amount;
        HashSet<Item> toRemove = new HashSet<Item>();
        Dictionary<Item, uint> copy = new Dictionary<Item, uint>(inventory[tag]);
        foreach (KeyValuePair<Item, uint> inventoryEntry in copy)
        {
            uint realRemove = System.Math.Min(inventoryEntry.Value, leftToRemove);
            inventory[tag][inventoryEntry.Key] = inventoryEntry.Value - realRemove;
            if (inventory[tag][inventoryEntry.Key] == 0)
                toRemove.Add(inventoryEntry.Key);
            leftToRemove -= realRemove;
        }

        foreach(Item item in toRemove)
        {
            inventory[tag].Remove(item);
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
        Dictionary<Item, uint> sendingEntries = sendingInventory.getOrCreate(type);

        HashSet<Item> toRemove = new HashSet<Item>();

        // todo add logic here that would prevent overfilling max inventory sizes, max slot sizes
        Dictionary < Item, uint> copy = new Dictionary<Item, uint>(sendingEntries);
        foreach (KeyValuePair<Item, uint> sendingEntry in copy)
        {
            uint realTransfer = System.Math.Min(leftToTransfer, sendingEntry.Value);
            sendingInventory.remove(type, realTransfer);

            KeyValuePair<Item, uint> receivingEntry = receivingInventory.getOrCreateEntry(type, sendingEntry.Key);
            receivingInventory.inventory[type][receivingEntry.Key] += realTransfer;
            leftToTransfer -= realTransfer;
        }

        receivingInventory.setTotalWeight();

        return count - leftToTransfer;
    }

    public Dictionary<Item, uint> getOrCreate(string type)
    {
        if (inventory.ContainsKey(type))
            return inventory[type];
        else
        {
            inventory.Add(type, new Dictionary<Item, uint>());
            return inventory[type];
        }
    }

    public KeyValuePair<Item, uint> getOrCreateEntry(string type, Item item)
    {
        if (!inventory.ContainsKey(type))
            inventory.Add(type, new Dictionary<Item, uint>());

        if (inventory[type].Count == 0)
            inventory[type].Add(item, 0);

        return inventory[type].ToList()[0];
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
