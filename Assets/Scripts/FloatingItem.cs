using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    private float amplitude = 0.5f; 
    private float frequency = 1f;   

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}