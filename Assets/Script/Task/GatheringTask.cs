using UnityEngine;
using System.Collections.Generic;

public abstract class GatheringTask : Task
{
    // todo create parent class for handling job transitions, tracking skills, and registering target

    GatheringStep currentStep;
    Target target;
    float gatherTimer;
    bool waitForMoreResourcesAtTownCenter;
    protected string targetTag;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        // todo check which state to intialize to

        target = GetComponent<Target>();
        currentStep = GatheringStep.Idle;
        gatherTimer = 0;
        waitForMoreResourcesAtTownCenter = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (target.target == null)
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
        if (target.target == null)
        {
            taskHandler.notifyTaskCompleted();
        }
    }

    void MovingToResourceUpdate()
    {
        float distanceToResource = Vector3.Distance(target.target.position, transform.position);

        // todo calculate stopping distance based on size of resource
        if (distanceToResource < 2)
            currentStep = GatheringStep.Gathering;
    }

    void GatheringUpdate()
    {
        Inventory thisInventory = gameObject.GetComponent<Inventory>();

        if (thisInventory.weight >= MAX_WEIGHT)
            currentStep = GatheringStep.Idle;

        // todo add gathering rate impact by tools/skill
        float gatherRate = 1;
        gatherTimer += Time.deltaTime;
        if (gatherTimer >= gatherRate)
        {
            // todo if there are gather mulitpliers, add logic for that here
            uint totalGather = System.Convert.ToUInt32(gatherTimer / gatherRate);
            gatherTimer -= totalGather * gatherRate;

            Inventory targetInventory = target.target.root.GetComponent<Inventory>();
            uint realGatherAmount = System.Math.Min(targetInventory.getTotalCountByTypes(Tags.Resources), totalGather);

            Inventory.transferTypes(targetInventory, thisInventory, Tags.Resources, realGatherAmount);

            if (targetInventory.getTotalCountByTypes(Tags.Resources) <= 0)
            {
                Destroy(target.target.root.gameObject);
                currentStep = GatheringStep.Idle;
            }
        }
    }

    void DroppingOffResourcesUpdate()
    {
        float distanceDropOff = Vector3.Distance(target.target.position, transform.position);

        // todo calculate stopping distance based on size of drop point
        if (distanceDropOff <= 3)
        {
            Inventory.transferAllOfTypes(gameObject.GetComponent<Inventory>(), target.target.root.gameObject.GetComponent<Inventory>(), Tags.Resources);
            if (completingTask)
            {
                taskHandler.notifyTaskCompleted();
                return;
            }
            currentStep = GatheringStep.Idle;
        }
    }

    void IdleUpdate()
    {
        dontMove();

        Inventory inventory = gameObject.GetComponent<Inventory>();

        if (inventory.weight == 0)
        {
            target.target = findClosestByTag(targetTag);
            GetComponent<NavMeshAgent>().enabled = true;
            if (target.target != null)
            {
                currentStep = GatheringStep.MovingToResource;
            }
            // no resource to gather
            else
            {
                if (!waitForMoreResourcesAtTownCenter)
                {
                    dropOffTownCenter();
                    // if we are too close to the TC, just idle
                    if (target.target == null || Vector3.Distance(target.target.position, transform.position) < 3)
                    {
                        taskHandler.setTask(Tasks.Idle);
                    }
                }
            }
        }
        else if (inventory.weight >= MAX_WEIGHT)
        {
            dropOffTownCenter();
        }
        else
        {
            target.target = findClosestTeamOrNeutralObjectsWithTags(new string[] { targetTag, Tags.TownCenterDropOff });
            GetComponent<NavMeshAgent>().enabled = true;
            if (target.target.gameObject.name.Equals(targetTag))
            {
                currentStep = GatheringStep.MovingToResource;
            }
            else
            {
                currentStep = GatheringStep.DroppingOffResources;
            }
        }
    }

    private void dropOffTownCenter()
    {
        target.target = findClosestTeamOrNeutralObjectsWithTag(Tags.TownCenterDropOff);
        GetComponent<NavMeshAgent>().enabled = true;
        if (target.target != null)
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
