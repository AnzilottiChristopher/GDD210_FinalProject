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
    [Header("Basic Enemy Stats")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int health = 3;
    [SerializeField] private float detectionRange;
    [SerializeField] private float patrolRange;
    [SerializeField] private GameObject mainObj;
    [SerializeField] private GameObject eyes;
    [SerializeField] private States state;
    [SerializeField] private Vector3 spawnPoint;
    
    [Header("How much time the AI spends looking")]
    [SerializeField] private float investigateTime = 4f;
    private float investigateTimer;
    
    [Header("What the AI listens for")]
    [SerializeField] private float hearingRange = 8f;
    [SerializeField] private LayerMask playerMask;
    private NavMeshAgent agent;
    private bool isHearing = false;
    
    [Header("Attacking Range")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f; //This is if player can survive multiple hits
    private float attackTimer = 0f;
    
    [Header("References to Other Objects")]
    [SerializeField] private GetPoint instance; 
    [SerializeField] private FieldOfView fov;

    [Header("Stuck / Respawn Settings")]
    [SerializeField] private float stuckThreshold = 0.1f;
    [SerializeField] private float stuckTime = 5f;
    private Vector3 lastPosition;
    private float stuckTimer;
    
    [Header("Flashlight Interaction")]
    [SerializeField] private float flashlightKillTime = 3f;
    [SerializeField] private float respawnDelay = 5f;
    private float flashlightTimer = 0f;
    private bool isDead = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private AudioClip chaseSound; // Sound to play when chasing the player

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        instance = transform.parent?.Find("Point")?.GetComponent<GetPoint>();
        if(instance == null)
        {
            instance = transform.Find("Point")?.GetComponent<GetPoint>();
        }
        mainObj = this.gameObject;
        eyes = mainObj.transform.Find("Body").GetChild(0).gameObject;
        fov = eyes.GetComponent<FieldOfView>();
        
        if(instance == null)
        {
            Debug.LogError("GetPoint not found");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = this.transform.position;
        state = States.Patrolling;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case States.Patrolling:
                agent.speed = speed;
                
                if(PlayerTracker.TrackerInstance.ShouldGiveHint())
                {
                    PlayerTracker.TrackerInstance.GiveHintToEnemy(instance);
                }
                if(!agent.hasPath)
                {
                    agent.SetDestination(instance.GetRandomPoint());
                }
                break;
            case States.Idle:
                break;
            case States.Searching:
                _timer();
                agent.speed = speed * 1.2f;
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
                agent.speed = speed * 0.5f;
                _timer();
                if(!agent.hasPath)
                {
                    agent.SetDestination(instance.GetRandomPoint());
                }
                break;
            case States.Stunned:
                break;
            case States.Dead:
                break;
            default:
                break;
        }
        CheckIfStuck();
        CheckFlashlight();
    }
    void FixedUpdate()
    {
        bool seesPlayer = fov.getVisibleTargets().Count > 0;
        if (seesPlayer)
        {
            state = States.Chasing;
            instance.updateLastKnownPlayerPos();

            // Play the chase sound if not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.clip = chaseSound;
                audioSource.Play();
            }
        }
        else if (state == States.Chasing)
        {
            state = States.Searching;
            investigateTimer = investigateTime;
            instance.respondToAlert();

            // Stop the chase sound when the enemy stops chasing
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else
        {
            CheckHearing();
        }
    }
    private void CheckFlashlight()
    {
        if (isDead) return; // don't do anything if dead

        // Find the player flashlight (assign via inspector or find tag)
        GameObject player = GameObject.FindWithTag("Player");
        if(player == null) return;

        Light flashlight = player.GetComponentInChildren<Light>();
        if(flashlight == null || !flashlight.enabled) 
        {
            flashlightTimer = 0f; // reset timer if flashlight off
            return;
        }

        // Check if enemy is inside the flashlight cone
        Vector3 directionToEnemy = (transform.position - flashlight.transform.position).normalized;
        float angle = Vector3.Angle(flashlight.transform.forward, directionToEnemy);

        // Use some range (spotAngle / 2) to detect if enemy is in cone
        if(angle < flashlight.spotAngle / 2f && Vector3.Distance(transform.position, flashlight.transform.position) < flashlight.range)
        {
            flashlightTimer += Time.deltaTime;

            if(flashlightTimer >= flashlightKillTime)
            {
                KillEnemy();
            }
        }
        else
        {
            flashlightTimer = 0f;
        }
    }
    private void KillEnemy()
    {
        if (isDead) return;

        isDead = true;
        state = States.Dead;
        agent.isStopped = true;
        agent.ResetPath();
        
        StartCoroutine(RespawnEnemy());
    }
    private System.Collections.IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(respawnDelay);

        mainObj.SetActive(true);
        transform.position = spawnPoint; // or entrance position if different
        agent.Warp(spawnPoint);
        state = States.Patrolling;
        agent.isStopped = false;
        isDead = false;
        flashlightTimer = 0f;
    }
    private void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if(distanceMoved < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if(stuckTimer >= stuckTime)
            {
                // Respawn at spawn point
                agent.Warp(spawnPoint); // instantly moves NavMeshAgent to spawn
                agent.ResetPath();
                state = States.Patrolling;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f; // reset timer if moving
        }

        lastPosition = transform.position;
    }
    private void CheckHearing()
    {
        isHearing = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, hearingRange, playerMask);

        if(hits.Length > 0)
        {
            Player player = hits[0].GetComponent<Player>();
            if(player.isMoving && !player.isCrouching && state != States.Chasing)
            {
                isHearing = true;
                instance.updateLastKnownPlayerPos();
                state = States.Investigating;
                investigateTimer = investigateTime;
                instance.respondToAlert();
                if(agent.hasPath)
                    agent.ResetPath();
            }
        }
    }
    private void _timer()
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
        
        float distanceToPlayer = Vector3.Distance(transform.position, instance.getPlayerPos());
        if(distanceToPlayer <= attackRange)
        {
            state = States.Attacking;
            agent.ResetPath();
        }
    }
    private void attacking()
    {
        // Make enemy face the player
        Vector3 direction = (instance.getPlayerPos() - transform.position).normalized;
        direction.y = 0; // keep only horizontal rotation
        if(direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        // Countdown timer for attack cooldown
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            // Attack logic here
            // e.g., reduce player health, play attack animation, sound, etc.
            Player player = instance.getPlayerPos() != null ? instance.getPlayerObj().GetComponent<Player>() : null;
            if(player != null)
            {
                // player.TakeDamage(1);
                GameManager.Manager.LoseGame();
            }

            attackTimer = attackCooldown; // reset cooldown
    }

        float distanceToPlayer = Vector3.Distance(transform.position, instance.getPlayerPos());
        if(distanceToPlayer > attackRange)
        {
            state = States.Chasing;
        }
    }
    

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
        
        if(isHearing)
        {
            Gizmos.color = Color.red;
        } else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
#endif
}
