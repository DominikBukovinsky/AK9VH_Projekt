using UnityEngine;

public class FrogController : MonoBehaviour
{

    [Header("Skok")]
    public float maxJumpForce = 15f; // Maximální síla skoku
    public float minJumpForce = 2f;  // Minimální síla skoku 
    public float maxChargeTime = 1f; // Jak dlouho trvá nabít na max

    [Header("Kontrola Země")]
    public Transform groundCheck; // Objekt pro kontrolu země
    public LayerMask groundLayer; // Vrstva, která je považována za zem

    private Rigidbody2D rb;
    private float jumpCharge = 0f;
    private bool isCharging = false;
    private bool isGrounded;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update běží každý snímek
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 (A/vlevo), 0, +1 (D/vpravo)

        // 3. Logika nabíjení skoku
        if (isGrounded)
        {
            // Hráč DRŽÍ mezerník
            if (Input.GetKey(KeyCode.Space))
            {
                isCharging = true;
                jumpCharge += Time.deltaTime; // Nabíjíme časem
            }

            // Hráč PUSTIL mezerník
            if (Input.GetKeyUp(KeyCode.Space))
            {
                PerformJump();
            }
        }
        else // Pokud jsme ve vzduchu
        {
            // Zrušíme nabíjení, pokud spadneme z plošiny
            isCharging = false;
            jumpCharge = 0f;
        }
    }

    void FixedUpdate()
    {

        if (isGrounded && !isCharging)
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
        Vector2 jumpDirection = Vector2.up; // Základní směr je nahoru
        
        if (horizontalInput != 0)
        {
            jumpDirection = new Vector2(horizontalInput, 1.5f).normalized;
        }

        rb.linearVelocity = Vector2.zero; // Reset rychlosti pro konzistentní skok
        rb.AddForce(jumpDirection * currentJumpForce, ForceMode2D.Impulse);

        // 4. Reset
        jumpCharge = 0f;
        isCharging = false;
    }
}