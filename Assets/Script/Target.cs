using UnityEngine;

public class Target : MonoBehaviour {

	public GameObject target;
	private MemoryEntry targetMemory;
	UnityEngine.AI.NavMeshAgent agent;

	public MemoryEntry TargetMemory { get => targetMemory; 
		set {
			if (targetMemory == null || value == null || targetMemory.InstanceID != value.InstanceID)
				target = null;
			targetMemory = value; } }

	// Use this for initialization
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 usedPosition = targetMemory != null ? TargetMemory.Position : AI.NOT_FOUND;
		if (target != null)
			usedPosition = target.transform.position;

		if (agent.isActiveAndEnabled && usedPosition != AI.NOT_FOUND && agent.destination != usedPosition)
		    agent.SetDestination(usedPosition);
	}
}
