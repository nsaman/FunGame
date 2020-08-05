using UnityEngine;

public abstract class GatheringTask : TargettedTask
{

    GatheringStep currentStep;
    float gatherTimer;
    bool waitForMoreResourcesAtTownCenter;
    protected string targetTag;

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
        // todo calculate stopping distance based on size of resource
        if (withinDistanceOfTarget(2))
            currentStep = GatheringStep.Gathering;
    }

    void GatheringUpdate()
    {
        dontMove();
        if (target.target == null || inventory.weight >= MAX_WEIGHT)
        {
            currentStep = GatheringStep.Idle;
            return;
        }

        float gatherRate = 1 / (1 + getInteractionEquipedMultiplier(targetTag));
        gatherTimer += Time.deltaTime;
        if (gatherTimer >= gatherRate)
        {
            // todo if there are gather mulitpliers, add logic for that here
            uint totalGather = System.Convert.ToUInt32(gatherTimer / gatherRate);
            gatherTimer -= totalGather * gatherRate;

            Inventory targetInventory = target.target.transform.root.GetComponent<Inventory>();
            uint realGatherAmount = System.Math.Min(targetInventory.getTotalCountByTypes(Tags.Resources), totalGather);

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
            transferBestForTask(transferInventory, targetTag);
            equipBestForTask(targetTag);
            Item bestItemCanCraft = bestItemCanCraftBySlots(transferInventory, targetTag, inventory.getEmptySlots());
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
        if (inventory.weight >= MAX_WEIGHT)
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
