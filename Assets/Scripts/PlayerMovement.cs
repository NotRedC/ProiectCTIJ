using System;
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

    private Rigidbody2D body;
    private Animator anim; // REFERINTA NOUA
    private SpriteRenderer sprite; // PENTRU FLIP

    private bool isGrounded;
    private bool isCharging;
    private bool hasMovedInAir;
    private float chargeTimer;

    private float lastNonZeroDir;
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
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                Launch();
            }
            else
            {
                isCharging = false; // Reset daca nu apasa
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

        float horizontalDir = 0;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            horizontalDir = Input.GetAxisRaw("Horizontal");
        }
        else if (timeSinceLastInput < inputMemoryTime)
        {
            horizontalDir = lastNonZeroDir;
        }

        Vector2 launchVec = (horizontalDir != 0)
            ? new Vector2(horizontalDir * (launchForce * 0.6f), launchForce)
            : new Vector2(0, launchForce);

        body.linearVelocity = Vector2.zero;
        body.AddForce(launchVec, ForceMode2D.Impulse);
        chargeTimer = 0f;

        if (anim != null) anim.SetTrigger("takeOff"); // Trigger optional pentru salt
    }

    void DoubleJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, 0);
        body.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
        if (anim != null) anim.SetTrigger("doubleJump");
    }

    void Dash()
    {
        float dashDirection = (Input.GetAxisRaw("Horizontal") != 0) ? Input.GetAxisRaw("Horizontal") : (sprite.flipX ? -1 : 1);
        body.linearVelocity = Vector2.zero;
        body.AddForce(new Vector2(dashDirection * dashForce, 0), ForceMode2D.Impulse);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
