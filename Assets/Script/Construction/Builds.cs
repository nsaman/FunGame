﻿using UnityEngine;
using System.Collections.Generic;

public static class Builds
{
    public static BuildPlan House { get {
            Dictionary<string, uint> cost = new Dictionary<string, uint>();
            cost.Add(Tags.Wood, 2);
            return new BuildPlan(Tags.House, new Vector3(2.5f, 2.5f, 2.5f), cost); } }

    
    public static bool canBuild(Inventory inventory, BuildPlan buildPlan)
    {
        foreach(KeyValuePair<string,uint> cost in buildPlan.Cost)
        {
            if (inventory.getTotalCountByType(cost.Key) < cost.Value)
                return false;
        }

        return true;
    }
}