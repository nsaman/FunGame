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
        currentTask = GetComponent<IdleTask>();
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
