using UnityEngine;
using UnityEngine.SceneManagement; // Nutné pro načítání scén

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

    [Header("Led (Ice)")]
    [Tooltip("Čím nižší číslo, tím více to klouže. 0 = nekonečné klouzání, 5 = rychlé zastavení.")]
    public float iceDeceleration = 1.5f; 

    [Header("Kontrola Zdi")]
    public Transform wallCheck; 
    public Vector2 wallCheckSize = new Vector2(0.3f, 1f); 
    public LayerMask wallLayer; 
    public float wallKickForceX = 3f; 
    public float wallKickForceY = 8f; 

    // --- TADY JE ZMĚNA PRO EGIRL ---
    [Header("Konec hry (Egirl)")] 
    public string sceneToLoad = "Credits"; // Název scény
    public LayerMask egirlLayer; // <--- Tady vybereme vrstvu v Inspectoru
    // -------------------------------

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
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
        Flip(); 

        bool onGround = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        bool onIce = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, iceLayer);
        bool onBounce = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, bounceLayer); 

        isGrounded = onGround || onIce || onBounce;
        isStandingOnIce = onIce;
        isStandingOnBounce = onBounce; 

        isWalled = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, wallLayer);

        if (anim != null) anim.SetBool("IsGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            if (isStandingOnBounce) PerformBounce(lastAirVelocity); 
        }
        
        if (isGrounded)
        {
            bool isSettled = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
            if (Input.GetKey(KeyCode.Space) && (!isStandingOnBounce || isSettled))
            {
                isCharging = true;
                jumpCharge += Time.deltaTime; 
            }
            if (Input.GetKeyUp(KeyCode.Space) && (!isStandingOnBounce || isSettled))
            {
                PerformJump();
            }
        }
        else 
        {
            isCharging = false;
            jumpCharge = 0f;
            if (isWalled && !wasWalled && !isGrounded) PerformWallKick();
        }

        wasWalled = isWalled;
        if (!isGrounded) lastAirVelocity = rb.linearVelocity;
        wasGrounded = isGrounded; 
    }
    
    void FixedUpdate()
    {
        if (isGrounded && !isCharging && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            if (!isStandingOnIce && !isStandingOnBounce)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else if (isStandingOnIce)
            {
                float newX = Mathf.Lerp(rb.linearVelocity.x, 0, iceDeceleration * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
            }
        }
    }

    void PerformJump()
    {
        if (!isGrounded || !isCharging) { isCharging = false; jumpCharge = 0f; return; }
        float chargePercent = Mathf.Clamp01(jumpCharge / maxChargeTime);
        float currentJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);
        Vector2 jumpDirection = horizontalInput != 0 ? new Vector2(horizontalInput, 2.5f).normalized : Vector2.up;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(jumpDirection * currentJumpForce, ForceMode2D.Impulse);
        jumpCharge = 0f;
        isCharging = false;
    }

    void PerformBounce(Vector2 incomingVelocity)
    {
        Vector2 reflectedVelocity = new Vector2(incomingVelocity.x, -incomingVelocity.y * bounceDamping);
        if (reflectedVelocity.y < minBounceVelocity) rb.linearVelocity = Vector2.zero;
        else rb.linearVelocity = reflectedVelocity;
        isCharging = false; jumpCharge = 0f;
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
        if (groundCheck != null) { Gizmos.color = Color.green; Gizmos.DrawWireCube(groundCheck.position, groundCheckSize); }
        if (wallCheck != null) { Gizmos.color = Color.blue; Gizmos.DrawWireCube(wallCheck.position, wallCheckSize); }
    }

    // ------------------------------------------------------------------
    // FUNKCE PRO DOTYK S EGIRL PŘES VRSTVU (LAYER)
    // ------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vypíšeme do konzole COKOLIV, čeho se dotkneme
        Debug.Log("Dotkl jsem se: " + collision.gameObject.name);

        // Dočasně ignorujeme kontrolu vrstvy a ptáme se na jméno objektu
        // Ujisti se, že se tvůj objekt Egirl ve scéně jmenuje "Egirl" (nebo uprav název v uvozovkách)
        if (collision.gameObject.name == "Egirl") 
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}