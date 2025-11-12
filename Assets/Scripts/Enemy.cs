using System;
using UnityEngine;

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
        Dead // Killed by player
    };



    // Basic enemy stats
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int health = 3;
    [SerializeField] private float detectionRange;
    [SerializeField] private GameObject mainObj;
    [SerializeField] private GameObject eyes;
    [SerializeField] private States state;



    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainObj = this.gameObject;
        eyes = mainObj.transform.GetChild(0).gameObject;
        state = States.Idle;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if(eyesight())
        {
            Debug.Log("Hit something");
        }
    }
    
    private bool eyesight()
    {
        RaycastHit lineOfSight;
        bool hit = Physics.Raycast(eyes.transform.position, eyes.transform.right,
        out lineOfSight, detectionRange);
        if (hit)
        {
            Debug.DrawLine(eyes.transform.position, eyes.transform.position + eyes.transform.right
            * detectionRange, Color.red);
        }
        else
        {
            Debug.DrawLine(eyes.transform.position, eyes.transform.position + eyes.transform.right
            * detectionRange, Color.green);
        }
        
        return hit;
    }
}
