// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System;
using System.Collections;
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

    [Header("Camera clamps")]
    public float maxCameraClamp = 80.0f;
    public float minCameraClamp = -20.0f;

    [Header("Springarm variables")]
    [Tooltip("If you want to switch to first person, set this number to 0")] 
    public float maxSpringarmLength = 10.0f;
    public float antiClip = 0.2f;

    public static bool disableMouse = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    float oldRotY = 0.0f;
    float rotY = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (!disableMouse)
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
        }

        oldRotY = rotY;

        // springarm
        RaycastHit hitInfo;
        if(Physics.Raycast(this.transform.position, -this.transform.forward, out hitInfo))
        {
            float length = hitInfo.distance;
            if(length < maxSpringarmLength)
            {
                GetComponentInChildren<Camera>().gameObject.transform.localPosition = new Vector3(0, 0, -length + antiClip);
            }
        }
        else
        {
            // if the maxSpringarmLength is changed during runtime. this will update the camera's position immediately if neccessary
            GetComponentInChildren<Camera>().gameObject.transform.localPosition = new Vector3(0, 0, -maxSpringarmLength);
        }




        ESChit();
    }

    private void ESChit()
    {
        // whenever ESC is hit, toggle visibility of cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
