using UnityEngine;
using System.Collections.Generic;

public class BuildSiteHandler : MonoBehaviour {

    Dictionary<string, uint> built;
    public Dictionary<string, uint> Built
    {
        get
        {
            return built;
        }
    }

    BuildPlan buildPlan;
    public BuildPlan BuildPlan
    {
        get
        {
            return buildPlan;
        }

        set
        {
            buildPlan = value;
        }
    }
    // Use this for initialization
    void Start () {
        built = new Dictionary<string, uint>();
        GetComponent<Transform>().root.localScale = BuildPlan.Size;
        foreach (KeyValuePair<string, uint> entry in buildPlan.Cost)
            built.Add(entry.Key, 0);

        // don't forget to initialize after instantiating and setting BuildPlan!
    }

    // Update is called once per frame
    void Update () {
	
	}

    // used to let the know the BuildSiteHandler that some progress has been made on the build and to check progress
    public bool NotifyBuild()
    {
        foreach (KeyValuePair<string, uint> costEntry in buildPlan.Cost)
        {
            if (built[costEntry.Key] < costEntry.Value)
                return false;
        }

        Transform buildSiteTransform = GetComponent<Transform>().root;

        System.Random rnd = new System.Random();

        buildSiteTransform.Rotate(0, rnd.Next(0, 5) * 90, 0);

        // all built up, transform into the build!
        GameObject building = (GameObject)Object.Instantiate(Resources.Load("Prefab/" + Tags.House), buildSiteTransform.position, buildSiteTransform.rotation);
        building.GetComponent<TeamPointer>().TeamController = GetComponent<TeamPointer>().TeamController;

        Destroy(gameObject);
        return true;
    }
    
    public HashSet<string> resourcesLeftToBuild()
    {
        HashSet<string> resourcesLeftToBuild = new HashSet<string>();

        foreach (KeyValuePair<string,uint> costEntry in buildPlan.Cost)
        {
            if (built[costEntry.Key] < costEntry.Value)
                resourcesLeftToBuild.Add(costEntry.Key);
        }

        return resourcesLeftToBuild;
    }
}
