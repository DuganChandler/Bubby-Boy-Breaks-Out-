using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    
    [SerializeField] LayerMask wallLayer;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    Animator animator;

    Vector2 moveInput;
    Vector2 startPos;

    [Tooltip("How big the bubble is relative to its 'normal' size")]
    public float bubbleSize = 1.0f; // 1.0 = default size
    [Tooltip("A multiplier for buoyant force. Increase for more float.")]
    public float buoyancyFactor = 5f;
    [Tooltip("Should the bubble also reduce gravity scale? (Optional)")]
    public float bubbleGravityScale = 0.2f;
    SpriteRenderer spriteRenderer;
    public float movementSpeed = 5f;
    public float jumpPluse = 1.0f;
    public int score;
    private float bubbleControl = 0f;

    public bool CanMove = true;

    public bool isOnWall = false;
    public float wallJumpDirection;
    private bool _isFacingRight = true;
    
    //audio
    public AudioClip expand;
    public AudioClip contract;
    public AudioClip splat;
    public AudioClip stick;
    public AudioClip bubble;
    public AudioClip move;


    public bool IsFacingRight {
        get {
            return _isFacingRight;
        } private set {
            if (_isFacingRight != value) {
                // transform.localScale *= new Vector2(-1, 1);
                spriteRenderer.flipX = true;
            }
            _isFacingRight = value;
        }
    }

    private bool _isMoving = false;
    public bool IsMoving {
        get {
            return _isMoving;
        } private set {
            _isMoving = value;
            animator.SetBool("IsMoving", value); 
            AudioSource.PlayClipAtPoint(move, transform.position);
        }
    }

    private bool _isBubble = false;
    public bool IsBubble {
        get {
            return _isBubble;
        } private set {
            _isBubble = value;
            // set animator to bubble boy
            animator.SetBool("IsBubble", value);
        }
    }
    
    public float CurrentSpeed { get {
        if (CanMove) {
            return movementSpeed;
        } else {
            return 0;
        }
    }}

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    void Update() {
        if (IsBubble) {
            if (bubbleControl > 0) {
                bubbleSize += Time.deltaTime * 0.5f;
            } else if (bubbleControl < 0) {
                bubbleSize -= Time.deltaTime * 0.5f;
            }
            bubbleSize = Mathf.Clamp(bubbleSize, 0.5f, 2f);
        }
    }

    

    void FixedUpdate() {
        SetWallDirection();
        float effectiveSpeed = IsBubble ? CurrentSpeed * 0.25f : CurrentSpeed;
        if (IsBubble) {
            rb.velocity = new Vector2(moveInput.x * effectiveSpeed, rb.velocity.y);
            // Optionally reduce gravity scale so we don't fall so fast:
            rb.gravityScale = bubbleGravityScale;

            // Calculate buoyant force based on bubbleSize relative to 1.0 = default
            // Example: If bubbleSize > 1, we get an upward force
            // If bubbleSize < 1, maybe no buoyancy or even a small downward force
            float sizeRatio = bubbleSize - 1.0f; // negative if smaller than default
            float buoyantForce = sizeRatio * buoyancyFactor;

            // If you only want buoyancy when the bubble is larger than default:
            if (buoyantForce < 0) buoyantForce = 0;

            // Apply upward force
            rb.AddForce(Vector2.up * buoyantForce, ForceMode2D.Force);

            // OPTIONAL: Add some linear drag for floaty, slower movement
            rb.drag = 2f;  
        } else if (isOnWall) {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        } else if (!isOnWall) {
            rb.gravityScale = 1;
            rb.velocity = new Vector2(moveInput.x * effectiveSpeed, rb.velocity.y);
            rb.drag = 0f;
        }
    }

    

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.layer == 8 && isOnWall == false) {
            isOnWall = true;
        }
        GameObject collidedWith = collider.gameObject;
        if(collidedWith.CompareTag("Danger")){
            StartCoroutine(DeathAnim(.3f));
        }
        if (collidedWith.CompareTag("Coin")){
            Destroy(collidedWith);
            score= score + 5;
            
        }
        if (collidedWith.CompareTag("Win"))
        {
            SceneManager.LoadSceneAsync("WinScreen");    
        }
    }

    IEnumerator DeathAnim(float duration) {
        animator.SetTrigger("Die");
        AudioSource.PlayClipAtPoint(bubble, transform.position);
        yield return new WaitForSeconds(duration);
        StartCoroutine(Respawn(0.1f));
    }
    
    IEnumerator Respawn(float duration){
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(duration);
        transform.position = startPos;
        spriteRenderer.enabled = true;
    }    

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.layer == 8 && isOnWall == true && !IsBubble) {
            isOnWall = false;
            rb.gravityScale = 1;
        }
    }

    private bool isOnGround() {
        RaycastHit2D hit = Physics2D.CircleCast(circleCollider.bounds.center, circleCollider.radius, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    public void SetWallDirection(){
        RaycastHit2D hitRight = Physics2D.CircleCast(circleCollider.bounds.center, circleCollider.radius, Vector2.right, 0.1f, wallLayer);
        RaycastHit2D hitLeft = Physics2D.CircleCast(circleCollider.bounds.center,circleCollider.radius, Vector2.left, 0.1f,wallLayer);
        if(hitRight.collider != null){
            wallJumpDirection = -1;
        }else if (hitLeft.collider != null){
            wallJumpDirection = 1;
        }
    }

     private void SetFacingDirection(Vector2 moveInput) {
        if (moveInput.x > 0 && !IsFacingRight) {
            IsFacingRight = true;
        } else if (moveInput.x < 0 && IsFacingRight) {
           IsFacingRight = false; 
        }
    }


    // Input system callbacks
    public void onMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput != Vector2.zero && !isOnWall) {
            IsMoving = true;
        } else {
            IsMoving = false;
        }
        if(isOnWall && !isOnGround() && !IsBubble){
            return;
        }
        // SetFacingDirection(moveInput);
    }

    public void onJump(InputAction.CallbackContext context) {
        if (context.started && isOnGround() && CanMove && !isOnWall && !IsBubble) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPluse);
        } else if(context.started && isOnWall && !IsBubble){
            isOnWall = false;
            rb.gravityScale = 1;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, 0f));
            Vector2 walljumpangle = new Vector2(1, 1.2f);
            Vector2 jumpDir = walljumpangle.normalized;
            jumpDir.x *= wallJumpDirection;
            rb.AddForce(jumpDir * 3f, ForceMode2D.Impulse);
            Debug.Log(jumpDir * 10f);
            
        }
        
    }

    public void onBubble(InputAction.CallbackContext context) {
        if (context.started && !IsBubble) {
            IsBubble = true;
            animator.SetTrigger("Expand");
            AudioSource.PlayClipAtPoint(expand, transform.position);
            Debug.Log("Bubble");
        } else if (context.started && IsBubble) {
            IsBubble = false;
            bubbleSize = 1.0f;
            animator.SetTrigger("Contract");
            AudioSource.PlayClipAtPoint(contract, transform.position);
            Debug.Log("Not Bubble");
        }
    }

    public void onBubbleControl(InputAction.CallbackContext context) {
        if (IsBubble) {
            bubbleControl = context.ReadValue<Single>();
        }
    }
}
