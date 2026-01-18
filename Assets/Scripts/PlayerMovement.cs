using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip grassSound;
    [SerializeField] private AudioClip snowSound;

    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float jumpCooldown;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private string iceMaterialName = "IceMaterial"; // Numele materialului creat de tine

    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer sprite;

    private bool isGrounded;
    private bool isOnIce = false; // variabila noua
    private bool isCharging;
    private bool isDashing;
    private bool isControlsReversed = false;
    private bool hasMovedInAir;
    private float chargeTimer;
    private bool isRecovering = false;
    private string surfaceTag;

    private float lastNonZeroDir = 1;
    private float timeSinceLastInput;
    private float inputMemoryTime = 0.1f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // INITIALIZARE
        sprite = GetComponent<SpriteRenderer>(); // INITIALIZARE
    }

    void Update()
    {
        if (isDashing) return;
        CheckGround();
        float currentInput = GetHorizontalInput();

        // GESTIONARE FLIP (STANGA/DREAPTA)
        if (currentInput > 0) sprite.flipX = true;
        else if (currentInput < 0) sprite.flipX = false;

        if (currentInput != 0)
        {
            lastNonZeroDir = currentInput;
            timeSinceLastInput = 0;
        }
        else
        {
            timeSinceLastInput += Time.deltaTime;
        }

        HandleInput();
        UpdateAnimations(currentInput); // FUNCTIE NOUA
    }

    void UpdateAnimations(float input)
    {
        if (anim == null) return;

        // Trimitem datele catre Animator
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCharging", isCharging);

        // Viteza orizontala pentru animatia de Walk
        anim.SetFloat("Speed", Mathf.Abs(input));

        // Viteza pe verticala (pentru a sti daca urcam sau cadem)
        anim.SetFloat("yVelocity", body.linearVelocity.y);
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        Collider2D hit = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        isGrounded = hit != null;

        if (isGrounded)
        {
            surfaceTag = hit.tag;
            isOnIce = hit.CompareTag("Ice");
        }
        else
        {
            isOnIce = false;
        }

        if (isGrounded && !wasGrounded)
        {
            hasMovedInAir = false;
            body.linearVelocity = Vector2.zero;

            StopCoroutine(RecoverFromJump());
            StartCoroutine(RecoverFromJump());

            PlayLandingSound(surfaceTag);
        }
    }

    void HandleInput()
    {
        if (isRecovering) return;

        if (isGrounded)
        {
            if (!isOnIce)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    isCharging = true;
                    chargeTimer += Time.deltaTime;
                    body.linearVelocity = Vector2.zero;
                    //Debug.Log("Charging: " + chargeTimer);
                }
            }
            else { }

            if (!isOnIce && Input.GetKeyUp(KeyCode.Space) && isCharging)
            {
                Launch();

            }
        }
        else
        {
            isCharging = false;
            chargeTimer = 0;

            if (!hasMovedInAir)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    DoubleJump();
                    hasMovedInAir = true;
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    StartCoroutine(Dash());
                    hasMovedInAir = true;
                }
            }
        }
    }

    void Launch()
    {
        isCharging = false;
        audioSource.pitch = 1.0f;
        float chargePercent = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float launchForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);

        Vector2 launchVec;
        float horizontalDir = 0;

        if (GetHorizontalInput() != 0)
        {
            horizontalDir = GetHorizontalInput();
        }
        else if (timeSinceLastInput < inputMemoryTime)
        {
            horizontalDir = lastNonZeroDir;
        }

        if (horizontalDir != 0)
        {
            launchVec = new Vector2(horizontalDir * (launchForce * 0.6f), launchForce);
        }
        else
        {
            launchVec = new Vector2(0, launchForce);
        }


        body.AddForce(launchVec, ForceMode2D.Impulse);
        audioSource.PlayOneShot(jumpSound);
        chargeTimer = 0f;

      
    }

    void DoubleJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
        body.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
        audioSource.PlayOneShot(jumpSound);
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        hasMovedInAir = true;

        float originalGravity = body.gravityScale;
        body.gravityScale = 0;
        float dashDirection = GetHorizontalInput();
        if (dashDirection == 0)
        {
            dashDirection = lastNonZeroDir;
        }
        body.linearVelocity = new Vector2(dashDirection * dashForce, 0);
        audioSource.PlayOneShot(dashSound);
        yield return new WaitForSeconds(dashDuration);
        body.gravityScale = originalGravity;
        body.linearVelocity = Vector2.zero;
        isDashing = false;

    }

    System.Collections.IEnumerator RecoverFromJump()
    {
        isRecovering = true;
        yield return new WaitForSeconds(jumpCooldown);
        isRecovering = false;
    }

    public float GetHorizontalInput()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (isControlsReversed)
        {
            Debug.Log("Controls are reversed!");
            return -input; // Flip it!
        }
        return input;
    }

    public void SetReversedControls(bool state)
    {
        isControlsReversed = state;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? (isOnIce ? Color.cyan : Color.green) : Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }

    void PlayLandingSound(string tag)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        if (tag == "Snow" || tag == "Ice")
        {

            audioSource.PlayOneShot(snowSound);
        }
        else
        {

            audioSource.PlayOneShot(grassSound);
        }
    }
}
