using UnityEditor;
using UnityEngine;


public enum TauntType
{
    Taunt,
    Hint,
    Story
}

public class TauntTrigger : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    private string[] messages;
    [SerializeField] private TauntType type;
    [SerializeField] private bool triggerOnce;
    [SerializeField] private float minFallSpeed;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            Rigidbody2D body =other.GetComponent<Rigidbody2D>();
            if (body == null)
            {
                return;
            }
            if(type == TauntType.Story)
            {
                if (body.linearVelocity.y < minFallSpeed) return;
            }
            else if (type == TauntType.Taunt)
            {
                if (body.linearVelocityY > minFallSpeed) return;
            }

            string randomMsg = messages[Random.Range(0, messages.Length)];
            TauntManager.Instance.ShowMessage(randomMsg, type);
            hasTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (type == TauntType.Hint && other.CompareTag("Player"))
        {
            TauntManager.Instance.HideMessage();
        }
    }
}