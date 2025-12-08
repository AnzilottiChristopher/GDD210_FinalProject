using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static PlayerTracker TrackerInstance;
    [SerializeField] private GameObject player;

    [Header("AI lying Settings")]
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
        IncreaseLieProbability();
    }
    private Vector3 GetFakeLocation()
    {
        Vector3 offset = Random.insideUnitSphere * lieDistance;
        offset.y = 0;
        return player.transform.position;
    }
    private void IncreaseLieProbability()
    {
        lieProbability = Mathf.Min(maxLieProbability, lieProbability + lieIncreaseRate * Time.deltaTime);
    }
    private void resetLieProbability()
    {
       lieProbability = 0.1f; 
    }
    public GameObject GetPlayer()
    {
        return player;
    }
}
