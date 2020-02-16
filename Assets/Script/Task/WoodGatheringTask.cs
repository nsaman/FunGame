using UnityEngine;
using System.Collections.Generic;

public class WoodGatheringTask : GatheringTask
{
    // Use this for initialization
    public override void Start()
    {
        targetTag = Tags.Wood;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
