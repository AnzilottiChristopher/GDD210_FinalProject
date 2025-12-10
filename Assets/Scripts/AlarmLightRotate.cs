using UnityEngine;

public class AlarmLightRotate : MonoBehaviour
{
    public float rotationSpeed = 120f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
