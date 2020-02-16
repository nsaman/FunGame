using System.Collections.Generic;
using UnityEngine;

public class BuildPlan {

    public BuildPlan(string finalBuild, Vector3 size, Dictionary<string, uint> cost) {
        this.finalBuild = finalBuild;
        this.cost = cost;
        this.size = size;
    }

    private string finalBuild;
    public string FinalBuild
    {
        get
        {
            return finalBuild;
        }
    }

    private Vector3 size;
    public Vector3 Size
    {
        get
        {
            return size;
        }
    }

    private Dictionary<string, uint> cost;
    public Dictionary<string, uint> Cost
    {
        get
        {
            return cost;
        }
    }

}
