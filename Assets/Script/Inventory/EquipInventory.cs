using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EquipInventory : Inventory
{
    EquipableItem head;
    GameObject headGameObject;
    EquipableItem chest;
    GameObject chestGameObject;
    EquipableItem mainHand;
    GameObject mainHandGameObject;
    EquipableItem offHand;
    GameObject offHandGameObject;
    EquipableItem legs;
    GameObject legsGameObject;
    EquipableItem feet;
    GameObject feetGameObject;
    Observes observes;
    protected new void Start()
    {
        base.Start();

        observes = transform.root.GetComponent<Observes>();
    }

    protected new void Update()
    {
        base.Update();

        /*if (observes != null && headGameObject != null)
            headGameObject.transform.localRotation = Quaternion.AngleAxis(observes.LookAngle, Vector3.up);*/
    }

    protected new void Awake()
    {
        base.Awake();
    }

    public override bool remove(string tag, uint amount)
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
            {
                inventory[tag].Remove(inventoryEntry.Key);
                if (inventoryEntry.Key == head)
                    unEquipHead();
                if (inventoryEntry.Key == chest)
                    unEquipChest();
                if (inventoryEntry.Key == mainHand)
                    unEquipMainHand();
                if (inventoryEntry.Key == offHand)
                    unEquipOffHand();
                if (inventoryEntry.Key == legs)
                    unEquipLegs();
                if (inventoryEntry.Key == feet)
                    unEquipFeet();
            }
            leftToRemove -= realRemove;
        }

        setTotalWeight();

        return leftToRemove == 0;
    }

    public override bool remove(Item item, uint amount)
    {
        if (!containsItem(item))
            return false;

        uint leftToRemove = amount;
        Dictionary<Item, uint> copy = new Dictionary<Item, uint>(inventory[tag]);
        foreach (KeyValuePair<Item, uint> inventoryEntry in copy)
        {
            uint realRemove = System.Math.Min(inventoryEntry.Value, leftToRemove);
            inventory[item.Tag][inventoryEntry.Key] = inventoryEntry.Value - realRemove;
            if (inventory[item.Tag][inventoryEntry.Key] == 0)
            {
                inventory[item.Tag].Remove(inventoryEntry.Key);
                if (inventoryEntry.Key == head)
                    unEquipHead();
                if (inventoryEntry.Key == chest)
                    unEquipChest();
                if (inventoryEntry.Key == mainHand)
                    unEquipMainHand();
                if (inventoryEntry.Key == offHand)
                    unEquipOffHand();
                if (inventoryEntry.Key == legs)
                    unEquipLegs();
                if (inventoryEntry.Key == feet)
                    unEquipFeet();
            }
            leftToRemove -= realRemove;
        }

        setTotalWeight();

        return leftToRemove == 0;
    }

    public void equip(EquipableItem item)
    {
        if (item.EquippableSlot == EquipableItem.EquipSlot.Head)
            equipHead(item);
        else if (item.EquippableSlot == EquipableItem.EquipSlot.Chest)
            equipChest(item);
        else if (item.EquippableSlot == EquipableItem.EquipSlot.MainHand)
            equipMainHand(item);
        else if (item.EquippableSlot == EquipableItem.EquipSlot.OffHand)
            equipOffHand(item);
        else if (item.EquippableSlot == EquipableItem.EquipSlot.Legs)
            equipLegs(item);
        else if (item.EquippableSlot == EquipableItem.EquipSlot.Feet)
            equipFeet(item);
    }

    public void equipHead(EquipableItem item)
    {
        if (item.EquippableSlot != EquipableItem.EquipSlot.Head)
            return;

        unEquipHead();

        if (containsItem(item))
        {
            head = item;
            headGameObject = (GameObject)Object.Instantiate(Resources.Load(item.PreFab));
            headGameObject.transform.parent = gameObject.transform.root;
            headGameObject.transform.localPosition = Vector3.zero;
            headGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void unEquipHead()
    {
        head = null;
        Destroy(headGameObject);
        headGameObject = null;
    }

    public void equipChest(EquipableItem item)
    {
        if (item.EquippableSlot != EquipableItem.EquipSlot.Chest)
            return;

        unEquipChest();

        if (containsItem(item))
        {
            chest = item;
            chestGameObject = (GameObject)Object.Instantiate(Resources.Load(item.PreFab));
            chestGameObject.transform.parent = gameObject.transform.root;
            chestGameObject.transform.localPosition = Vector3.zero;
            chestGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void unEquipChest()
    {
        chest = null;
        Destroy(chestGameObject);
        chestGameObject = null;
    }

    public void equipMainHand(EquipableItem item)
    {
        if (item.EquippableSlot != EquipableItem.EquipSlot.MainHand)
            return;

        unEquipMainHand();

        if (containsItem(item))
        {
            mainHand = item;
            mainHandGameObject = (GameObject)Object.Instantiate(Resources.Load(item.PreFab));
            mainHandGameObject.transform.parent = gameObject.transform.root;
            mainHandGameObject.transform.localPosition = Vector3.zero;
            mainHandGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void unEquipMainHand()
    {
        mainHand = null;
        Destroy(mainHandGameObject);
        mainHandGameObject = null;
    }

    public void equipOffHand(EquipableItem item)
    {
        if (item.EquippableSlot != EquipableItem.EquipSlot.OffHand)
            return;

        unEquipOffHand();

        if (containsItem(item))
        {
            offHand = item;
            offHandGameObject = (GameObject)Object.Instantiate(Resources.Load(item.PreFab));
            offHandGameObject.transform.parent = gameObject.transform.root;
            offHandGameObject.transform.localPosition = Vector3.zero;
            offHandGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void unEquipOffHand()
    {
        offHand = null;
        Destroy(offHandGameObject);
        offHandGameObject = null;
    }

    public void equipLegs(EquipableItem item)
    {
        if (item.EquippableSlot != EquipableItem.EquipSlot.Legs)
            return;

        unEquipLegs();

        if (containsItem(item))
        {
            legs = item;
            legsGameObject = (GameObject)Object.Instantiate(Resources.Load(item.PreFab));
            legsGameObject.transform.parent = gameObject.transform.root;
            legsGameObject.transform.localPosition = Vector3.zero;
            legsGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void unEquipLegs()
    {
        legs = null;
        Destroy(legsGameObject);
        legsGameObject = null;
    }

    public void equipFeet(EquipableItem item)
    {
        if (item.EquippableSlot != EquipableItem.EquipSlot.Feet)
            return;

        unEquipFeet();

        if (containsItem(item))
        {
            feet = item;
            feetGameObject = (GameObject)Object.Instantiate(Resources.Load(item.PreFab));
            feetGameObject.transform.parent = gameObject.transform.root;
            feetGameObject.transform.localPosition = Vector3.zero;
            feetGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void unEquipFeet()
    {
        feet = null;
        Destroy(feetGameObject);
        feetGameObject = null;
    }

    public HashSet<EquipableItem.EquipSlot> getEmptySlots()
    {
        HashSet<EquipableItem.EquipSlot> returnSet = new HashSet<EquipableItem.EquipSlot>();
        if (head == null)
            returnSet.Add(EquipableItem.EquipSlot.Head);
        if (chest == null)
            returnSet.Add(EquipableItem.EquipSlot.Chest);
        if (mainHand == null)
            returnSet.Add(EquipableItem.EquipSlot.MainHand);
        if (offHand == null)
            returnSet.Add(EquipableItem.EquipSlot.OffHand);
        if (legs == null)
            returnSet.Add(EquipableItem.EquipSlot.Legs);
        if (feet == null)
            returnSet.Add(EquipableItem.EquipSlot.Head);

        return returnSet;
    }

    public EquipableItem Head
    {
        get
        {
            return head;
        }
    }

    public EquipableItem Chest
    {
        get
        {
            return chest;
        }
    }

    public EquipableItem MainHand
    {
        get
        {
            return mainHand;
        }
    }

    public EquipableItem OffHand
    {
        get
        {
            return offHand;
        }
    }

    public EquipableItem Legs
    {
        get
        {
            return legs;
        }
    }

    public EquipableItem Feet
    {
        get
        {
            return feet;
        }
    }
}