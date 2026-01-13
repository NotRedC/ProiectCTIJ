using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public string iceMaterialName = "IceMaterial"; // Numele materialului creat de tine

    private Rigidbody2D body;
    private Animator anim; // REFERINTA NOUA
    private SpriteRenderer sprite; // PENTRU FLIP

    private bool isGrounded;
    private bool isOnIce; // variabila noua
    private bool isCharging;
    private bool isDashing;
    private bool hasMovedInAir;
    private float chargeTimer;

    private float lastNonZeroDir = 1;
    private float timeSinceLastInput;
    public float inputMemoryTime = 0.1f;

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
        float currentInput = Input.GetAxisRaw("Horizontal");

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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            hasMovedInAir = false;
            body.linearVelocity = Vector2.zero;
        }
    }

    void HandleInput()
    {
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isCharging = true;
                chargeTimer += Time.deltaTime;
                body.linearVelocity = Vector2.zero;
                //Debug.Log("Charging: " + chargeTimer);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
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
        float chargePercent = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float launchForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);

        Vector2 launchVec;
        float horizontalDir = 0;

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            horizontalDir = Input.GetAxisRaw("Horizontal");
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
        chargeTimer = 0f;

        if (anim != null) anim.SetTrigger("takeOff"); // Trigger optional pentru salt
    }

    void DoubleJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
        body.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        hasMovedInAir = true;

        float originalGravity = body.gravityScale;
        body.gravityScale = 0;
        float dashDirection = Input.GetAxisRaw("Horizontal");
        if (dashDirection == 0)
        {
            dashDirection = lastNonZeroDir;
        }
        body.linearVelocity = new Vector2(dashDirection * dashForce, 0);
        yield return new WaitForSeconds(dashDuration);
        body.gravityScale = originalGravity;
        body.linearVelocity = Vector2.zero;
        isDashing = false;

    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? (isOnIce ? Color.cyan : Color.green) : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}