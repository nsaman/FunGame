using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Observes : MonoBehaviour
{
    public const float OBERVATION_DELAY = 2f;
    public const float OBERVATION_RADIUS = 20f;
    public static Vector3 EYE_LEVEL = new Vector3(0, .8f, .52f);
    public static float FORWARD = 0f;

    private float observationTimer;
    private Remembers remembers;
    private Target target;
    private float lookAngle = FORWARD;
    public float LookAngle { get => lookAngle; }
    RandomSingleton rnd = RandomSingleton.Instance;
    private GameObject obscuringObject;
    private Vector3 obscuredPosition;

    // Start is called before the first frame update
    void Start()
    {
        remembers = transform.root.gameObject.GetComponent<Remembers>();
        target = transform.root.gameObject.GetComponent<Target>();
        resetObscured();
    }

    // Update is called once per frame
    void Update()
    {
        if (target.target != null || target.TargetMemory != null)
        {
            Vector3 lookingAtPosition = target.target != null ? target.target.transform.position : target.TargetMemory.Position;
            Vector3 facingDirection = transform.root.rotation * new Vector3(0f, 0f, 1f);
            lookAngle = Vector2.SignedAngle(new Vector2(lookingAtPosition.x, lookingAtPosition.z) - new Vector2(transform.root.position.x, transform.root.position.z), new Vector2(facingDirection.x, facingDirection.z));
            if (lookAngle > 90)
                lookAngle = 90;
            else if (lookAngle < -90)
                lookAngle = -90;
        } else
        {
            lookAngle = FORWARD;
        }

        observationTimer += Time.deltaTime;

        if (observationTimer >= OBERVATION_DELAY)
        {
            observationTimer = (float)rnd.NextDouble() * OBERVATION_DELAY;

            Quaternion lookDirection = Quaternion.AngleAxis(lookAngle, Vector3.up) * transform.rotation;
            Vector3 eyePosition = transform.position + lookDirection * EYE_LEVEL;
            Vector3 searchPosition = eyePosition + lookDirection * new Vector3(0, 0, OBERVATION_RADIUS);

            // try looking at the target
            if (target != null)
            {
                // if the target has been set, try tracking it
                if (target.target != null)
                {
                    // if we can see the target, track it
                    if (Physics.Raycast(eyePosition, target.target.transform.position + new Vector3(0, .1f, 0) - eyePosition, out RaycastHit targetHit, OBERVATION_RADIUS * 4) && targetHit.collider.transform.root.gameObject.GetInstanceID() == target.target.GetInstanceID())
                    {
                        remembers.Remember(target.target);
                        resetObscured();
                    }
                }
                // if we only have the memory, try looking for it  
                else if (target.TargetMemory != null)
                {
                    // if we can see something, check if it's the target. if not make sure to have logic to forget
                    if (Physics.Raycast(eyePosition, target.TargetMemory.Position + new Vector3(0, .1f, 0) - eyePosition, out RaycastHit memoryHit, OBERVATION_RADIUS * 4))
                    {
                        GameObject collidedObject = memoryHit.collider.transform.root.gameObject;
                        if (collidedObject.GetInstanceID() == target.TargetMemory.InstanceID)
                        {
                            remembers.Remember(target.TargetMemory);
                            target.target = collidedObject;
                            resetObscured();
                        }
                        // we can't see our memory! make sure to have a way to forget if we get stuck
                        // if what we're seeing cannot move, remember that and forget about the target next time if we can't get any closer
                        else if(!Array.Exists(Tags.Moves, x => collidedObject.CompareTag(x)))
                        {
                            if (obscuringObject == collidedObject && Vector3.Distance(transform.position, obscuredPosition) < .2)
                            {
                                remembers.Forget(target.TargetMemory);
                                resetObscured();
                            }
                            else
                            {
                                obscuringObject = collidedObject;
                                obscuredPosition = transform.position;
                            }
                        }
                    }
                }
            }

            HashSet<MemoryEntry> expectedObserved = remembers.expectedObeserves(searchPosition, OBERVATION_RADIUS);
            expectedObserved.RemoveWhere(memory => {
                Vector3 slightlyAboveTheGroundPosition = memory.Position + new Vector3(0, .1f, 0);

                // this code is here as I am getting a raycastHit collider hit but the actual Raycast() is returning false. I'm not sure how that's possible
                return Physics.Raycast(eyePosition, slightlyAboveTheGroundPosition - eyePosition, Vector3.Distance(slightlyAboveTheGroundPosition, eyePosition));
            });
            remembers.ForgetAll(expectedObserved);

            if(target != null && (target.target != null || target.TargetMemory != null))
            {
                foreach(MemoryEntry memory in expectedObserved)
                {
                    bool breakFlag = false;

                    if (target.target != null && target.target.GetInstanceID() == memory.InstanceID)
                        target.target = null;
                    if (target.TargetMemory != null && target.TargetMemory.InstanceID == memory.InstanceID)
                        target.TargetMemory = null;
                    if (breakFlag)
                        break;
                }
            }

            Collider[] collisions = Physics.OverlapSphere(searchPosition, OBERVATION_RADIUS);
            HashSet<GameObject> alreadyViewed = new HashSet<GameObject>();
            HashSet<GameObject> seenObjects = new HashSet<GameObject>(collisions.Where(x =>
            {
                Transform rootTransform = x.gameObject.transform.root;
                GameObject rootObject = rootTransform.gameObject;

                if (Tags.DontRemember.Contains(rootObject.tag) || alreadyViewed.Contains(rootObject))
                    return false;
                bool rayCollided = Physics.Raycast(eyePosition, x.transform.position - eyePosition, out RaycastHit raycastHit, Vector3.Distance(x.transform.position, eyePosition));
                if (rayCollided && raycastHit.collider.gameObject.transform.root == rootTransform)
                {
                    alreadyViewed.Add(rootObject);
                    return true;
                }
                return false;
            }).Select(x => x.gameObject.transform.root.gameObject).ToList());

            remembers.RememberAll(seenObjects);

            if(target != null && target.target == null && target.TargetMemory != null)
            {
                foreach (GameObject seenObject in seenObjects)
                {
                    if (target.TargetMemory.InstanceID == seenObject.GetInstanceID())
                    {
                        target.target = seenObject;
                        break;
                    }
                }
            }
        }
    }

    private void resetObscured()
    {
        obscuringObject = null;
        obscuredPosition = Vector3.positiveInfinity;
    }
}
 