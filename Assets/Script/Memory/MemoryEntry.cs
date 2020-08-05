using UnityEngine;

public class MemoryEntry
{
    public MemoryEntry(int instanceID, Vector3 position, bool moving, long timeStamp, string tag, TeamController team, bool dangerous)
    {
        InstanceID = instanceID;
        Position = position;
        Moving = moving;
        TimeStamp = timeStamp;
        Tag = tag;
        Team = team;
        Dangerous = dangerous;
    }

    public int InstanceID { get; }
    public Vector3 Position { get; }
    public bool Moving { get; }
    public long TimeStamp { get; }
    public string Tag { get; }
    public TeamController Team { get; }
    public bool Dangerous { get; }
}
