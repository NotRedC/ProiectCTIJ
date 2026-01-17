using UnityEngine;
using static Unity.VisualScripting.Member;

[RequireComponent(typeof(AudioSource))]
public class KonamiCheat : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform player;          
    [SerializeField] private Transform finalPlatform;

    [Header("Audio")]
    [SerializeField] private AudioClip sound;
    private AudioSource source;
    private float verticalOffset = 1f; 

    private KeyCode[] konamiCode = new KeyCode[] {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };

    private int currentIndex = 0;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
          
            if (Input.GetKeyDown(konamiCode[currentIndex]))
            { 
                currentIndex++;
                if (currentIndex >= konamiCode.Length)
                {
                    ActivateCheat();
                    currentIndex = 0; 
                }
            }
            else
            {
            
                currentIndex = 0;
            }
        }
    }

    void ActivateCheat()
    {
        Debug.Log("KONAMI CODE ACTIVATED!");
        if (sound != null)
        {
            source.PlayOneShot(sound);
        }
        player.position = new Vector2(finalPlatform.position.x, finalPlatform.position.y + verticalOffset);
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
        PlayerMovement p = player.GetComponent<PlayerMovement>();
        if (p != null) p.SetReversedControls(false); 
    }
}