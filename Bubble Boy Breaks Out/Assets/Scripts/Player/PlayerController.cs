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

    public bool isOnWall = false;
    public float movementSpeed = 5f;
    public float jumpPluse = 4f;
    public float wallJumpDirection;
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
        SetWallDirection();

    }

    void FixedUpdate() {
        
        if (isOnWall) {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        } else if (!isOnWall) {
            rb.gravityScale = 1;
            rb.velocity = new Vector2(moveInput.x * CurrentSpeed, rb.velocity.y);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collider){
        if (collider.gameObject.layer == 8 && isOnWall == false) {
            isOnWall = true;
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
    
    

    public void onMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
        if(isOnWall && !isOnGround()){
            return;
        }
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
        if (context.started && isOnGround() && CanMove && !isOnWall) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPluse);
        } else if(context.started && isOnWall){
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

    
}
