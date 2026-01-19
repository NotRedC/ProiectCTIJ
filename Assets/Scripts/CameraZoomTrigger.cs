using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomAmount;
    public float originalSize;

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