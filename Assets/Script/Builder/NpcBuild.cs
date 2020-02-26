using UnityEngine;
using System.Collections.Generic;

public class NpcBuild : Build {
    
    public NpcBuild() : base()
    {
        cost = new Dictionary<string, uint>();
        cost.Add(Tags.Food, 10);
        buildTime = 2f;
    }

    public override void handleComplete(Vector3 position, TeamController teamController)
    {
        GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefab/" + Tags.Npc), position, new Quaternion());

        TeamPointer teamPointer = gameObject.GetComponent<TeamPointer>();
        if (teamPointer != null)
            teamPointer.TeamController = teamController;
    }
}
