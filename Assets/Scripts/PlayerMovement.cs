using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float dashForce;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public string iceMaterialName = "IceMaterial"; // Numele materialului creat de tine

    private Rigidbody2D body;
    private bool isGrounded;
    private bool isOnIce; // variabila noua
    private bool isCharging;
    private bool hasMovedInAir;
    private float chargeTimer;

    private float lastNonZeroDir;
    private float timeSinceLastInput;
    public float inputMemoryTime = 0.1f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();
        float currentInput = Input.GetAxisRaw("Horizontal");

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
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        // Detectam collider-ul de sub noi
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = hit != null;

        // Verificam daca solul pe care stam este gheata
        if (isGrounded && hit.sharedMaterial != null)
        {
            isOnIce = (hit.sharedMaterial.name == iceMaterialName);
        }
        else
        {
            isOnIce = false;
        }

        if (isGrounded && !wasGrounded)
        {
            hasMovedInAir = false;
            // Daca nu e gheata, oprim player-ul cand aterizeaza
            if (!isOnIce)
            {
                body.linearVelocity = Vector2.zero;
            }
        }
    }

    void HandleInput()
    {
        if (isGrounded)
        {
            // MODIFICARE: Poate incarca saritura DOAR daca NU este pe gheata
            if (Input.GetKey(KeyCode.Space) && !isOnIce)
            {
                isCharging = true;
                chargeTimer += Time.deltaTime;
                body.linearVelocity = Vector2.zero;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                // Daca a apucat sa incarce inainte sa intre pe gheata, il lasam sa lanseze
                if (isCharging) Launch();
            }

            // Daca e pe gheata si incearca sa apese Space, dam un feedback (optional)
            if (Input.GetKeyDown(KeyCode.Space) && isOnIce)
            {
                Debug.Log("Nu poti sari de pe gheata!");
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
                    Dash();
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

        body.linearVelocity = Vector2.zero;
        body.AddForce(launchVec, ForceMode2D.Impulse);

        chargeTimer = 0f;
    }

    void DoubleJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
        body.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
    }

    void Dash()
    {
        float dashDirection = Input.GetAxisRaw("Horizontal");
        if (dashDirection == 0) dashDirection = lastNonZeroDir; // Dash in ultima directie daca nu apasam nimic

        body.linearVelocity = Vector2.zero;
        body.AddForce(new Vector2(dashDirection * dashForce, 0), ForceMode2D.Impulse);
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