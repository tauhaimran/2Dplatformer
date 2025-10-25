using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSaw : MonoBehaviour
{
    [Header("Movement Points (Empty GameObjects)")]
    public Transform LeftPoint;     // Left boundary
    public Transform RightPoint;    // Right boundary

    [Header("Saw Sprite (Child)")]
    public Transform SawSprite;     // The saw sprite to rotate

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f; // degrees per second

    private bool movingRight = true;

    void Update()
    {
        if (LeftPoint == null || RightPoint == null || SawSprite == null)
            return;

        // --- ROTATE THE SAW ON ITS OWN AXIS ---
        SawSprite.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // --- MOVE THE SAW HORIZONTALLY BETWEEN TWO POINTS ---
        Vector3 targetPosition = movingRight ? RightPoint.position : LeftPoint.position;

        // Keep Y constant so it only moves horizontally
        Vector3 newPos = Vector2.MoveTowards(
            new Vector2(SawSprite.position.x, SawSprite.position.y),
            new Vector2(targetPosition.x, SawSprite.position.y),
            moveSpeed * Time.deltaTime
        );

        SawSprite.position = new Vector3(newPos.x, SawSprite.position.y, SawSprite.position.z);

        // --- SWITCH DIRECTION WHEN REACHING LIMIT ---
        if (Mathf.Abs(SawSprite.position.x - targetPosition.x) < 0.05f)
        {
            movingRight = !movingRight;
        }
    }
}
