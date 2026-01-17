using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomAmount; // Bigger number = Zoom OUT
    public float originalSize; // Return to this when leaving

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraController>().SetZoom(zoomAmount, 5f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraController>().SetZoom(originalSize, 0f);
        }
    }
}