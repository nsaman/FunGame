using UnityEngine;
using System.Collections.Generic;
using System;

public class CraftingTask : TargettedTask
{
    public const int SKILL_LEVEL_DIVISOR = 10;

    Dictionary<string, uint> leftToBuild = new Dictionary<string, uint>();
    CraftingStep currentStep;
    float buildTimer;
    RandomSingleton rand;

    CraftedItem buildItem;
    public CraftedItem BuildItem
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

        currentStep = CraftingStep.Idle;
        completingTask = false;
        rand = RandomSingleton.Instance;
    }

    void OnEnable()
    {
        completingTask = false;
    }

    protected override void Update()
    {
        if (target.TargetMemory == null)
            currentStep = CraftingStep.Idle;

        switch (currentStep)
        {
            case CraftingStep.MovingToResources:
                MovingToResourcesUpdate();
                break;
            case CraftingStep.Crafting:
                CraftingUpdate();
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
        // todo calculate stopping distance based on size of drop point
        if (withinDistanceOfTarget(3))
        {
            Inventory targetInventory = target.target.transform.root.gameObject.GetComponent<Inventory>();
            Inventory.transferAllOfTypes(inventory, targetInventory, Tags.Resources);
            if (completingTask)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }
            if (targetInventory.containsAny(leftToBuild.Keys))
                currentStep = CraftingStep.Idle;
            else
            {
                taskHandler.notifyTaskCompleted();
                return;
            }
        }
    }

    private void CraftingUpdate()
    {
        buildTimer += Time.deltaTime;

        float buildRate = 1;
        if (buildTimer >= buildRate)
        {
            // todo if there are craft mulitpliers, add logic for that here
            uint buildTime = System.Convert.ToUInt32(buildTimer / buildRate);
            uint totalBuild = System.Convert.ToUInt32(buildTime * (skills.getLevel(Skills.Crafting) / SKILL_LEVEL_DIVISOR + 1));
            buildTimer -= buildTime * buildRate;

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

                    skills.gainXp(Skills.Crafting, (int)finalBuildAmount);

                    leftToBuild[inventoryEntry.Key.Tag] -= finalBuildAmount;
                    if (leftToBuild[inventoryEntry.Key.Tag] == 0)
                        leftToBuild.Remove(inventoryEntry.Key.Tag);
                }
            }

            if (leftToBuild.Count == 0)
            {
                inventory.add(
                    buildItem.craft(Math.Max(Math.Min(1f + (float)rand.NextNormalDistribution(skills.getLevel(Skills.Crafting)/100, .2),2), 1))
                    , 1);
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
            if (target.TargetMemory == null || !target.TargetMemory.Tag.Equals(Tags.TownCenter))
                target.TargetMemory = findClosestTeamOrNeutralMemoriesWithTag(Tags.TownCenter);

            if (target.TargetMemory != null && withinDistanceOfTarget(2))
            {
                Inventory.transferAllOfTypes(inventory, target.target.transform.root.GetComponent<Inventory>(), Tags.Resources);
                taskHandler.notifyTaskCompleted();
                return;
            }
            else if (target.TargetMemory == null)
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
            currentStep = CraftingStep.Crafting;
        } else
        {
            if (target.TargetMemory == null || !target.TargetMemory.Tag.Equals(Tags.TownCenter))
                target.TargetMemory = findClosestTeamOrNeutralMemoriesWithTag(Tags.TownCenter);

            // I'm not sure if workers should wait for more resources or quit their task
            if (target.TargetMemory == null)
            {
                completingTask = true;
                return;
            }

            if (withinDistanceOfTarget(2))
            {
                Inventory targetInventory = target.target.transform.root.GetComponent<Inventory>();
                Inventory.transferTypes(targetInventory, inventory, leftToBuild.Keys, (uint)(maxWeight() - inventory.weight));
                currentStep = CraftingStep.Crafting;
            }
            else
            {
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
        Crafting,
        Idle,
        MovingToResources
    }
}
