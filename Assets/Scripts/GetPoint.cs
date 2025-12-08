using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class GetPoint : MonoBehaviour
{
    [SerializeField] private float Range;
    [SerializeField] private float smallRange;
    [SerializeField] private Vector3 initLocation;
    [SerializeField] private float init_Range;
    [SerializeField] private GameObject player;
    private Vector3 playerPos;
    private Vector3 lastKnownPlayerPos;
    private Vector3 hintPosition; // Director AI hint position

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        init_Range = Range;
        initLocation = this.transform.position;
        playerPos = player.transform.position;
        hintPosition = initLocation;
    }
    
    private void Update() {
        playerPos = player.transform.position;
    }
    
    public void updateLastKnownPlayerPos()
    {
        lastKnownPlayerPos = playerPos;
    }
    
    public void updateFakePlayerPos(Vector3 fakePos)
    {
        lastKnownPlayerPos = fakePos;
    }
    
    // New: subtle hint update that doesn't trigger alert
    public void updateHintPosition(Vector3 hint)
    {
        hintPosition = hint;
        this.transform.position = hint;
    }
    
    public Vector3 getLastKnownPlayerPos()
    {
        return lastKnownPlayerPos;
    }
    
    public Vector3 getPlayerPos()
    {
        return playerPos;
    }
    
    public Vector3 getHintPosition()
    {
        return hintPosition;
    }
    
    public void respondToAlert()
    {
        Range = smallRange;
        this.transform.position = lastKnownPlayerPos;
    }
    
    public void resetPointToOrigin()
    {
        this.transform.position = initLocation;
        Range = init_Range;
        hintPosition = initLocation;
    }
    
    public GameObject getPlayerObj()
    {
        return player;
    }
    
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public Vector3 GetRandomPoint(Transform point = null, float radius = 0)
    {
        Vector3 _point;

        if (RandomPoint(point == null ? transform.position : point.position, radius == 0 ? Range : radius, out _point))
        {
            Debug.DrawRay(_point, Vector3.up, Color.black, 1);
            return _point;
        }
        return point == null? Vector3.zero : point.position;
    }
    
    // Get a random point biased toward the hint position
    public Vector3 GetRandomPointNearHint(float biasStrength = 0.5f)
    {
        Vector3 center = Vector3.Lerp(transform.position, hintPosition, biasStrength);

        Vector3 _point;
        if(RandomPoint(center, Range, out _point))
        {
            Debug.DrawRay(_point, Vector3.up, Color.magenta, 1);
            return _point;
        }
        return transform.position;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
        
        // Show hint position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hintPosition, 0.5f);
    }
#endif
}