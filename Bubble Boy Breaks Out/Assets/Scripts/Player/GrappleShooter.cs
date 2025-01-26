using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Callbacks;
using UnityEngine;

public class GrappleShooter : MonoBehaviour
{
    public float grapplingSpeed = 10f;
    public float maxDistance = 20f;
    public LayerMask grappleableMask;

    private Rigidbody2D rb;

    public LineRenderer lineRenderer;
    private Vector2 grapplingHookPos;
    private bool isGrappling = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get the position of the mouse click
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Cast a ray from the mouse position to check for grappleable objects
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, grappleableMask);

            if (hit.collider != null)
            {
                // Grapple to the object
                grapplingHookPos = hit.point;
                isGrappling = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Stop grappling
            isGrappling = false;
        }

        if (isGrappling)
        {
            // Move the player towards the grappling hook position
            Vector2 newPos = Vector2.MoveTowards(transform.position, grapplingHookPos, grapplingSpeed * Time.deltaTime);
            transform.position = newPos;

            // Update the line renderer to show the grapple line
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplingHookPos);

            // Check if the player is too far away from the grappling hook position
            float distance = Vector2.Distance(transform.position, grapplingHookPos);
            if (distance > maxDistance)
            {
                isGrappling = false;
            }
        }
        else
        {
            // Disable the line renderer when not grappling
            lineRenderer.enabled = false;
        }
    }
    // public Transform arrowTransform;         // Reference to the same arrow used for aiming
    // public Transform player;                 // Player transform
    // public float maxGrappleDistance = 10f;   // How far your grapple can reach
    // public LayerMask whatIsGrappleable;      // Layer mask for valid grapple targets

    // public float moveSpeed = 10f;            // Speed to move player to grapple point (if doing a smooth move)


    // void Update()
    // {
    //     // Check if left mouse button is pressed
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         FireGrapple();
    //     }
    // }

    // void FireGrapple()
    // {
    //     // The arrow’s 'right' direction is the forward direction 
    //     // (assuming your arrow sprite points to the right)
    //     Vector2 direction = arrowTransform.right;

    //     // Cast a ray from the player's position
    //     RaycastHit2D hit = Physics2D.Raycast(player.position, direction, maxGrappleDistance, whatIsGrappleable);

    //     if (hit.collider != null)
    //     {
    //         // We hit something grappleable
    //         Vector2 grapplePoint = hit.point;

    //         // Move the player to that point (smoothly or instantly)
    //         StartCoroutine(MovePlayer(grapplePoint));
    //     }
    //     else
    //     {
    //         // No hit, do nothing or play "failed grapple" animation/sound
    //     }
    // }

    // System.Collections.IEnumerator MovePlayer(Vector2 targetPos)
    // {
    //     // Smooth movement toward the target
    //     while (Vector2.Distance(player.position, targetPos) > 0.05f)
    //     {
    //         player.position = Vector2.MoveTowards(
    //             player.position, 
    //             targetPos, 
    //             moveSpeed * Time.deltaTime
    //         );
    //         yield return null;
    //     }

    //     // Snap to position
    //     player.position = targetPos;

    //     // (Optional) Activate your "stick to wall" logic here
    //     // StickToWall();
    // } 
}
