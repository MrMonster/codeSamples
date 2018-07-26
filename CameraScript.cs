/*
 * Custom camera script built off the principles of how a real world steady cam functions
 * 
 * Using a set of nested objects to act as the harness and arm of the camera rig,
 * This can function as a simple, flexible, and reliable 3rd person camera
 * 
 * This script is attached to the 'harness' object
 * The 'arm' object is a child of the 'harness', and the camera is a child of the 'arm'
 * 
 */

using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    //The 'arm' object
    public Transform camHold;
    //Can be thought of as mouse x and y sensitivity
    public float rotationSpeed = 160f;
    public float verticalSpeed = 130f;
    //Set limits for vertical rotation
    public float UpperVertBounds = 30;
    public float LowerVertBounds = -80;
    //The camera rig's follow target
    public Transform player;
    //Vertical rotation container
    private float xRot;

    void FixedUpdate () {
        //inherit player position
        transform.position = player.position;
        //rotate the 'harness' on the horizontal axis
        transform.Rotate (0f, Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed, 0f);
        //get vertical axis rotation for the 'arm' and apply easing based on the degree of rotation
        xRot +=-Input.GetAxis("Mouse Y") * Time.deltaTime * (verticalSpeed - Mathf.Abs((xRot+20)*1.5f));
        //clamp the vertical rotation within bounds
        xRot = Mathf.Clamp(xRot, LowerVertBounds, UpperVertBounds);
        //apply vertical rotation to 'arm'
        camHold.localEulerAngles = new Vector3(xRot, camHold.localEulerAngles.y, camHold.localEulerAngles.z);
        
    }
}
