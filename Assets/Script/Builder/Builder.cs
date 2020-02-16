using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Builder : MonoBehaviour {

    protected Inventory inventory;

    protected HashSet<Type> builds;
    public HashSet<Type> Builds
    {
        get
        {
            return builds;
        }
    }
    private Vector3 spawnPosition;
    protected Vector3 SpawnPosition
    {
        get
        {
            return spawnPosition;
        }

        set
        {
            spawnPosition = value;
        }
    }
    // list of builds that have been paid for
    private List<Build> activeQueue;
    protected List<Build> ActiveQueue
    {
        get
        {
            return activeQueue;
        }
    }
    // list of builds that have not been paid for
    private List<Build> buildQueue;
    protected List<Build> BuildQueue
    {
        get
        {
            return buildQueue;
        }
    }


    // Use this for initialization
    protected virtual void Start () {
        inventory = gameObject.transform.root.GetComponent<Inventory>();
        setSpawnPosition();
        builds = new HashSet<Type>();
        activeQueue = new List<Build>();
        buildQueue = new List<Build>();
    }

    protected abstract void setSpawnPosition();

    // Update is called once per frame
    protected virtual void Update () {
        if (activeQueue.Count > 0)
        {
            // build and check completeness
            if (activeQueue[0].continueBuild())
            {
                activeQueue[0].handleComplete(spawnPosition);
                activeQueue.RemoveAt(0);
                tryPromoteQueued();
            }
        }
        else
            tryPromoteQueued();
    }

    protected void tryPromoteQueued()
    {
        // activelyBuild() will add the queued build
        if (buildQueue.Count > 0 && activelyBuild(buildQueue[0]))
            buildQueue.RemoveAt(0);
    }

    // returns false if could not build
    public void queueBuild(Type build)
    {
        Build buildObject = (Build)Activator.CreateInstance(build);
        buildQueue.Add(buildObject);
    }

    public bool canBuild(Build build)
    {
        return hasBuild(build) && hasResourcesToBuild(build);
    }

    public bool hasBuild(Build build)
    {
        return builds.Contains(build.GetType());
    }

    public bool hasResourcesToBuild(Build build)
    {
        foreach (KeyValuePair<string, uint> costEntry in build.Cost)
        {
            bool costEntryEvaluated = false;

            // todo add logic to handle multiple stacks of inventoryEntry and roll over to next entry if we have multiple same entry types
            foreach (InventoryEntry inventoryEntry in inventory.inventory)
            {
                if (costEntry.Key.Equals(inventoryEntry.item))
                {
                    if (costEntry.Value <= inventoryEntry.count)
                        costEntryEvaluated = true;
                    else
                        return false;
                }
            }

            if (!costEntryEvaluated)
                return false;
        }

        return true;
    }

    public bool activelyBuild(Type build)
    {
        Build buildObject = (Build)Activator.CreateInstance(build);
        return activelyBuild(buildObject);
    }

    public bool activelyBuild(Build build)
    {
        if (canBuild(build))
        {
            foreach (KeyValuePair<string, uint> costEntry in build.Cost)
            {
                inventory.remove(costEntry.Key, costEntry.Value);
            }

            activeQueue.Insert(0, build);

            return true;
        }

        return false;
    }
}
