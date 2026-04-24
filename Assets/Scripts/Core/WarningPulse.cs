using UnityEngine;

public class WarningPulse : MonoBehaviour
{
    private void Update()
    {
        float s = 1F + Mathf.PingPong(Time.time * 5F, 0.5F);
        transform.localScale = new Vector3(s, s, 1F);
    }
}