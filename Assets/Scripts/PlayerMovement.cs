using System;
using UnityEngine;
using UnityEngine.WSA;

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
    private bool isGrounded;
    private bool isCharging;
    private bool hasMovedInAir;
    private float chargeTimer;

    private float lastNonZeroDir;
    private float timeSinceLastInput;
    public float inputMemoryTime = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        float currentInput = Input.GetAxisRaw("Horizontal");

        if (currentInput != 0)
        {
            lastNonZeroDir = currentInput; // Remember this direction
            timeSinceLastInput = 0;        // Reset the timer
        }
        else
        {
            timeSinceLastInput += Time.deltaTime; // Track how long it's been
        }
        HandleInput();
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if(isGrounded && !wasGrounded)
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

            if(!hasMovedInAir)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    DoubleJump();
                    hasMovedInAir = true;
                }
                else if(Input.GetKeyDown(KeyCode.LeftShift))
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
        //Debug.Log("Launch Force: " + launchForce);
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
        body.linearVelocity = new Vector2(body.linearVelocityX, 0);
        body.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);

        Debug.Log("Double Jump");
    }

    void Dash()
    {
        float dashDirection = Input.GetAxisRaw("Horizontal");
        body.linearVelocity = Vector2.zero;
        body.AddForce(new Vector2(dashDirection * dashForce, 0), ForceMode2D.Impulse);

        Debug.Log("Dash");
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            // If grounded, draw Green. If air, draw Red.
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
