using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Enemy States
    enum States
    {
        Idle, // Actively not doing anything
        Searching, // Looking for player after losing sight
        Chasing, // Actively going after player
        Attacking, // Attacking the player
        Investigating, // Heard noise of player and looking for area
        Stunned, // Stunned by the player momentarily
        Dead, // Killed by player
        Patrolling, // Patrolling an area
    };



    // Basic enemy stats
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int health = 3;
    [SerializeField] private float detectionRange;
    [SerializeField] private float patrolRange;
    [SerializeField] private GameObject mainObj;
    [SerializeField] private GameObject eyes;
    [SerializeField] private States state;
    private NavMeshAgent agent;
    [SerializeField] private GetPoint instance; 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        instance = transform.parent.Find("Point").GetComponent<GetPoint>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainObj = this.gameObject;
        eyes = mainObj.transform.GetChild(0).gameObject;
        state = States.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case States.Patrolling:
                if (!agent.hasPath)
                {
                    agent.SetDestination(instance.GetRandomPoint());
                }
                break;
            case States.Idle:
                break;
            case States.Searching:
                break;
            case States.Chasing:
                chasing();
                break;
            case States.Attacking:
                attacking();
                break;
            case States.Investigating:
                break;
            case States.Stunned:
                break;
            case States.Dead:
                break;
            default:
                break;
        }
    }
    void FixedUpdate()
    {
        if(eyesight())
        {
            state = States.Attacking;
            agent.ResetPath();
        }
    }

    private void chasing()
    {
        
    }
    private void attacking()
    {
        
    }
    private bool eyesight()
    {
        return false;
        // RaycastHit lineOfSight;
        // bool hit = Physics.Raycast(eyes.transform.position, eyes.transform.forward,
        // out lineOfSight, detectionRange);
        // //If statements not needed just used for debugging purposes
        // if (hit && lineOfSight.collider.CompareTag("Player"))
        // {
        //     Debug.Log("Hit player");
        //     Debug.DrawLine(eyes.transform.position, eyes.transform.position + eyes.transform.forward
        //     * detectionRange, Color.red);
        // }
        // else
        // {
        //     Debug.DrawLine(eyes.transform.position, eyes.transform.position + eyes.transform.forward
        //     * detectionRange, Color.green);
        // }

        // return hit && lineOfSight.collider != null && lineOfSight.collider.CompareTag("Player");
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
#endif
}
