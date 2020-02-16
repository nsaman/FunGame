using UnityEngine;
using System.Collections.Generic;

public abstract class Task : MonoBehaviour
{

    protected bool completingTask;
    protected TaskHandler taskHandler;
    public const uint MAX_WEIGHT = 2;

    public virtual void Start()
    {
        completingTask = false;
        taskHandler = GetComponent<TaskHandler>();
    }

    protected virtual void Update()
    {
    }

    public abstract void completeTask();

    protected void dontMove()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }
    protected void move()
    {
        GetComponent<NavMeshAgent>().enabled = true;
    }

    protected Transform findClosestByTag(string tag)
    {
        return findClosestByTags(new string[] { tag }, transform);
    }

    protected Transform findClosestByTags(string[] tags)
    {
        return findClosestByTags(tags, transform);
    }

    public static Transform findClosestByTags(string[] tags, Transform startingTransform)
    {

        HashSet<GameObject> tagObjects = new HashSet<GameObject>();

        foreach (string tag in tags)
        {
            tagObjects.UnionWith(GameObject.FindGameObjectsWithTag(tag));
        }

        return findClosest(tagObjects, startingTransform);
    }

    public Transform findClosest(ICollection<GameObject> tagObjects)
    {
        return Task.findClosest(tagObjects, transform);
    }

    public static Transform findClosest(ICollection<GameObject> tagObjects, Transform startingTransform)
    {
        Transform closest = null;
        float distanceToClosest = float.MaxValue;

        foreach (GameObject gameobject in tagObjects)
        {
            Transform newTransform = gameobject.GetComponent<Transform>();
            float distanceToNew = Vector3.Distance(newTransform.position, startingTransform.position);
            if (distanceToNew < distanceToClosest)
            {
                closest = newTransform;
                distanceToClosest = distanceToNew;
            }
        }

        return closest;
    }

    public Transform findClosestTeamOrNeutralObjectsWithTag(string tag)
    {
        return (findClosest(getTeamOrNeutralObjectsWithTags(new string[] { tag }), transform));
    }

    public Transform findClosestTeamOrNeutralObjectsWithTags(string[] tags)
    {
        return (findClosest(getTeamOrNeutralObjectsWithTags(tags), transform));
    }

    public HashSet<GameObject> getTeamOrNeutralObjectsWithTags(string[] tags)
    {
        HashSet<GameObject> returnObjects = new HashSet<GameObject>();

        foreach (string tag in tags)
            returnObjects.UnionWith(getTeamOrNeutralObjectsWithTag(tag));

        return returnObjects;
    }

    public HashSet<GameObject> getTeamOrNeutralObjectsWithTag(string tag)
    {
        HashSet<GameObject> returnObjects = new HashSet<GameObject>();

        GameObject[] tagObjects = GameObject.FindGameObjectsWithTag(tag);
        
        foreach(GameObject taggedObject in tagObjects)
        {
            if (taggedObject.transform.root.GetComponent<TeamPointer>() == null)
                returnObjects.Add(taggedObject);
            else if (taggedObject.transform.root.GetComponent<TeamPointer>().TeamController == gameObject.transform.root.GetComponent<TeamPointer>().TeamController)
                returnObjects.Add(taggedObject);
        }

        return returnObjects;
    }

}
