using UnityEngine;
using System.Collections.Generic;

public class BuildTask : Task {

    GameObject buildTarget;
    public GameObject BuildTarget
    {
        get
        {
            return buildTarget;
        }

        set
        {
            buildTarget = value;
            buildDistance = 1 + buildTarget.GetComponent<BuildSiteHandler>().BuildPlan.Size.x;
        }
    }

    BuildStep currentStep;
    Target target;
    float buildTimer;
    float buildDistance;
    float waitForResourcesTimer;
    BuildSiteHandler buildSiteHandler;

    // Use this for initialization
    public override void Start () {
        base.Start();

        target = GetComponent<Target>();
        currentStep = BuildStep.Idle;
        buildTimer = 0;
        waitForResourcesTimer = 0;
        buildSiteHandler = buildTarget.GetComponent<BuildSiteHandler>();
        equipBestForTask(Tags.BuildSite);
    }

    void OnEnable()
    {
        completingTask = false;
        buildTimer = 0;
        waitForResourcesTimer = 0;
    }

    // Update is called once per frame
    protected override void Update ()
    {
        if (target.target == null)
            currentStep = BuildStep.Idle;

        if (buildTarget == null && !completingTask)
        {
            completingTask = true;
            currentStep = BuildStep.Idle;
        }

        switch (currentStep)
        {
            case BuildStep.MovingToBuild:
                MovingToBuildUpdate();
                break;
            case BuildStep.Building:
                BuildingUpdate();
                break;
            case BuildStep.ReturningToResources:
                ReturningToResourcesUpdate();
                break;
            case BuildStep.Idle:
                IdleUpdate();
                break;
            default:
                currentStep = BuildStep.Idle;
                break;
        }

        base.Update();
    }

    private void MovingToBuildUpdate()
    {
        float distanceToResource = Vector3.Distance(target.target.position, transform.position);
        
        if (distanceToResource <= buildDistance)
            currentStep = BuildStep.Idle;
    }

    private void BuildingUpdate()
    {
        buildTimer += Time.deltaTime;

        float buildRate = 1 / (1 + getInteractionEquipedMultiplier(Tags.BuildSite));
        if (buildTimer >= buildRate)
        {
            // todo if there are gather mulitpliers, add logic for that here
            uint totalBuild = System.Convert.ToUInt32(buildTimer / buildRate);
            buildTimer -= totalBuild * buildRate;
            uint buildTracker = 0;

            Dictionary<Item, uint> resourceEntries = inventory.getEntriesOfTypes(Tags.Resources);
            uint countOfDepletedResources = 0;

            foreach (KeyValuePair<Item, uint> inventoryEntry in resourceEntries)
            {
                if (buildSiteHandler.BuildPlan.Cost.ContainsKey(inventoryEntry.Key.Tag) && inventoryEntry.Value > 0)
                {
                    // deez variable names lul
                    uint leftOverBuild = totalBuild - buildTracker;
                    uint leftOverBuilt = buildSiteHandler.BuildPlan.Cost[inventoryEntry.Key.Tag] - buildSiteHandler.Built[inventoryEntry.Key.Tag];
                    uint finalBuildAmount = System.Math.Min(leftOverBuilt, leftOverBuild);
                    totalBuild += finalBuildAmount;
                    inventory.remove(inventoryEntry.Key.Tag, finalBuildAmount);
                    buildSiteHandler.Built[inventoryEntry.Key.Tag] += finalBuildAmount;

                    if(buildSiteHandler.NotifyBuild())
                    {
                        completingTask = true;
                        currentStep = BuildStep.Idle;
                        return;
                    }
                }
                else
                    countOfDepletedResources++;
                
                if (inventoryEntry.Value == 0)
                    countOfDepletedResources++;
            }

            if (countOfDepletedResources == resourceEntries.Count)
                currentStep = BuildStep.Idle;
        }
    }

