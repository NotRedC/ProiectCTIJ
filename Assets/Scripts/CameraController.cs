using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Target")]
    public Transform player; // Drag your Player object here

    [Header("Screen Settings")]
    // Adjust these to match your level design grid
    public float screenWidth = 18f;
    public float screenHeight = 10f;

    // Used to center the camera if your grid starts at 0,0
    public Vector2 offset = new Vector2(0, 0);

    // Smoothing (Optional: Set to 0 for instant snap, 5-10 for fast slide)
    public float smoothSpeed = 0f;
    void LateUpdate()
    {
        if (player == null) return;

        // 1. Calculate which "Room" the player is in
        // Math.Round would round to the nearest whole number.
        // We want strict grids, so we divide position by screen size, round, then multiply back.

        float currentRoomX = Mathf.Round(player.position.x / screenWidth) * screenWidth;
        float currentRoomY = Mathf.Round(player.position.y / screenHeight) * screenHeight;

        // 2. Determine target position
        Vector3 targetPosition = new Vector3(currentRoomX + offset.x, currentRoomY + offset.y, transform.position.z);

        transform.position = targetPosition;
    }

    void OnDrawGizmos()
    {
        // Set the color of the debug box
        Gizmos.color = Color.yellow;

        // Draw a box at the camera's current position matching the screen size
        Gizmos.DrawWireCube(transform.position, new Vector3(screenWidth, screenHeight, 1));

        // Optional: Draw the center point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
