public class WoodGatheringTask : GatheringTask
{
    // Use this for initialization
    public override void Start()
    {
        targetTag = Tags.Tree;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
