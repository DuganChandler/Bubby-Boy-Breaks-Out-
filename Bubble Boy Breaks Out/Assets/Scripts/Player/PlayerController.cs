using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    Rigidbody2D rb;
    CircleCollider2D circleCollider;

    Vector2 moveInput;

    [Tooltip("How big the bubble is relative to its 'normal' size")]
    public float bubbleSize = 1.0f; // 1.0 = default size
    [Tooltip("A multiplier for buoyant force. Increase for more float.")]
    public float buoyancyFactor = 5f;
    [Tooltip("Should the bubble also reduce gravity scale? (Optional)")]
    public float bubbleGravityScale = 0.2f;

    public float movementSpeed = 5f;
    public float jumpPluse = 1.0f;

    private float bubbleControl = 0f;

    public bool CanMove = true;


    private bool _isFacingRight = true;
    public bool IsFacingRight {
        get {
            return _isFacingRight;
        } private set {
            if (_isFacingRight != value) {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    private bool _isBubble = false;
    public bool IsBubble {
        get {
            return _isBubble;
        } private set {
            _isBubble = value;
            // set animator to bubble boy
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        // Normal horizontal movement, but maybe slower if in bubble:
        float effectiveSpeed = IsBubble ? CurrentSpeed * 0.25f : CurrentSpeed;
        rb.velocity = new Vector2(moveInput.x * effectiveSpeed, rb.velocity.y);

        if (IsBubble)
        {
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
        }
        else
        {
            // Normal mode
            rb.gravityScale = 1.0f; 
            rb.drag = 0f;
        }
        // rb.velocity = new Vector2(moveInput.x * CurrentSpeed, rb.velocity.y);
        // if (IsBubble) {
        // }
    }



    private bool isOnGround() {
        RaycastHit2D hit = Physics2D.CircleCast(circleCollider.bounds.center, circleCollider.radius, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    public void onMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>(); 
        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput) {
        if (moveInput.x > 0 && !IsFacingRight) {
            IsFacingRight = true;
        } else if (moveInput.x < 0 && IsFacingRight) {
           IsFacingRight = false; 
        }
    }

    public void onJump(InputAction.CallbackContext context) {
        Debug.Log(isOnGround());
        if (context.started && isOnGround() && CanMove && !IsBubble) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPluse);
        }
    }

    public void onBubble(InputAction.CallbackContext context) {
        if (context.started && !IsBubble) {
            IsBubble = true;
            Debug.Log("Bubble");
        } else if (context.started && IsBubble) {
            IsBubble = false;
            bubbleSize = 1.0f;
            Debug.Log("Not Bubble");
        }
    }

    public void onBubbleControl(InputAction.CallbackContext context) {
        if (IsBubble) {
            bubbleControl = context.ReadValue<Single>();
        }
    }
}
