using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


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
    [SerializeField] private float investigateTime = 4f;
    private float investigateTimer;
    private NavMeshAgent agent;
    [SerializeField] private GetPoint instance; 
    [SerializeField] private FieldOfView fov;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        instance = transform.parent.Find("Point").GetComponent<GetPoint>();
        mainObj = this.gameObject;
        eyes = mainObj.transform.GetChild(0).gameObject;
        fov = eyes.GetComponent<FieldOfView>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
                searchArea();
                if(!agent.hasPath)
                {
                    agent.SetDestination(instance.GetRandomPoint());
                }
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
        bool seesPlayer = fov.getVisibleTargets().Count > 0;
        if(seesPlayer)
        {
            state = States.Chasing;
            instance.updateLastKnownPlayerPos();
            
        }
        else if(state == States.Chasing)
        {
            state = States.Searching;
            investigateTimer = investigateTime;
            instance.lostPlayer();
        }
    }
    private void searchArea()
    {
        investigateTimer -= Time.deltaTime;

        if(investigateTimer <= 0f)
        {
            instance.resetPointToOrigin();
            state = States.Patrolling;
            agent.ResetPath();
        }
    }
    private void chasing()
    {
        agent.speed = speed * 1.5f;
        agent.SetDestination(instance.getPlayerPos());
    }
    private void attacking()
    {
        
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
#endif
}
