/*
 * A surface based physics player controller
 * 
 * Expands on Unity's physics behavior to allow for a player,
 * to walk on any suface on any mesh
 * 
 * Also contains all of the player controls, excluding camera controls
 * 
 * 
 */

using UnityEngine;
using System.Collections;

//Require rigidbody for physics operations
[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour {

    //The parent container for the camera rig aka 'harness'
    public Transform cameraBox;
    //The 'arm' of the camera rig
    public Transform camHold;
    //The main camera
    public Transform cam;

    //Velocity values for movement
    public float mass = 0.3f;
	public float moveAcceleration = 0.3f;
    //Control values for movement feel
    public float maxMoveSpeed = 2.0f;
	public float slowdownTime = 0.3f;
    
    //The player's animation controller
    public Animator animator;
    //The player's physics body
    public Rigidbody rigBod;
    //Local containers
	private Vector3 acceleration = new Vector3(0f, 0f, 0f);
	private Vector3 speed = new Vector3 (0f, 0f, 0f);
	private float moveSpeed = 0f;
	private Vector3 lastPosition;
    //Is grounded check
	public bool floored = false;
    //Holds time of previous input for reference
    private float lastInput;
    //Values used for checkpoint system
    //Position of end of level
    public Transform endGoal;
    //Are all checkpints collected
    public bool collected = false;
    //Current number of checkpoints collected
    public int numCollected = 0;

	//Used to debug vector changes
	private Vector3 lastDiff;
	
	void Start () {
        //Get components from Player object
        animator = GetComponent<Animator>();
        rigBod = GetComponent<Rigidbody>();
        //Set base values
        lastInput = Time.time;
		lastDiff = new Vector3(0, 0, 0);
	}
	
    //Send message if touching ground
	void Floor () {
		floored = true;
		SendMessage("startStickingDown");
	}

    //Controls movement behavior
    void ProcessMove () {
        //Get input axis
		float zAxis = Input.GetAxis ("Vertical");
		float xAxis = Input.GetAxis ("Horizontal");
        //Clamps move speed acceleration for sum of axis
        moveSpeed += Mathf.Clamp((Mathf.Abs (xAxis) + Mathf.Abs(zAxis)),0,1) * moveAcceleration * Time.deltaTime;
        //Limiter on acceleration
        if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
        //Call blended animations for movement
        animator.SetFloat("InputVertical", moveSpeed);
        //Check if input is greater than minimum margin
        if (Mathf.Abs (xAxis) > 0.2 || Mathf.Abs(zAxis) > 0.2) {
            //Record last input time
            lastInput = Time.time;
            //Get the difference between movement input an camera angle
			Vector3 diff = (camHold.forward * zAxis + camHold.right * xAxis);
            //normalize difference
            diff.Normalize();
            //Inverse difference to get proper direction vector
            diff = transform.InverseTransformDirection(diff);
            //remove local y from difference
            diff.y = 0;
            //get new vector direction after local y removal
            diff = transform.TransformDirection(diff);
            //Point character in resulting direction
            transform.LookAt(transform.position + diff, transform.up);
            //Record for reference
            lastDiff = diff;
		}
        //Show direction vector for debugging
        Debug.DrawRay (transform.position, lastDiff);
        //de-accelerate based on time from last input
        moveSpeed = Mathf.Lerp(moveSpeed, 0, (Time.time - lastInput) / slowdownTime);
        //move body on direction vector
        transform.Translate (new Vector3(xAxis*2, 0, zAxis*2) * Time.deltaTime, Space.Self);
        //apply velocity to physics body for gravity relationship
        rigBod.velocity += transform.forward * moveSpeed;
    }

    //local gravity force
    public float myGravity = 500f;

    void FixedUpdate () {
        //Check if all checkpoints met
        if (numCollected > 4 && collected == false)
        {
            collected = true;
        }
        //Check if player should have falling animation
        animator.SetBool("IsGrounded", floored);
        //handle movement
        ProcessMove();
        //Container for surface to influence gravity
        RaycastHit gravHit;

        if (Physics.Raycast(transform.position, -transform.up, out gravHit))
        {
            //Check if player is off the ground by minimum distance
            if(gravHit.distance > 0.1)
            {
                //Apply gravity to the player
                rigBod.velocity += -transform.up * myGravity * Time.deltaTime;
            }
            
        }
    }

    //Collision check to see if player is in contact with ground
    void OnCollisionStay(Collision col)
    {
        Floor();
    }
    
}
