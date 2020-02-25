using UnityEngine;
using System.Collections.Generic;

public abstract class Task : MonoBehaviour
{

    protected bool completingTask;
    protected TaskHandler taskHandler;
    public const uint MAX_WEIGHT = 3;
    protected EquipInventory inventory;

    public virtual void Start()
    {
        completingTask = false;
        taskHandler = GetComponent<TaskHandler>();
        inventory = GetComponent<EquipInventory>();
    }

    protected virtual void Update()
    {
    }

    public abstract void completeTask();

    protected void dontMove()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }
    protected void move()
    {
        GetComponent<NavMeshAgent>().enabled = true;
    }

    protected Transform findClosestByTag(string tag)
    {
        return findClosestByTags(new string[] { tag }, transform);
    }

    protected Transform findClosestByTags(string[] tags)
    {
        return findClosestByTags(tags, transform);
    }

    public static Transform findClosestByTags(string[] tags, Transform startingTransform)
    {

        HashSet<GameObject> tagObjects = new HashSet<GameObject>();

        foreach (string tag in tags)
        {
            tagObjects.UnionWith(GameObject.FindGameObjectsWithTag(tag));
        }

        return findClosest(tagObjects, startingTransform);
    }

    public Transform findClosest(ICollection<GameObject> tagObjects)
    {
        return Task.findClosest(tagObjects, transform);
    }

    public static Transform findClosest(ICollection<GameObject> tagObjects, Transform startingTransform)
    {
        Transform closest = null;
        float distanceToClosest = float.MaxValue;

        foreach (GameObject gameobject in tagObjects)
        {
            Transform newTransform = gameobject.GetComponent<Transform>();
            float distanceToNew = Vector3.Distance(newTransform.position, startingTransform.position);
            if (distanceToNew < distanceToClosest)
            {
                closest = newTransform;
                distanceToClosest = distanceToNew;
            }
        }

        return closest;
    }

    public Transform findClosestTeamOrNeutralObjectsWithTag(string tag)
    {
        return (findClosest(getTeamOrNeutralObjectsWithTags(new string[] { tag }), transform));
    }

    public Transform findClosestTeamOrNeutralObjectsWithTags(string[] tags)
    {
        return (findClosest(getTeamOrNeutralObjectsWithTags(tags), transform));
    }

    public HashSet<GameObject> getTeamOrNeutralObjectsWithTags(string[] tags)
    {
        HashSet<GameObject> returnObjects = new HashSet<GameObject>();

        foreach (string tag in tags)
            returnObjects.UnionWith(getTeamOrNeutralObjectsWithTag(tag));

        return returnObjects;
    }

    public HashSet<GameObject> getTeamOrNeutralObjectsWithTag(string tag)
    {
        HashSet<GameObject> returnObjects = new HashSet<GameObject>();

        GameObject[] tagObjects = GameObject.FindGameObjectsWithTag(tag);
        
        foreach(GameObject taggedObject in tagObjects)
        {
            if (taggedObject.transform.root.GetComponent<TeamPointer>() == null)
                returnObjects.Add(taggedObject);
            else if (taggedObject.transform.root.GetComponent<TeamPointer>().TeamController == gameObject.transform.root.GetComponent<TeamPointer>().TeamController)
                returnObjects.Add(taggedObject);
        }

        return returnObjects;
    }

    protected void transferBestForTask(Inventory sendingInvetory, string targetTag)
    {
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableHeads));
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableChests));
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableMainHands));
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableOffHands));
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableLegs));
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableFeets));
    }

    protected void transferForTaskFromItems(Inventory sendingInvetory, string targetTag, Dictionary<Item, uint> items)
    {
        EquipableItem currentItem = null;
        foreach (KeyValuePair<Item, uint> entry in items)
        {
            EquipableItem equipableItem = (EquipableItem)entry.Key;
            if (equipableItem.getEffectivenessByTag(targetTag) > 1 && (currentItem == null || equipableItem.getEffectivenessByTag(targetTag) > currentItem.getEffectivenessByTag(targetTag)))
                currentItem = equipableItem;
        }
        if (currentItem != null)
        {
            inventory.receive(sendingInvetory, currentItem);
        }
    }

    protected void equipBestForTask(string targetTag)
    {
        equipForTaskFromItems(targetTag, inventory.getEntriesOfTypes(Tags.EquipableHeads));
        equipForTaskFromItems(targetTag, inventory.getEntriesOfTypes(Tags.EquipableChests));
        equipForTaskFromItems(targetTag, inventory.getEntriesOfTypes(Tags.EquipableMainHands));
        equipForTaskFromItems(targetTag, inventory.getEntriesOfTypes(Tags.EquipableOffHands));
        equipForTaskFromItems(targetTag, inventory.getEntriesOfTypes(Tags.EquipableLegs));
        equipForTaskFromItems(targetTag, inventory.getEntriesOfTypes(Tags.EquipableFeets));
    }

    protected void equipForTaskFromItems(string targetTag, Dictionary<Item, uint> items)
    {
        EquipableItem currentItem = null;
        foreach (KeyValuePair<Item, uint> entry in items)
        {
            EquipableItem equipableItem = (EquipableItem)entry.Key;
            if (equipableItem.getEffectivenessByTag(targetTag) > 1 && (currentItem == null || equipableItem.getEffectivenessByTag(targetTag) > currentItem.getEffectivenessByTag(targetTag)))
                currentItem = equipableItem;
        }
        if (currentItem != null)
        {
            inventory.equip(currentItem);
        }
    }

    protected float getInteractionEquipedMultiplier(string targetTag)
    {
        return (inventory.Head != null ? inventory.Head.getEffectivenessByTag(targetTag) : 0)
            + (inventory.Chest != null ? inventory.Chest.getEffectivenessByTag(targetTag) : 0)
            + (inventory.MainHand != null ? inventory.MainHand.getEffectivenessByTag(targetTag) : 0)
            + (inventory.OffHand != null ? inventory.OffHand.getEffectivenessByTag(targetTag) : 0)
            + (inventory.Legs != null ? inventory.Legs.getEffectivenessByTag(targetTag) : 0)
            + (inventory.Feet != null ? inventory.Feet.getEffectivenessByTag(targetTag) : 0);
    }

    protected static EquipableItem bestItemCanCraftBySlots(Inventory inventory, string targetTag, ICollection<EquipableItem.EquipSlot> slots)
    {
        EquipableItem item = null;
        foreach (EquipableItem.EquipSlot slot in slots)
        {
            EquipableItem currentItem = bestItemCanCraftBySlot(inventory, targetTag, slot);
            if (currentItem != null)
            {
                if (item == null || currentItem.getEffectivenessByTag(targetTag) > item.getEffectivenessByTag(targetTag))
                    item = currentItem;
            }
        }

        return item;
    }

    protected static EquipableItem bestItemCanCraftBySlot(Inventory inventory, string targetTag, EquipableItem.EquipSlot slot)
    {
        EquipableItem item = null;
        foreach(EquipableItem currentItem in EquipableItem.getItemsByEquipSlot(slot))
        {
            if(inventory.canBuild(currentItem) && 
                ((item == null && currentItem.getEffectivenessByTag(targetTag) > 0) || 
                (item != null && currentItem.getEffectivenessByTag(targetTag) > item.getEffectivenessByTag(targetTag))))
                item = currentItem;
        }

        return item;
    }
}
