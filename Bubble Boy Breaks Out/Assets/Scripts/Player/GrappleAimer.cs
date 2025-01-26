using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAimer : MonoBehaviour {
    public Transform arrowTransform;  // The transform of the arrow GameObject
    public Transform player;          // The transform of the player GameObject

    void Update()
    {
        AimArrowAtMouse();
    }

    void AimArrowAtMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // We only care about x/y in 2D, so make sure z is the same as the player or arrow
        mousePos.z = player.position.z;
        
        // Calculate direction from the player to the mouse
        Vector3 direction = mousePos - player.position;
        
        // Calculate angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Apply rotation to the arrow around the Z axis
        arrowTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
