using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Skok")]
    public float maxJumpForce = 15f; 
    public float minJumpForce = 2f;  
    public float maxChargeTime = 1f; 

    [Header("Kontrola Země")]
    public Transform groundCheck; 
    public Vector2 groundCheckSize = new Vector2(1f, 0.2f); 
    public LayerMask groundLayer; 
    public LayerMask iceLayer;    
    public LayerMask bounceLayer; 

    [Header("Odraz (Bounce)")]
    [Tooltip("Jaká část energie se zachová po odrazu (0.7 = 70%)")]
    public float bounceDamping = 0.7f; 
    [Tooltip("Pokud je odraz slabší než toto, postava se zastaví")]
    public float minBounceVelocity = 2f; 

    [Header("Kontrola Zdi")]
    public Transform wallCheck; 
    public Vector2 wallCheckSize = new Vector2(0.3f, 1f); 
    public LayerMask wallLayer; 
    public float wallKickForceX = 3f; 
    public float wallKickForceY = 8f; 

    private Rigidbody2D rb;
    private Animator anim;
    private float jumpCharge = 0f;
    private bool isCharging = false;
    public bool isGrounded;
    private bool isStandingOnIce; 
    private bool isStandingOnBounce;
    
    private bool isWalled; 
    private bool isFacingRight = true; 
    private float horizontalInput;

    private bool wasWalled = false; 
    private bool wasGrounded = false; 
    private Vector2 lastAirVelocity; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. KONTROLA VSTUPU A PŘEPÍNÁNÍ SMĚRU
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
        Flip(); 

        // 2. KONTROLA STAVŮ
        bool onGround = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        bool onIce = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, iceLayer);
        bool onBounce = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, bounceLayer); 

        isGrounded = onGround || onIce || onBounce;
        isStandingOnIce = onIce;
        isStandingOnBounce = onBounce; 

        isWalled = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, wallLayer);

        if (anim != null)
        {
            anim.SetBool("IsGrounded", isGrounded);
        }

        // 3. LOGIKA SKOKU, ODRAZU A BOUNCE
        
        if (isGrounded && !wasGrounded)
        {
            if (isStandingOnBounce)
            {
                PerformBounce(lastAirVelocity); 
            }
        }
        
        if (isGrounded)
        {
            // ***** TADY JE TA OPRAVA *****
            
            // Zjistíme, jestli jsme se "usadili" (skoro se nehýbeme)
            bool isSettled = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

            // Povolíme nabíjení pokud:
            // 1. Nestojíme na trampolíně (je to normální zem/led)
            // 2. NEBO stojíme na trampolíně, ale už jsme se "usadili" (dopadali)
            if (Input.GetKey(KeyCode.Space) && (!isStandingOnBounce || isSettled))
            {
                isCharging = true;
                jumpCharge += Time.deltaTime; 
            }

            // Stejnou logiku použijeme pro puštění klávesy
            if (Input.GetKeyUp(KeyCode.Space) && (!isStandingOnBounce || isSettled))
            {
                PerformJump();
            }
        }
        else // --- Logika ve vzduchu ---
        {
            isCharging = false;
            jumpCharge = 0f;

            if (isWalled && !wasWalled && !isGrounded)
            {
                PerformWallKick();
            }
        }

        // 4. AKTUALIZACE STAVU PRO PŘÍŠTÍ SNÍMEK
        wasWalled = isWalled;
        
        if (!isGrounded)
        {
            lastAirVelocity = rb.linearVelocity;
        }
        wasGrounded = isGrounded; 
    }
    
    void FixedUpdate()
    {
        if (isGrounded && !isCharging && !isStandingOnIce && !isStandingOnBounce && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void PerformJump()
    {
        if (!isGrounded || !isCharging) 
        {
            isCharging = false;
            jumpCharge = 0f;
            return;
        }
        float chargePercent = Mathf.Clamp01(jumpCharge / maxChargeTime);
        float currentJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);
        
        Vector2 jumpDirection = Vector2.up; 
        
        if (horizontalInput != 0)
        {
            jumpDirection = new Vector2(horizontalInput, 2.5f).normalized; 
        }

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(jumpDirection * currentJumpForce, ForceMode2D.Impulse);

        jumpCharge = 0f;
        isCharging = false;
    }

    void PerformBounce(Vector2 incomingVelocity)
    {
        Vector2 reflectedVelocity = new Vector2(
            incomingVelocity.x,          
            -incomingVelocity.y * bounceDamping 
        );

        if (reflectedVelocity.y < minBounceVelocity)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = reflectedVelocity;
        }
        
        isCharging = false;
        jumpCharge = 0f;
    }

    void PerformWallKick()
    {
        float kickDirection = isFacingRight ? -1f : 1f;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(new Vector2(wallKickForceX * kickDirection, wallKickForceY), ForceMode2D.Impulse);
    }

    void Flip()
    {
        if (horizontalInput > 0f && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0f && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green; 
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
        }
    }
}