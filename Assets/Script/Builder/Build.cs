using UnityEngine;
using System.Collections.Generic;

public abstract class Build
{
    protected Dictionary<string, uint> cost;
    protected float buildTime;
    protected float progressTime;

    public Build()
    {
        progressTime = 0;
    }

    public Dictionary<string, uint> Cost
    {
        get
        {
            return cost;
        }
    }

    public float BuildTime
    {
        get
        {
            return buildTime;
        }
    }

    // returns true if completed, false if uncompleted
    public bool continueBuild()
    {
        progressTime += Time.deltaTime;
        if (progressTime >= buildTime)
            return true;

        return false;
    }

    public abstract void handleComplete(Vector3 position);
}