    private void ReturningToResourcesUpdate()
    {
        float distance = Vector3.Distance(target.target.position, transform.position);

        // todo calculate stopping distance based on size of drop point
        if (distance <= 3)
        {
            Inventory.transferAll(inventory, target.target.root.gameObject.GetComponent<Inventory>());
            if (completingTask)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }
            currentStep = BuildStep.Idle;
        }
    }

    private void IdleUpdate()
    {
        dontMove();
        // if completing task, try to drop off first
        if (completingTask)
        {
            target.target = findClosestTeamOrNeutralObjectsWithTag(Tags.TownCenterDropOff);

            if (target != null && Vector3.Distance(target.target.position, transform.position) <= 2)
            {
                Inventory transferInventory = target.target.root.gameObject.GetComponent<Inventory>();
                Inventory.transferAll(inventory, transferInventory);
                taskHandler.notifyTaskCompleted();
                return;
            } else if (target == null)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }

            move();
            currentStep = BuildStep.ReturningToResources;
            return;
        }

        bool hasResourcesToBuild = hasBuildResources();
        if (hasResourcesToBuild && Vector3.Distance(buildTarget.transform.position, transform.position) <= buildDistance)
        {
            currentStep = BuildStep.Building;
        } else if (hasResourcesToBuild)
        {
            target.target = buildTarget.transform;
            move();
            currentStep = BuildStep.MovingToBuild;
        } else
        {
            HashSet<GameObject> resoucePoints = getTeamOrNeutralObjectsWithTag(Tags.TownCenterDropOff);
            ICollection<string> resourcesLeftToBuild = buildSiteHandler.resourcesLeftToBuild();

            resoucePoints.RemoveWhere(rp => !rp.transform.root.GetComponent<Inventory>().containsAny(resourcesLeftToBuild));

            Transform bestPlaceToGetResources = findClosest(resoucePoints);

            // I'm not sure if workers should wait for more resources or quit their task
            if (bestPlaceToGetResources == null)
            {
                waitForResourcesTimer += Time.deltaTime;
                if (waitForResourcesTimer > 30)
                    completingTask = true;
                return;
            }
            waitForResourcesTimer = 0;

            if (Vector3.Distance(target.target.position, bestPlaceToGetResources.position) <= 2)
            {
                Inventory transferInventory = target.target.root.gameObject.GetComponent<Inventory>();
                Inventory.transferAll(inventory, transferInventory);
                transferBestForTask(transferInventory, Tags.BuildSite);
                equipBestForTask(Tags.BuildSite);
                Item bestItemCanCraft = bestItemCanCraftBySlots(transferInventory, Tags.BuildSite, inventory.getEmptySlots());
                if (bestItemCanCraft != null)
                {
                    taskHandler.pushTask(Tasks.CraftingTask);
                    GetComponent<CraftingTask>().BuildItem = bestItemCanCraft;
                    return;
                }
                Inventory.transferTypes(bestPlaceToGetResources.root.GetComponent<Inventory>(), inventory, resourcesLeftToBuild, (uint)(MAX_WEIGHT - inventory.weight));
            } else
            {
                target.target = bestPlaceToGetResources;
                move();
                currentStep = BuildStep.ReturningToResources;
            }
        }
    }

    override public void completeTask()
    {
        completingTask = true;
        currentStep = BuildStep.Idle;
    }

    bool hasBuildResources()
    {
        foreach(KeyValuePair<string,uint> builtEntry in buildSiteHandler.Built)
        {
            // if we still need more resources and they're in our inventory
            if (buildSiteHandler.BuildPlan.Cost[builtEntry.Key] > builtEntry.Value && inventory.getTotalCountByType(builtEntry.Key) > 0)
            {
                return true;
            }
        }

        return false;
    }
    
    enum BuildStep
    {
        Building,
        Idle,
        MovingToBuild,
        ReturningToResources
    }
}