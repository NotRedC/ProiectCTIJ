using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Target")]
    public Transform player;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;   
    private float targetSize;
    private Camera cam;

    [Header("Screen Settings")]
    private float screenWidth = 16f;
    private float screenHeight = 10f;
    public Vector2 offset = new Vector2(0f, 5f);

    private Vector2 gridOrigin = new Vector2(5f, 0.83f);

    void Start()
    {
        cam = GetComponent<Camera>();
        targetSize = cam.orthographicSize;
       
    }

    public float smoothSpeed = 0f;
    void LateUpdate()
    {
        if (player == null) return;

        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        }

        float rawX = player.position.x;
        float rawY = player.position.y;

        float currentRoomX = Mathf.Round(rawX / screenWidth) * screenWidth;
        float currentRoomY = Mathf.Floor(rawY / screenHeight) * screenHeight;

        Vector3 targetPosition = new Vector3(currentRoomX + gridOrigin.x + offset.x, currentRoomY + gridOrigin.y + offset.y, transform.position.z);

        if (smoothSpeed > 0)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(screenWidth, screenHeight, 1));

    }

    public void SetZoom(float newSize, float speed)
    {
        targetSize = newSize;
        smoothSpeed = speed;
    }
}
