﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeCamera : MonoBehaviour
{
    [Header("set this to the target you want the camera to follow")]
    public Transform target; // camera's pivot point will follow this target
    [Header("set this to the player so the player will turn with mouse movement")]
    public Transform player; // when mouse is moved along x axis, player is rotated

    [Header("")] // used for a gap in the inspector
    public float turningSpeed = 1.0f;

    public float maxCameraClamp = 80;
    public float minCameraClamp = -20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float oldRotY = 0.0f;
    float rotY = 0.0f;
    // Update is called once per frame
    void Update()
    {
        this.transform.position = target.position;


        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // accumulate Y rotation for clamping
        rotY += mouseY;
        rotY = Mathf.Clamp(rotY, -maxCameraClamp, -minCameraClamp);

        if (mouseX != 0)
        {
            player.Rotate(0, mouseX * turningSpeed, 0);
            transform.RotateAround(target.position, Vector3.up, mouseX);
        }
        if (mouseY != 0)
        {
            // rotate the camera back to default, then to the new location
            transform.RotateAround(target.position, target.transform.right, oldRotY);
            transform.RotateAround(target.position, target.transform.right, -rotY);
        }

        oldRotY = rotY;
    }
}
