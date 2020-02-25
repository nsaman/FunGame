using UnityEngine;

public class TownCenterBuilder : Builder {
    
	protected override void Start () {
        base.Start();
        builds.Add(typeof(NpcBuild));
	}
    
    protected override void Update () {
        base.Update();
    }
    
    protected override void setSpawnPosition()
    {
        // todo change position if the size or look of the TC changes
        Vector3 tcPosition = GetComponent<Transform>().position;
        SpawnPosition = new Vector3(tcPosition.x + 4, tcPosition.y, tcPosition.z + 4);
    }
    

}
