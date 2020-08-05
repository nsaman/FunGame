using UnityEngine;

public abstract class TargettedTask : Task
{
    const float NULL_DISTANCE = -1;

    protected Target target;
    public override void Start()
    {
        base.Start();

        target = GetComponent<Target>();
    }

    protected bool withinDistanceOfTarget(float distance)
    {
        return target.target != null && Vector3.Distance(target.target.transform.root.position, transform.position) <= distance;
    }
}
