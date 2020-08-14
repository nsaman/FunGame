using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class GatheringTask : TargettedTask
{
    public const int SKILL_LEVEL_DIVISOR = 10;

    GatheringStep currentStep;
    float gatherTimer;
    bool waitForMoreResourcesAtTownCenter;
    protected string targetTag;
    protected Skill usedSkill;
    protected List<Skill> trainedSkills;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        currentStep = GatheringStep.Idle;
        gatherTimer = 0;
        waitForMoreResourcesAtTownCenter = false;
        equipBestForTask(targetTag);
    }

    void OnEnable()
    {
        completingTask = false;
        currentStep = GatheringStep.Idle;
        /*float distanceDropOff = Vector3.Distance(target.target.position, transform.position);

        // todo calculate stopping distance based on size of drop point
        if (distanceDropOff <= 3)
        {
            Inventory transferInventory = target.target.root.gameObject.GetComponent<Inventory>();
            Inventory.transferAll(inventory, transferInventory);
            transferBestForTask(transferInventory, targetTag);
            equipBestForTask(targetTag);
            Item bestItemCanCraft = bestItemCanCraftBySlots(transferInventory, targetTag, inventory.getEmptySlots());
            if (bestItemCanCraft != null)
            {
                taskHandler.pushTask(Tasks.CraftingTask);
                GetComponent<CraftingTask>().BuildItem = bestItemCanCraft; 
            }
            currentStep = GatheringStep.Idle;
        }*/
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (target.TargetMemory == null)
            currentStep = GatheringStep.Idle;

        switch (currentStep)
        {
            case GatheringStep.MovingToResource:
                MovingToResourceUpdate();
                break;
            case GatheringStep.Gathering:
                GatheringUpdate();
                break;
            case GatheringStep.DroppingOffResources:
                DroppingOffResourcesUpdate();
                break;
            case GatheringStep.Idle:
                IdleUpdate();
                break;
            default:
                currentStep = GatheringStep.Idle;
                break;
        }

        base.Update();
    }

    override public void completeTask()
    {
        completingTask = true;
        dropOffTownCenter();
        if (target.TargetMemory == null)
        {
            taskHandler.notifyTaskCompleted();
        }
    }

    void MovingToResourceUpdate()
    {
        MemoryEntry closestMemory = findClosestTeamOrNeutralMemoriesWithTag(targetTag);
        if (closestMemory != null)
        {
            target.TargetMemory = closestMemory;
        }
        // todo calculate stopping distance based on size of resource
        if (withinDistanceOfTarget(2))
            currentStep = GatheringStep.Gathering;
    }

    void GatheringUpdate()
    {
        dontMove();
        if (target.target == null || inventory.weight >= maxWeight())
        {
            currentStep = GatheringStep.Idle;
            return;
        }

        // this should be replaced by item speed/iteraction
        float gatherRate = 1 / (1 + getInteractionEquipedMultiplier(targetTag));
        gatherTimer += Time.deltaTime;
        if (gatherTimer >= gatherRate)
        {
            uint gatherTime = Convert.ToUInt32(gatherTimer / gatherRate);
            uint totalGather = Convert.ToUInt32(gatherTime * (skills.getLevel(usedSkill) / SKILL_LEVEL_DIVISOR + 1));
            gatherTimer -= gatherTime * gatherRate;

            Inventory targetInventory = target.target.transform.root.GetComponent<Inventory>();
            uint realGatherAmount = Math.Min(targetInventory.getTotalCountByTypes(Tags.Resources), totalGather);

            foreach (Skill skill in trainedSkills)
                skills.gainXp(skill, (int)realGatherAmount);

            Inventory.transferTypes(targetInventory, inventory, Tags.Resources, realGatherAmount);

            if (targetInventory.getTotalCountByTypes(Tags.Resources) <= 0)
            {
                Destroy(target.target.transform.root.gameObject);
                remembers.Forget(target.TargetMemory);
                currentStep = GatheringStep.Idle;
            }
        }
    }

    void DroppingOffResourcesUpdate()
    {
        // todo calculate stopping distance based on size of drop point
        if (withinDistanceOfTarget(3))
        {
            Inventory transferInventory = target.target.transform.root.gameObject.GetComponent<Inventory>();
            Inventory.transferAll(inventory, transferInventory);
            if (completingTask)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }
            transferBestForTask(transferInventory, targetTag, skills.getLevel(Skills.Strength) - 1);
            equipBestForTask(targetTag);
            CraftedItem bestItemCanCraft = bestItemCanCraftBySlots(transferInventory, targetTag, inventory.getEmptySlots(), skills.getLevel(Skills.Strength) - 1);
            if (bestItemCanCraft != null)
            {
                taskHandler.pushTask(Tasks.CraftingTask);
                GetComponent<CraftingTask>().BuildItem = bestItemCanCraft;
            }
            currentStep = GatheringStep.Idle;
        }
    }

    void IdleUpdate()
    {
        dontMove();

        // drop off if inventory is full
        if (inventory.weight >= maxWeight())
        {
            dropOffTownCenter();
        }
        // no resources in inventory
        else if(inventory.getTotalCountByTypes(Tags.Resources) == 0)
        {
            MemoryEntry closestMemory = findClosestTeamOrNeutralMemoriesWithTag(targetTag);
            if (closestMemory != null)
            {
                move();
                target.TargetMemory = closestMemory;
                currentStep = GatheringStep.MovingToResource;
            }
            // no resource to gather
            else
            {
                if (!waitForMoreResourcesAtTownCenter)
                {
                    completeTask();
                }
            }
        }
        // partial inventory, either drop off or gather based on what's closer
        else
        {
            MemoryEntry closestMemory = findClosestTeamOrNeutralMemoriesWithTags(new string[] { targetTag, Tags.TownCenter });
            if (closestMemory != null)
            {
                move();
                target.TargetMemory = closestMemory;
                if (target.TargetMemory.Tag.Equals(targetTag))
                {
                    currentStep = GatheringStep.MovingToResource;
                }
                else
                {
                    currentStep = GatheringStep.DroppingOffResources;
                }
            }
        }
    }

    private void dropOffTownCenter()
    {
        target.TargetMemory = findClosestTeamOrNeutralMemoriesWithTag(Tags.TownCenter);
        move();
        if (target.TargetMemory != null)
            currentStep = GatheringStep.DroppingOffResources;
    }

    // todo create a public gathering enum
    enum GatheringStep
    {
        MovingToResource,
        Gathering,
        DroppingOffResources,
        Idle
    }
}
