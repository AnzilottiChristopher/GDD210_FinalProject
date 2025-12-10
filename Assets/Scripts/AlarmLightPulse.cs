using UnityEngine;

public class AlarmLightPulse : MonoBehaviour
{
    [SerializeField] private Light alarmLight;
    [SerializeField] private float maxIntensity = 8f;
    [SerializeField] private float pulseSpeed = 5f;
    void Update()
    {
        float pulse = Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
        alarmLight.intensity = pulse * maxIntensity;
    }
}
