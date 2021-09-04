using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangePlayer : MonoBehaviour
{
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
        if (Input.GetKey(walkForward))
        {
            rb.AddRelativeForce(Vector3.forward * walkSpeed);
        }
        if (Input.GetKey(walkBack))
        {
            rb.AddRelativeForce(Vector3.back * walkSpeed);
        }
        if (Input.GetKey(strafeLeft))
        {
            rb.AddRelativeForce(Vector3.left * walkSpeed);
        }
        if (Input.GetKey(strafeRight))
        {
            rb.AddRelativeForce(Vector3.right * walkSpeed);
        }
    }
}
