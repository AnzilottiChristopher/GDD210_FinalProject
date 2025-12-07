using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
public class GetPoint : MonoBehaviour
{
    [SerializeField] private float Range;
    [SerializeField] private float smallRange;
    [SerializeField] private float init_x;
    [SerializeField] private float init_z;
    [SerializeField] private float init_Range;
    [SerializeField] private GameObject player;
    private Vector3 playerPos;
    private Vector3 lastKnownPlayerPos;

    void Awake()
    {
        init_x = this.transform.position.x;
        init_z = this.transform.position.y;
        init_Range = Range;
        playerPos = player.transform.position;
    }
    private void Update() {
        playerPos = player.transform.position;
    }
    
    public void foundPlayer()
    {
        Range = smallRange;
        this.transform.position = playerPos;
        lastKnownPlayerPos = player.transform.position;
    }
    public void trackPlayer()
    {
        
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
#endif
}
