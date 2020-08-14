using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Remembers : MonoBehaviour
{
    // we may need to review the datastructures here on performance
    private Dictionary<string, Dictionary<int, MemoryEntry>> memories;
    private SortedDictionary<long, Dictionary<int, MemoryEntry>> timeIndex;
    // at some point we should convert to using a spatial index
    private SortedDictionary<float, SortedDictionary<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>>> positionIndex;
    // exipiringQueue reffers to things that should be forgotten later, like a moving npc that is no longer in the same spot
    private SortedList<long, HashSet<MemoryEntry>> expiringQueue;
    private SortedList<long, HashSet<MemoryEntry>> forgettingQueue;
    private Dictionary<int, MemoryEntry> forgetting;
    private Target target;

    public SortedDictionary<long, Dictionary<int, MemoryEntry>> TimeIndex { get => timeIndex; }
    public SortedList<long, HashSet<MemoryEntry>> ForgettingQueue { get => forgettingQueue; }

    private void Awake()
    {
        memories = new Dictionary<string, Dictionary<int, MemoryEntry>>();
        timeIndex = new SortedDictionary<long, Dictionary<int, MemoryEntry>>();
        positionIndex = new SortedDictionary<float, SortedDictionary<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>>>();
        expiringQueue = new SortedList<long, HashSet<MemoryEntry>>();
        forgettingQueue = new SortedList<long, HashSet<MemoryEntry>>();
        forgetting = new Dictionary<int, MemoryEntry>();
        target = transform.root.gameObject.GetComponent<Target>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long currentTimeMinusTwentyMinutes = currentTime - 1200000;

        // remove old memories
        while (TimeIndex.Count > 0 && TimeIndex.First().Key < currentTimeMinusTwentyMinutes)
        {
            long currentKey = TimeIndex.First().Key;
            foreach (MemoryEntry memory in TimeIndex[currentKey].Values.ToList())
                DeleteFromMemories(memory, null);
        }

        // remove old forgettings
        while (ForgettingQueue.Count > 0 && ForgettingQueue.First().Key < currentTimeMinusTwentyMinutes)
        {
            long currentKey = ForgettingQueue.First().Key;
            foreach (MemoryEntry memory in ForgettingQueue[currentKey].ToList())
                DeleteFromForgets(memory);
        }

        // expire memories
        while (expiringQueue.Count > 0 && expiringQueue.First().Key < currentTime)
        {
            long currentKey = expiringQueue.First().Key;
            foreach (MemoryEntry memory in expiringQueue[currentKey])
                Remove(memory, false);
            expiringQueue.Remove(currentKey);
        }
    }

    public ICollection<MemoryEntry> FindGameMemoriesWithTag(string tag)
    {
        if (!memories.ContainsKey(tag))
            return new List<MemoryEntry>();
        return memories[tag].Values;
    }

    public bool hasMemoryOf(GameObject gameObject)
    {
        string objectTag = gameObject.transform.root.tag;
        return memories.ContainsKey(objectTag) && memories[objectTag].ContainsKey(gameObject.GetInstanceID());
    }

    public MemoryEntry FindMemoryByGameObject(GameObject gameObject)
    {
        string objectTag = gameObject.transform.root.tag;
        if (!memories.ContainsKey(objectTag) || !memories[objectTag].ContainsKey(gameObject.GetInstanceID()))
            return null;
        return memories[objectTag][gameObject.GetInstanceID()];
    }

    public void RememberAll(Remembers remembers)
    {
        foreach(Dictionary<int, MemoryEntry> memories in remembers.TimeIndex.Values)
            RememberAll(memories.Values);
    }

    public void RememberAll(ICollection<GameObject> gameObjects)
    {
        foreach(GameObject gameObject in gameObjects)
            Remember(gameObject);
    }
    public void RememberAll(ICollection<MemoryEntry> memories)
    {
        foreach (MemoryEntry memoryEntry in memories)
            Remember(memoryEntry);
    }

    public void Remember(MemoryEntry memoryEntry)
    {
        Put(memoryEntry);
    }

    public void Remember(GameObject gameObject)
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        GameObject rootObject = gameObject.transform.root.gameObject;
        TeamPointer teamPointer = rootObject.GetComponent<TeamPointer>();
        TeamController team = null;
        if (teamPointer != null)
            team = teamPointer.TeamController;
        bool dangerous = rootObject.CompareTag(Tags.Npc) && rootObject.GetComponent<EquipInventory>().containsAny(Tags.Weapons);

        Put(new MemoryEntry(
            rootObject.GetInstanceID(),
            rootObject.transform.position,
            Array.Exists(Tags.Moves, x => rootObject.CompareTag(x)),
            currentTime,
            rootObject.tag,
            team,
            dangerous
            ));
    }

    public void ForgetAll(Remembers remembers)
    {
        foreach (ICollection<MemoryEntry> memories in remembers.ForgettingQueue.Values)
            ForgetAll(memories);
    }

    public void ForgetAll(ICollection<MemoryEntry> memories)
    {
        foreach (MemoryEntry memory in memories)
            Forget(memory);
    }

    public void Forget(GameObject gameObject)
    {
        GameObject rootObject = gameObject.transform.root.gameObject;

        if(memories.ContainsKey(rootObject.tag) && memories[rootObject.tag].ContainsKey(rootObject.GetInstanceID()))
        {
            Forget(memories[rootObject.tag][rootObject.GetInstanceID()]);
        }
    }

    public void Forget(MemoryEntry memory)
    {
        if (memories.ContainsKey(memory.Tag) && memories[memory.Tag].ContainsKey(memory.InstanceID) && memories[memory.Tag][memory.InstanceID].TimeStamp <= memory.TimeStamp)
        {
            if (!memory.Moving)
                Remove(memories[memory.Tag][memory.InstanceID], true);
            else
            {
                long expireTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 30000;
                if (!expiringQueue.ContainsKey(expireTime))
                    expiringQueue.Add(expireTime, new HashSet<MemoryEntry>());
                expiringQueue[expireTime].Add(memories[memory.Tag][memory.InstanceID]);
            }
        }
    }

    private void Remove(MemoryEntry memory, bool newForgetTimeStamp)
    {
        if (memories.ContainsKey(memory.Tag) && memories[memory.Tag].ContainsKey(memory.InstanceID) && memories[memory.Tag][memory.InstanceID].TimeStamp <= memory.TimeStamp)
        {
            DeleteFromMemories(memory, null);

            MemoryEntry newTimeStampMemory = memory;
            if (newForgetTimeStamp)
            {
                newTimeStampMemory = new MemoryEntry(
                memory.InstanceID,
                memory.Position,
                memory.Moving,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                memory.Tag,
                memory.Team,
                memory.Dangerous);
            }

            forgetting[newTimeStampMemory.InstanceID] = newTimeStampMemory;

            if (!ForgettingQueue.ContainsKey(newTimeStampMemory.TimeStamp))
                ForgettingQueue.Add(newTimeStampMemory.TimeStamp, new HashSet<MemoryEntry>());
            ForgettingQueue[newTimeStampMemory.TimeStamp].Add(newTimeStampMemory);
        }
    }

    private void Put(MemoryEntry memoryEntry)
    {
        // do logic on an existing memory entry of the same instanceID
        if (memories.ContainsKey(memoryEntry.Tag) && memories[memoryEntry.Tag].ContainsKey(memoryEntry.InstanceID))
        {
            // make sure there isn't a newer memory of the gameObject
            if (memories[memoryEntry.Tag][memoryEntry.InstanceID].TimeStamp >= memoryEntry.TimeStamp)
                return;
            else
            {
                if (target != null && target.TargetMemory != null && target.TargetMemory.InstanceID == memoryEntry.InstanceID)
                {
                    target.TargetMemory = memoryEntry;
                }
                DeleteFromMemories(memories[memoryEntry.Tag][memoryEntry.InstanceID], memoryEntry);
            }
        }

        if (forgetting.ContainsKey(memoryEntry.InstanceID)) {
            if (forgetting[memoryEntry.InstanceID].TimeStamp > memoryEntry.TimeStamp)
                return;
            else
                DeleteFromForgets(forgetting[memoryEntry.InstanceID]);
        }

        if (!memories.ContainsKey(memoryEntry.Tag))
            memories.Add(memoryEntry.Tag, new Dictionary<int, MemoryEntry>());

        memories[memoryEntry.Tag][memoryEntry.InstanceID] = memoryEntry;


        // add to the time index
        if (!TimeIndex.ContainsKey(memoryEntry.TimeStamp))
            TimeIndex.Add(memoryEntry.TimeStamp, new Dictionary<int, MemoryEntry>());

        TimeIndex[memoryEntry.TimeStamp].Add(memoryEntry.InstanceID,memoryEntry);


        // add to the position index
        if (!positionIndex.ContainsKey(memoryEntry.Position.x))
            positionIndex.Add(memoryEntry.Position.x, new SortedDictionary<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>>());
        if (!positionIndex[memoryEntry.Position.x].ContainsKey(memoryEntry.Position.y))
            positionIndex[memoryEntry.Position.x].Add(memoryEntry.Position.y, new SortedDictionary<float, Dictionary<int, MemoryEntry>>());
        if (!positionIndex[memoryEntry.Position.x][memoryEntry.Position.y].ContainsKey(memoryEntry.Position.z))
            positionIndex[memoryEntry.Position.x][memoryEntry.Position.y].Add(memoryEntry.Position.z, new Dictionary<int, MemoryEntry>());

        positionIndex[memoryEntry.Position.x][memoryEntry.Position.y][memoryEntry.Position.z].Add(memoryEntry.InstanceID, memoryEntry);
    }

    private void DeleteFromMemories(MemoryEntry memoryEntry, MemoryEntry preserveIndexesForMemory)
    {
        if (memories.ContainsKey(memoryEntry.Tag) && memories[memoryEntry.Tag].ContainsKey(memoryEntry.InstanceID))
        {
            MemoryEntry localEntry = memories[memoryEntry.Tag][memoryEntry.InstanceID];

            positionIndex[localEntry.Position.x][localEntry.Position.y][localEntry.Position.z].Remove(localEntry.InstanceID);
            if (preserveIndexesForMemory == null || localEntry.Position.x != preserveIndexesForMemory.Position.x
                || localEntry.Position.y != preserveIndexesForMemory.Position.y || localEntry.Position.z != preserveIndexesForMemory.Position.z)
            {
                if (positionIndex[localEntry.Position.x][localEntry.Position.y][localEntry.Position.z].Count == 0)
                    positionIndex[localEntry.Position.x][localEntry.Position.y].Remove(localEntry.Position.z);
                if (positionIndex[localEntry.Position.x][localEntry.Position.y].Count == 0)
                    positionIndex[localEntry.Position.x].Remove(localEntry.Position.y);
                if (positionIndex[localEntry.Position.x].Count == 0)
                    positionIndex.Remove(localEntry.Position.x);
            }

            TimeIndex[localEntry.TimeStamp].Remove(localEntry.InstanceID);
            if (preserveIndexesForMemory == null || localEntry.TimeStamp != preserveIndexesForMemory.TimeStamp)
            {
                if (TimeIndex[localEntry.TimeStamp].Count == 0)
                    TimeIndex.Remove(localEntry.TimeStamp);
            }

            memories[localEntry.Tag].Remove(localEntry.InstanceID);
            if (preserveIndexesForMemory == null || !localEntry.Tag.Equals(preserveIndexesForMemory.Tag))
            {
                if (memories[localEntry.Tag].Count == 0)
                    memories.Remove(localEntry.Tag);
            }

            if (target != null && target.TargetMemory == localEntry)
            {
                target.TargetMemory = null;
            }
        }
    }

    private void DeleteFromForgets(MemoryEntry memoryEntry)
    {
        if (forgetting.ContainsKey(memoryEntry.InstanceID))
        {
            ForgettingQueue[memoryEntry.TimeStamp].Remove(memoryEntry);
            if (ForgettingQueue[memoryEntry.TimeStamp].Count == 0)
                ForgettingQueue.Remove(memoryEntry.TimeStamp);

            forgetting.Remove(memoryEntry.InstanceID);
        }
    }

    public HashSet<MemoryEntry> expectedObeserves(Vector3 searchPosition, float observationRadius)
    {
        HashSet<MemoryEntry> returnMemoryEntries = new HashSet<MemoryEntry>();

        // this can't be efficient!
        IEnumerable<KeyValuePair<float, SortedDictionary<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>>>> xs = positionIndex.Where(x => x.Key >= searchPosition.x - observationRadius &&
                                 x.Key <= searchPosition.x + observationRadius);
        foreach (KeyValuePair<float, SortedDictionary<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>>> y in xs)
        {
            IEnumerable<KeyValuePair<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>>> ys = y.Value.Where(localY => localY.Key >= searchPosition.y - observationRadius &&
                                     localY.Key <= searchPosition.y + observationRadius);
            foreach (KeyValuePair<float, SortedDictionary<float, Dictionary<int, MemoryEntry>>> z in ys)
            {
                IEnumerable<KeyValuePair<float, Dictionary<int, MemoryEntry>>> zs = z.Value.Where(localZ => localZ.Key >= searchPosition.z - observationRadius &&
                                         localZ.Key <= searchPosition.z + observationRadius);
                foreach(KeyValuePair<float, Dictionary<int, MemoryEntry>> position in zs) {
                    foreach (MemoryEntry memoryEntry in position.Value.Values)
                        if (Vector3.Distance(memoryEntry.Position, searchPosition) <= observationRadius)
                            returnMemoryEntries.Add(memoryEntry);
                }
            }
        }

        return returnMemoryEntries;
    }
}
