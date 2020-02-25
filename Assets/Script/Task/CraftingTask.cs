using UnityEngine;
using System.Collections.Generic;

public class CraftingTask : Task
{
    Dictionary<string, uint> leftToBuild = new Dictionary<string, uint>();
    CraftingStep currentStep;
    Target target;
    float buildTimer;

    Item buildItem;
    public Item BuildItem
    {
        get
        {
            return buildItem;
        }
        set
        {
            buildItem = value;
            leftToBuild = buildItem.CraftCost;
        }
    }

    public override void Start()
    {
        base.Start();
        
        target = GetComponent<Target>();
        completingTask = false;
    }

    void OnEnable()
    {
        completingTask = false;
    }

    protected override void Update()
    {
        if (target.target == null)
            currentStep = CraftingStep.Idle;

        switch (currentStep)
        {
            case CraftingStep.MovingToResources:
                MovingToResourcesUpdate();
                break;
            case CraftingStep.Building:
                BuildingUpdate();
                break;
            case CraftingStep.Idle:
                IdleUpdate();
                break;
            default:
                currentStep = CraftingStep.Idle;
                break;
        }

        base.Update();
    }

    private void MovingToResourcesUpdate()
    {
        float distance = Vector3.Distance(target.target.position, transform.position);

        // todo calculate stopping distance based on size of drop point
        if (distance <= 3)
        {
            Inventory.transferAllOfTypes(inventory, target.target.root.gameObject.GetComponent<Inventory>(), Tags.Resources);
            if (completingTask)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }
            currentStep = CraftingStep.Idle;
        }
    }

    private void BuildingUpdate()
    {
        buildTimer += Time.deltaTime;

        float buildRate = 1;
        if (buildTimer >= buildRate)
        {
            // todo if there are gather mulitpliers, add logic for that here
            uint totalBuild = System.Convert.ToUInt32(buildTimer / buildRate);
            buildTimer -= totalBuild * buildRate;

            Dictionary<Item, uint> resourceEntries = inventory.getEntriesOfTypes(Tags.Resources);
            if (resourceEntries.Count == 0)
            {
                currentStep = CraftingStep.Idle;
                return;
            }

            uint countOfBuilt = 0;
            foreach (KeyValuePair<Item, uint> inventoryEntry in resourceEntries)
            {
                if (leftToBuild.ContainsKey(inventoryEntry.Key.Tag) && inventoryEntry.Value > 0)
                {
                    uint finalBuildAmount = System.Math.Min(totalBuild - countOfBuilt, leftToBuild[inventoryEntry.Key.Tag]);
                    countOfBuilt += finalBuildAmount;
                    inventory.remove(inventoryEntry.Key.Tag, finalBuildAmount);

                    leftToBuild[inventoryEntry.Key.Tag] -= finalBuildAmount;
                    if (leftToBuild[inventoryEntry.Key.Tag] == 0)
                        leftToBuild.Remove(inventoryEntry.Key.Tag);
                }
            }

            if (leftToBuild.Count == 0)
            {
                inventory.add(buildItem,1);
                completingTask = true;
                currentStep = CraftingStep.Idle;
            }
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
                Inventory.transferAllOfTypes(inventory, target.target.root.GetComponent<Inventory>(), Tags.Resources);
                taskHandler.notifyTaskCompleted();
                return;
            }
            else if (target == null)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }

            move();
            currentStep = CraftingStep.MovingToResources;
            return;
        }
        
        if (hasBuildResources())
        {
            currentStep = CraftingStep.Building;
        } else
        {
            HashSet<GameObject> resoucePoints = getTeamOrNeutralObjectsWithTag(Tags.TownCenterDropOff);

            resoucePoints.RemoveWhere(rp => !rp.transform.root.GetComponent<Inventory>().containsAny(leftToBuild.Keys));

            Transform bestPlaceToGetResources = findClosest(resoucePoints);

            // I'm not sure if workers should wait for more resources or quit their task
            if (bestPlaceToGetResources == null)
            {
                completingTask = true;
                return;
            }

            if (Vector3.Distance(target.target.position, bestPlaceToGetResources.position) <= 2)
            {
                Inventory.transferTypes(bestPlaceToGetResources.root.GetComponent<Inventory>(), inventory, leftToBuild.Keys, (uint)(MAX_WEIGHT - inventory.weight));
            }
            else
            {
                target.target = bestPlaceToGetResources;
                move();
                currentStep = CraftingStep.MovingToResources;
            }
        }
    }

    override public void completeTask()
    {
        completingTask = true;
        currentStep = CraftingStep.Idle;
    }

    bool hasBuildResources()
    {
        foreach (KeyValuePair<string, uint> builtEntry in leftToBuild)
        {
            // if we still need more resources and they're in our inventory
            if (builtEntry.Value > 0 && inventory.getTotalCountByType(builtEntry.Key) > 0)
            {
                return true;
            }
        }

        return false;
    }

    enum CraftingStep
    {
        Building,
        Idle,
        MovingToResources
    }
}
