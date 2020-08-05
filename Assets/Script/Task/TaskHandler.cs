using UnityEngine;

public class TaskHandler : MonoBehaviour {
    
    Task currentTask;
    public Task CurrentTask
    {
        get { return currentTask; }
    }
    Task nextTask;
    public Task NextTask
    {
        get { return nextTask; }
    }

    // Use this for initialization
    void Start () {
        foreach (Task component in GetComponents<Task>())
            if(component.enabled)
                currentTask = component;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void setTask(Tasks task)
    {

        switch (task)
        {
            case Tasks.BuildTask:
                this.nextTask = GetComponent<BuildTask>();
                break;
            case Tasks.BerryGathering:
                this.nextTask = GetComponent<BerryGatheringTask>();
                break;
            case Tasks.CraftingTask:
                this.nextTask = GetComponent<CraftingTask>();
                break;
            case Tasks.Idle:
                this.nextTask = GetComponent<IdleTask>();
                break;
            case Tasks.WoodGathering:
                this.nextTask = GetComponent<WoodGatheringTask>();
                break;
            default:
                this.nextTask = GetComponent<IdleTask>();
                break;
        }

        currentTask.completeTask();
    }

    public void pushTask(Tasks task)
    {
        Task pendingTask = null;
        switch (task)
        {
            case Tasks.BuildTask:
                pendingTask = GetComponent<BuildTask>();
                break;
            case Tasks.BerryGathering:
                pendingTask = GetComponent<BerryGatheringTask>();
                break;
            case Tasks.CraftingTask:
                pendingTask = GetComponent<CraftingTask>();
                break;
            case Tasks.Idle:
                pendingTask = GetComponent<IdleTask>();
                break;
            case Tasks.WoodGathering:
                pendingTask = GetComponent<WoodGatheringTask>();
                break;
        }
        
        if (pendingTask != null)
        {
            nextTask = currentTask;
            nextTask.enabled = false;

            currentTask = pendingTask;
            currentTask.enabled = true;
        }
    }

    public void notifyTaskCompleted()
    {
        currentTask.enabled = false;

        if (nextTask != null)
            currentTask = nextTask;
        else
            currentTask = GetComponent<IdleTask>();

        nextTask = null;
        currentTask.Start();
        currentTask.enabled = true;
    }
}
