using UnityEngine;

public class ReverseControlZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Turn ON the curse
            other.GetComponent<PlayerMovement>().SetReversedControls(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Turn OFF the curse
            other.GetComponent<PlayerMovement>().SetReversedControls(false);
        }
    }
}