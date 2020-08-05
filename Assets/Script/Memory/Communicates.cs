using System.Collections.Generic;
using UnityEngine;

public class Communicates : MonoBehaviour
{
    public const float COMMUNICATION_DELAY = 2f;
    public const float COMMUNICATION_RADIUS = 10f;

    private float communicationTimer = COMMUNICATION_DELAY;
    private Remembers remembers;
    private TeamPointer teamPointer;
    RandomSingleton rnd = RandomSingleton.Instance;

    // Start is called before the first frame update
    void Start()
    {
        remembers = transform.root.gameObject.GetComponent<Remembers>();
        teamPointer = transform.root.gameObject.GetComponent<TeamPointer>();
    }

    // Update is called once per frame
    void Update()
    {
        communicationTimer += Time.deltaTime;

        if (communicationTimer >= COMMUNICATION_DELAY)
        {
            communicationTimer -= COMMUNICATION_DELAY;
            communicationTimer += (float)rnd.NextDouble() * COMMUNICATION_DELAY;

            HashSet<GameObject> completedRootObjects = new HashSet<GameObject>();
            Collider[] collisions = Physics.OverlapSphere(transform.root.position, COMMUNICATION_RADIUS);
            foreach(Collider collider in collisions)
            {
                GameObject gameObject = collider.transform.root.gameObject;
                if (!completedRootObjects.Contains(gameObject))
                {
                    completedRootObjects.Add(gameObject);
                    Remembers remembers = gameObject.GetComponent<Remembers>();
                    TeamPointer teamPointer = gameObject.GetComponent<TeamPointer>();
                    if (remembers != null && teamPointer != null && this.teamPointer.TeamController == teamPointer.TeamController)
                    {
                        remembers.RememberAll(this.remembers);
                        remembers.ForgetAll(this.remembers);
                    }
                }
            }
        }
    }
}
