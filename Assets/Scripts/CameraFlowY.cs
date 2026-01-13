using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Tinta de urmarit")]
    public Transform target;        // Trage Player-ul aici in Inspector

    [Header("Setari Fluiditate")]
    public float smoothSpeed = 5f;  // Cat de lin se misca camera

    [Header("Offset si Limite")]
    public Vector2 offset = new Vector2(0f, 2f); // X si Y offset fata de player

    public bool useLimits = false;  // Bifeaza daca vrei sa limitezi camera pe X
    public float minX = -5f;        // Limita stanga
    public float maxX = 5f;         // Limita dreapta

    private float fixedZ;

    void Start()
    {
        // Salvam Z-ul camerei (de obicei -10) ca sa nu il pierdem
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Calculam unde ar trebui sa fie camera (Target + Offset)
            float desiredX = target.position.x + offset.x;
            float desiredY = target.position.y + offset.y;

            // 2. Daca limitarea este activata, "taiem" valorile X care depasesc limitele
            if (useLimits)
            {
                desiredX = Mathf.Clamp(desiredX, minX, maxX);
            }

            // 3. Cream vectorul destinatie final
            Vector3 targetPosition = new Vector3(desiredX, desiredY, fixedZ);

            // 4. Aplicam miscarea interpolata (Lerp) pentru fluiditate
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // 5. Mutam camera
            transform.position = smoothedPosition;
        }
    }
}