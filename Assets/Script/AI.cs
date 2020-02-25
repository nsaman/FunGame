using UnityEngine;

public class AI {

    TeamController teamController;
    bool tryBuild;

    public AI(TeamController teamController)
    {
        this.teamController = teamController;
        tryBuild = true;
    }

    public void handleTeam()
    {

        System.Random rnd = new System.Random();
        foreach (TeamPointer teamPointer in teamController.Registry[Tags.Npc])
        {
            TaskHandler taskHandler = teamPointer.gameObject.GetComponent<TaskHandler>();
            
            if (taskHandler.CurrentTask is IdleTask)
            {
                if(rnd.Next(1, 3) == 1)
                    taskHandler.setTask(Tasks.WoodGathering);
                else
                    taskHandler.setTask(Tasks.BerryGathering);
            }
        }

        foreach (TeamPointer teamPointer in teamController.Registry[Tags.TownCenter])
        {
            teamPointer.transform.root.GetComponent<TownCenterBuilder>().activelyBuild(typeof(NpcBuild));

            Inventory inventory = teamPointer.transform.root.GetComponent<Inventory>();

            if (tryBuild && teamController.Registry.ContainsKey(Tags.BuildSite) && teamController.Registry[Tags.BuildSite].Count == 0 && Builds.canBuild(inventory, Builds.House))
            {
                Quaternion tcQuaternion = teamPointer.transform.root.rotation;
                // + 2 is the space between buildings
                Vector3 buildSize = Builds.House.Size;

                // start box away from tc
                Vector3 currentBuildLocation = teamPointer.transform.root.position;

                Vector3 closestConstructionSite = findClosestConstructionSite(50, 2f, 2f, currentBuildLocation, buildSize, tcQuaternion);
                if (closestConstructionSite != NOT_FOUND)
                {
                    Vector3 finalBuildSize = buildSize;
                    finalBuildSize.y = 1;
                    GameObject buildSite = (GameObject)Object.Instantiate(Resources.Load("Prefab/" + Tags.BuildSite), closestConstructionSite, tcQuaternion);
                    buildSite.transform.localScale = finalBuildSize;

                    BuildSiteHandler buildSiteHandler = buildSite.GetComponent<BuildSiteHandler>();
                    buildSiteHandler.BuildPlan = Builds.House;

                    foreach (TeamPointer npcTeamPointer in teamController.Registry[Tags.Npc])
                    {
                        TaskHandler taskHandler = npcTeamPointer.transform.root.GetComponent<TaskHandler>();
                        if (!(taskHandler.CurrentTask is BuildTask))
                        {
                            taskHandler.setTask(Tasks.BuildTask);
                            npcTeamPointer.transform.root.GetComponent<BuildTask>().BuildTarget = buildSite;
                            break;
                        }
                    }
                }
                else
                    tryBuild = false;
            }
        }

    }

    public static Vector3 findClosestConstructionSite(float maxDistance, float horizontalSpacing, float searchGranularity, Vector3 startingBuildLocation, Vector3 buildSize, Quaternion rotation)
    {
        float maxX = maxDistance + startingBuildLocation.x;
        float maxZ = maxDistance + startingBuildLocation.z;

        return findClosestConstructionSite(maxX, maxZ, horizontalSpacing, searchGranularity, startingBuildLocation, buildSize, rotation);
    }

    public static Vector3 findClosestConstructionSite(float maxX, float maxZ, float horizontalSpacing, float searchGranularity, Vector3 startingBuildLocation, Vector3 buildSize, Quaternion rotation)
    {
        // assume startingBuildLocation is on the ground
        Vector3 currentBuildLocation = startingBuildLocation + new Vector3(0, buildSize.y, 0);
        Vector3 buildSizeWithSpacing = buildSize + new Vector3(horizontalSpacing, 0, horizontalSpacing);

        int advanceAmount = 1;
        int currentAdvanced = 0;
        bool moveX = true;
        bool movePositive = true;

        // probably should do logic on a circle instead of a square
        while (currentBuildLocation.x < (maxX - buildSize.x) && currentBuildLocation.x > (-maxX + buildSize.x)
            && currentBuildLocation.y < (maxZ - buildSize.y) && currentBuildLocation.y > (-maxZ + buildSize.y))
        {
            Collider[] collisions = Physics.OverlapBox(currentBuildLocation, buildSizeWithSpacing, rotation);

            if (collisions.Length == 1 && collisions[0].tag.Equals(Tags.Ground))
            {
                currentBuildLocation.y = startingBuildLocation.y;
                return currentBuildLocation;
            }

            // screw one liners
            if (currentAdvanced >= advanceAmount)
            {
                if (moveX && movePositive)
                    moveX = false;
                else if (!moveX && movePositive)
                {
                    moveX = true;
                    movePositive = false;
                    advanceAmount++;
                }
                else if (moveX && !movePositive) 
                    moveX = false;
                else if (!moveX && !movePositive)
                {
                    moveX = true;
                    movePositive = true;
                    advanceAmount++;
                }

                currentAdvanced = 0;
            }

            if (moveX && movePositive)
                currentBuildLocation += rotation * new Vector3(searchGranularity, 0, 0);
            else if (moveX && !movePositive)
                currentBuildLocation -= rotation * new Vector3(searchGranularity, 0, 0);
            else if (!moveX && movePositive)
                currentBuildLocation += rotation * new Vector3(0, 0, searchGranularity);
            else if (!moveX && !movePositive)
                currentBuildLocation -= rotation * new Vector3(0, 0, searchGranularity);

            currentAdvanced++;
        }

        // returning some garbage since c# has structs that can't be null (???)
        return NOT_FOUND;
    }

    // some garbage
    public static Vector3 NOT_FOUND = new Vector3(float.MinValue, float.MinValue, float.MinValue);
}
