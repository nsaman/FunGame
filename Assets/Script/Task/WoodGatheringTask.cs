using System.Collections.Generic;

public class WoodGatheringTask : GatheringTask
{
    // Use this for initialization
    public override void Start()
    {
        targetTag = Tags.Tree;
        usedSkill = Skills.WoodChopping;
        trainedSkills = new List<Skill>() { Skills.WoodChopping, Skills.Strength };
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
