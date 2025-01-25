using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    Rigidbody2D rb;
    CircleCollider2D circleCollider;

    Vector2 moveInput;


    public float movementSpeed = 5f;
    public float jumpPluse = 4f;

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
        
    }

    void FixedUpdate() {
        rb.velocity = new Vector2(moveInput.x * CurrentSpeed, rb.velocity.y);
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
        if (context.started && isOnGround() && CanMove) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPluse);
        }
    }
}
