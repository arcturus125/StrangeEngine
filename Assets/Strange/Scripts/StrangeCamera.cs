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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = target.position;





        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0)
        {
            player.Rotate(0, mouseX * turningSpeed, 0);
            transform.RotateAround(target.position, Vector3.up, mouseX);
        }
        if (mouseY != 0)
        {
            transform.RotateAround(target.position, target.transform.right, -mouseY);
        }


        // basic clipping

        if ( (transform.rotation.eulerAngles.x > 80.0f) && (transform.rotation.eulerAngles.x < 100.0f) )
        {
            transform.rotation = Quaternion.Euler(80,
                                                  transform.rotation.eulerAngles.y,
                                                  transform.rotation.eulerAngles.z);
        }
        if (transform.rotation.eulerAngles.x < 1.0f)
        {
            transform.rotation = Quaternion.Euler(1.0f,
                                                  transform.rotation.eulerAngles.y,
                                                  transform.rotation.eulerAngles.z);
        }
    }
}
