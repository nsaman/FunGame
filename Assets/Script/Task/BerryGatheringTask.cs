using System.Collections.Generic;

public class BerryGatheringTask : GatheringTask
{
    // Use this for initialization
    public override void Start()
    {
        targetTag = Tags.Berries;
        usedSkill = Skills.BerryGathering;
        trainedSkills = new List<Skill>(){ Skills.BerryGathering };
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
