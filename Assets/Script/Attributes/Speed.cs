using System;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Speed : MonoBehaviour
{
    public const float BASE_SPEED = 5;

    [SerializeField] float rate;
    Inventory inventory;
    SkillsController skillsController;
    NavMeshAgent navMeshAgent;
    FirstPersonController firstPersonController;

    public float Rate { get => rate; }

    void Start()
    {
        rate = BASE_SPEED;

        inventory = transform.root.GetComponent<Inventory>();
        skillsController = transform.root.GetComponent<SkillsController>();
        navMeshAgent = transform.root.GetComponent<NavMeshAgent>();
        firstPersonController = transform.root.GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (inventory != null && skillsController != null)
            rate = Math.Min(BASE_SPEED, BASE_SPEED / (Math.Max(inventory.weight, 1)/skillsController.getLevel(Skills.Strength)));

        if (navMeshAgent != null)
            navMeshAgent.speed = rate;
        if (firstPersonController != null)
            firstPersonController.WalkSpeed = rate;
    }
}
