using UnityEngine;
using System.Collections.Generic;

public class NpcBuild : Build {
    
    public NpcBuild() : base()
    {
        cost = new Dictionary<string, uint>();
        cost.Add(Tags.Food, 2);
        buildTime = 2f;
    }

    public override void handleComplete(Vector3 position)
    {
        Object.Instantiate(Resources.Load("Prefab/" + Tags.Npc), position, new Quaternion());
    }
}
