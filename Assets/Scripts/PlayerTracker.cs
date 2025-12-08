using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static PlayerTracker TrackerInstance;
    [SerializeField] private GameObject player;

    [Header("AI Hint Settings")]
    [SerializeField] private float hintInterval = 3f; // How often to give hints
    private float hintTimer = 0f;
    
    [Header("AI Lying Settings")]
    [SerializeField] private float lieProbability = 0.1f;
    [SerializeField] private float lieIncreaseRate = 0.01f;
    [SerializeField] private float maxLieProbability = 0.5f;
    [SerializeField] private float lieDistance = 5f;

    private void Awake() {
        if(TrackerInstance == null) TrackerInstance = this;
        else Destroy(gameObject);

        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        hintTimer -= Time.deltaTime;
        
        // Gradually increase lie probability over time
        IncreaseLieProbability();
    }

    // Call this during patrolling to get periodic hints
    public bool ShouldGiveHint()
    {
        if(hintTimer <= 0f)
        {
            hintTimer = hintInterval;
            return true;
        }
        return false;
    }

    // Gentle hint update - moves the point but doesn't shrink range
    public void GiveHintToEnemy(GetPoint point)
    {
        Vector3 hintPos;

        if(Random.value < lieProbability)
        {
            hintPos = GetFakeLocation();
        }
        else
        {
            hintPos = player.transform.position;
        }
        
        // Move the point to the hint position (correct or lie)
        point.updateHintPosition(hintPos);
        
        // Reset probability after giving a hint
        ResetLieProbability();
    }

    // Direct update when enemy actually detects player
    public void UpdatePointLocation(GetPoint point)
    {
        Vector3 newPos;

        if(Random.value < lieProbability)
        {
            newPos = GetFakeLocation();
            point.updateFakePlayerPos(newPos);
        }
        else
        {
            point.updateLastKnownPlayerPos();
        }
        point.respondToAlert();
        
        // Reset probability after giving a hint
        ResetLieProbability();
    }

    private Vector3 GetFakeLocation()
    {
        Vector3 offset = Random.insideUnitSphere * lieDistance;
        offset.y = 0;
        return player.transform.position + offset; // Fixed: actually add the offset!
    }

    private void IncreaseLieProbability()
    {
        lieProbability = Mathf.Min(maxLieProbability, lieProbability + lieIncreaseRate);
    }

    public void ResetLieProbability()
    {
       lieProbability = 0.1f; 
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}