using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Target")]
    public Transform player; // Drag your Player object here

    [Header("Zoom Settings")]
    public float defaultSize; // Your normal game zoom
    public float zoomSpeed = 2f;   // How fast it zooms in/out
    private float targetSize;
    private Camera cam;

    [Header("Screen Settings")]
    // Adjust these to match your level design grid
    public float screenWidth = 18f;
    public float screenHeight = 10f;
    public Vector2 offset = new Vector2(0, 0);

    void Start()
    {
        cam = GetComponent<Camera>();
        targetSize = cam.orthographicSize;
        defaultSize = cam.orthographicSize;
        //RecalculateGrid();
    }

    public float smoothSpeed = 0f;
    void LateUpdate()
    {
        if (player == null) return;

        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        }

        float currentRoomX = Mathf.Round(player.position.x / screenWidth) * screenWidth;
        float currentRoomY = Mathf.Round(player.position.y / screenHeight) * screenHeight;

       
        Vector3 targetPosition = new Vector3(currentRoomX + offset.x, currentRoomY + offset.y, transform.position.z);

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
