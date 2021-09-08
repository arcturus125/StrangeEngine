// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangePlayer : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    public float walkSpeed = 1.0f;

    // movement keys, WASD by default
    public static KeyCode walkForward   = KeyCode.W;
    public static KeyCode walkBack      = KeyCode.S;
    public static KeyCode strafeLeft    = KeyCode.A;
    public static KeyCode strafeRight   = KeyCode.D;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        // start with no force
        Vector3 movementForce = Vector3.zero;

        // if buttons are pressed, accumulate force in that direction - opposite directions cancel out
        if (Input.GetKey(walkForward))
            movementForce += Vector3.forward;
        if (Input.GetKey(walkBack))
            movementForce += Vector3.back;
        if (Input.GetKey(strafeLeft))
            movementForce += Vector3.left;
        if (Input.GetKey(strafeRight))
            movementForce += Vector3.right;

        // normalise movementForce so that moving diagonal does not move the player faster
        Vector3 finalForce = transform.TransformDirection(movementForce.normalized * walkSpeed);
        // move the player
        rb.MovePosition(transform.position + finalForce);
    }
}
