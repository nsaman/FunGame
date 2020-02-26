using UnityEngine;
using System.Collections.Generic;

public class TeamController {

    public uint team = 0;
    public Color32 teamColor;
    private AI ai;
    public AI AI
    {
        get { return ai; }
    }
    private Dictionary<string, List<TeamPointer>> registry;
    public Dictionary<string, List<TeamPointer>> Registry
    {
        get { return registry; }
    }

    public TeamController()
    {
        registry = new Dictionary<string, List<TeamPointer>>();

        foreach (string tag in Tags.Teamed)
            registry.Add(tag, new List<TeamPointer>());

        ai = new AI(this);
    }

    public void register(TeamPointer teamPointer)
    {
        string type = teamPointer.gameObject.transform.root.gameObject.tag;

        if (!registry.ContainsKey(type))
            registry.Add(type, new List<TeamPointer>());

        registry[type].Add(teamPointer);
    }

    public void deregister(TeamPointer teamPointer)
    {
        string type = teamPointer.gameObject.transform.root.gameObject.tag;

        if (registry.ContainsKey(type) && registry[type].Contains(teamPointer))
            registry[type].Remove(teamPointer);
    }
}
