using UnityEngine;
using System.Collections;

public class IdleTask : Task
{

    // Use this for initialization
    public new void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	new void Update () {
        dontMove();
    }

    override public void completeTask()
    {
         taskHandler.notifyTaskCompleted();
    }
}
