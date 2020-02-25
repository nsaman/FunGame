using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour {
    
    public Dictionary<string,Dictionary<Item,uint>> inventory;

    public float weight;

	// Use this for initialization
	protected void Awake() {
		inventory = new Dictionary<string, Dictionary<Item,uint>>();
	}

    // Update is called once per frame
    protected void Update () {
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

    public float getTotalWeightByTypes(ICollection<string> types)
    {
        float totalWeight = 0;
        foreach (string type in types)
            totalWeight += getTotalWeightByType(type);

        return totalWeight;
    }

    public float getTotalWeightByType(string type)
    {
        if (!inventory.ContainsKey(type))
            return 0;

        float totalWeight = 0;
        foreach (KeyValuePair<Item, uint> inventoryEntry in inventory[type])
        {
            totalWeight += inventoryEntry.Value * inventoryEntry.Key.Weight;
        }
        return totalWeight;
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

    public virtual void add(Item item, uint count)
    {
        createEntry(item);

        inventory[item.Tag][item] += count;

        setTotalWeight();
    }

    public virtual bool remove(string tag, uint amount)
    {
        if (!inventory.ContainsKey(tag))
            return false;

        uint leftToRemove = amount;
        Dictionary<Item, uint> copy = new Dictionary<Item, uint>(inventory[tag]);
        foreach (KeyValuePair<Item, uint> inventoryEntry in copy)
        {
            uint realRemove = System.Math.Min(inventoryEntry.Value, leftToRemove);
            inventory[tag][inventoryEntry.Key] = inventoryEntry.Value - realRemove;
            if (inventory[tag][inventoryEntry.Key] == 0)
                inventory[tag].Remove(inventoryEntry.Key);
            leftToRemove -= realRemove;
        }

        setTotalWeight();

        return leftToRemove == 0;
    }

    public virtual bool remove(Item item, uint amount)
    {
        if (!containsItem(item))
            return false;

        uint leftToRemove = amount;
        Dictionary<Item, uint> copy = new Dictionary<Item, uint>(inventory[item.Tag]);
        foreach (KeyValuePair<Item, uint> inventoryEntry in copy)
        {
            uint realRemove = System.Math.Min(inventoryEntry.Value, leftToRemove);
            inventory[item.Tag][inventoryEntry.Key] = inventoryEntry.Value - realRemove;
            if (inventory[item.Tag][inventoryEntry.Key] == 0)
                inventory[item.Tag].Remove(inventoryEntry.Key);
            leftToRemove -= realRemove;
        }

        setTotalWeight();

        return leftToRemove == 0;
    }

    public static void transferAll(Inventory sendingInventory, Inventory receivingInventory)
    {
        foreach (string type in sendingInventory.inventory.Keys)
            transferAllOfType(sendingInventory, receivingInventory, type);
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

        // todo add logic here that would prevent overfilling max inventory sizes, max slot sizes
        Dictionary < Item, uint> copy = new Dictionary<Item, uint>(sendingEntries);
        foreach (KeyValuePair<Item, uint> sendingEntry in copy)
        {
            uint realTransfer = System.Math.Min(leftToTransfer, sendingEntry.Value);
            sendingInventory.remove(type, realTransfer);

            receivingInventory.add(sendingEntry.Key, realTransfer);

            leftToTransfer -= realTransfer;
        }

        receivingInventory.setTotalWeight();

        return count - leftToTransfer;
    }

    public bool receive(Inventory sendingInventory, Item item)
    {
        if (!sendingInventory.remove(item, 1))
            return false;

        add(item, 1);

        return true;
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

    public void createEntry(Item item)
    {
        Dictionary<Item, uint> items = getOrCreate(item.Tag);

        if (!items.ContainsKey(item))
            items.Add(item, 0);
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

    public bool containsItem(Item item)
    {
        return containsItem(this, item);
    }

    public static bool containsItem(Inventory inventory, Item item)
    {
        return inventory.inventory.ContainsKey(item.Tag) && inventory.inventory[item.Tag].ContainsKey(item) && inventory.inventory[item.Tag][item] > 0;
    }

    public bool canBuild(Item item)
    {
        foreach (KeyValuePair<string, uint> cost in item.CraftCost)
        {
            if (getTotalCountByType(cost.Key) < cost.Value)
                return false;
        }

        return true;
    }
}
