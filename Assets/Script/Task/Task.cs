using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Task : MonoBehaviour
{
    public const int MIN_TARGET_WEIGHT = 2;

    protected bool completingTask;
    protected TaskHandler taskHandler;
    protected EquipInventory inventory;
    protected Remembers remembers;
    protected SkillsController skills;

    public virtual void Start()
    {
        completingTask = false;
        taskHandler = GetComponent<TaskHandler>();
        inventory = GetComponent<EquipInventory>();
        remembers = GetComponent<Remembers>();
        skills = GetComponent<SkillsController>();
    }

    protected virtual void Update()
    {
    }

    public int maxWeight()
    {
        return Math.Max(MIN_TARGET_WEIGHT, skills.getLevel(Skills.Strength));
    }

    public abstract void completeTask();

    protected void dontMove()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
    }
    protected void move()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
    }

    protected MemoryEntry findClosestByTag(string tag)
    {
        return findClosestByTags(new string[] { tag }, transform.position);
    }

    protected MemoryEntry findClosestByTags(ICollection<string> tags)
    {
        return findClosestByTags(tags, transform.position);
    }

    private MemoryEntry findClosestByTags(ICollection<string> tags, Vector3 position)
    {

        HashSet<MemoryEntry> memories = new HashSet<MemoryEntry>();

        foreach (string tag in tags)
        {
            memories.UnionWith(remembers.FindGameMemoriesWithTag(tag));
        }

        return findClosest(memories, position);
    }

    public MemoryEntry findClosest(ICollection<MemoryEntry> memories, Vector3 position)
    {
        MemoryEntry returnMemory = null;
        float distanceToClosest = float.MaxValue;

        foreach (MemoryEntry memory in memories)
        {
            float distanceToNew = Vector3.Distance(memory.Position, position);
            if (distanceToNew < distanceToClosest)
            {
                returnMemory = memory;
                distanceToClosest = distanceToNew;
            }
        }

        return returnMemory;
    }

    public MemoryEntry findClosestTeamOrNeutralMemoriesWithTag(string tag)
    {
        return findClosest(getTeamOrNeutralMemoriesWithTags(new string[] { tag }), transform.position);
    }

    public MemoryEntry findClosestTeamOrNeutralMemoriesWithTags(string[] tags)
    {
        return findClosest(getTeamOrNeutralMemoriesWithTags(tags), transform.position);
    }

    public HashSet<MemoryEntry> getTeamOrNeutralMemoriesWithTags(string[] tags)
    {
        HashSet<MemoryEntry> returnMemories = new HashSet<MemoryEntry>();

        foreach (string tag in tags)
            returnMemories.UnionWith(getTeamOrNeutralMemoriesWithTag(tag));

        return returnMemories;
    }

    public HashSet<MemoryEntry> getTeamOrNeutralMemoriesWithTag(string tag)
    {
        HashSet<MemoryEntry> returnMemories = new HashSet<MemoryEntry>();

        ICollection<MemoryEntry> memories = remembers.FindGameMemoriesWithTag(tag);
        
        foreach(MemoryEntry memory in memories)
        {
            if (memory.Team == null)
                returnMemories.Add(memory);
            else if (memory.Team == gameObject.transform.root.GetComponent<TeamPointer>().TeamController)
                returnMemories.Add(memory);
        }

        return returnMemories;
    }

    protected void transferBestForTask(Inventory sendingInvetory, string targetTag, int maxWeight)
    {
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableHeads), maxWeight);
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableChests), maxWeight);
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableMainHands), maxWeight);
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableOffHands), maxWeight);
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableLegs), maxWeight);
        transferForTaskFromItems(sendingInvetory, targetTag, sendingInvetory.getEntriesOfTypes(Tags.EquipableFeets), maxWeight);
    }

    protected void transferForTaskFromItems(Inventory sendingInvetory, string targetTag, Dictionary<Item, uint> items, int maxWeight)
    {
        EquipableItem currentItem = null;
        foreach (KeyValuePair<Item, uint> entry in items)
        {
            EquipableItem equipableItem = (EquipableItem)entry.Key;
            if (equipableItem.getEffectivenessByTag(targetTag) > 1 && (currentItem == null || equipableItem.getEffectivenessByTag(targetTag) > currentItem.getEffectivenessByTag(targetTag)) && equipableItem.Weight + inventory.weight <= maxWeight)
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

    protected static EquipableItem bestItemCanCraftBySlots(Inventory inventory, string targetTag, ICollection<EquipableItem.EquipSlot> slots, float maxWeight)
    {
        EquipableItem item = null;
        foreach (EquipableItem.EquipSlot slot in slots)
        {
            EquipableItem currentItem = bestItemCanCraftBySlot(inventory, targetTag, slot, maxWeight);
            if (currentItem != null)
            {
                if (item == null || currentItem.getEffectivenessByTag(targetTag) > item.getEffectivenessByTag(targetTag))
                    item = currentItem;
            }
        }

        return item;
    }

    protected static EquipableItem bestItemCanCraftBySlot(Inventory inventory, string targetTag, EquipableItem.EquipSlot slot, float maxWeight)
    {
        EquipableItem item = null;
        foreach(EquipableItem currentItem in EquipableItem.getItemsByEquipSlot(slot))
        {
            if(currentItem.Weight <= maxWeight &&
                inventory.canBuild(currentItem) && 
                ((item == null && currentItem.getEffectivenessByTag(targetTag) > 0) || 
                (item != null && currentItem.getEffectivenessByTag(targetTag) > item.getEffectivenessByTag(targetTag))))
                item = currentItem;
        }

        return item;
    }
}
