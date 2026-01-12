using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform target;        // Aici vei trage Player-ul în Inspector
    public float smoothSpeed = 5f; // Cât de repede "ajunge" camera din urmă jucătorul
    public float yOffset = 2f;      // Ajustează cât de sus/jos să stea camera față de player

    private float fixedX;
    private float fixedZ;

    void Start()
    {
        // Salvăm poziția inițială pe X și Z ca să nu se modifice niciodată
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculăm doar noua poziție pe Y
            float desiredY = target.position.y + yOffset;

            // Creăm un vector nou unde X și Z rămân neschimbate
            Vector3 lerpedPosition = Vector3.Lerp(transform.position, new Vector3(fixedX, desiredY, fixedZ), smoothSpeed * Time.deltaTime);

            // Aplicăm poziția
            transform.position = lerpedPosition;
        }
    }
}