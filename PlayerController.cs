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

    public Transform cameraBox;
    public Transform camHold;
    public Transform cam;

    public float mass = 0.3f;
	public float moveAcceleration = 0.3f;
	public float maxMoveSpeed = 2.0f;
	public float slowdownTime = 0.3f;
    

    public Animator animator;
    public Rigidbody rigBod;
	private Vector3 acceleration = new Vector3(0f, 0f, 0f);
	private Vector3 speed = new Vector3 (0f, 0f, 0f);
	private float moveSpeed = 0f;
	private Vector3 lastPosition;

	public bool floored = false;

    private float lastInput;

    public Transform endGoal;

    public bool collected = false;

    public int numCollected = 0;

	//Used to debug vector changes
	private Vector3 lastDiff;
	
	void Start () {
        animator = GetComponent<Animator>();
        rigBod = GetComponent<Rigidbody>();
		lastInput = Time.time;
		lastDiff = new Vector3(0, 0, 0);
	}
	

	void Floor () {
		floored = true;
		SendMessage("startStickingDown");

	}

    public void ChangeTarget(Transform target)
    {
        shootPoint.GetComponent<targetGoal>().target = target;
    }

	void Jump () {
        
        if (floored ) {
            animator.SetTrigger("Jump");
            floored = false;
		}
	}

    void DoubleJump()
    {

        if (floored)
        {
            animator.SetTrigger("DoubleJump");
            floored = false;
        }
    }

    void ProcessMove () {
		float zAxis = Input.GetAxis ("Vertical");
		float xAxis = Input.GetAxis ("Horizontal");
        animator.SetFloat("InputVertical", Mathf.Clamp((Mathf.Abs(zAxis ) + Mathf.Abs(xAxis)),0,1));
        moveSpeed += Mathf.Clamp((Mathf.Abs (xAxis) + Mathf.Abs(zAxis)),0,1) * moveAcceleration * Time.deltaTime;
		if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
        animator.SetFloat("InputVertical", moveSpeed);

        if (Mathf.Abs (xAxis) > 0.2 || Mathf.Abs(zAxis) > 0.2) {
			lastInput = Time.time;
			Transform mainCamera = Camera.main.transform;

			Vector3 diff = (camHold.forward * zAxis + camHold.right * xAxis);
			diff.Normalize();
			diff = transform.InverseTransformDirection(diff);
			diff.y = 0;
			diff = transform.TransformDirection(diff);
			transform.LookAt(transform.position + diff, transform.up);
			lastDiff = diff;
		}
		Debug.DrawRay (transform.position, lastDiff);
		moveSpeed = Mathf.Lerp(moveSpeed, 0, (Time.time - lastInput) / slowdownTime);
        transform.Translate (new Vector3(xAxis*2, 0, zAxis*2) * Time.deltaTime, Space.Self);
        rigBod.velocity += transform.forward * moveSpeed;
    }

    float myGravity = 500f;
    float jumpPower = 0f;
    float fireRate = 0f;
    float canFire = 0f;
    bool canDouble = true;
    public Rigidbody gravityBullet;
    public Transform shootPoint;

    void FixedUpdate () {

        if (numCollected > 4 && collected == false)
        {
            collected = true;
        }

        animator.SetBool("IsGrounded", floored);
        ProcessMove();
        
        canFire = Time.time;

        
        RaycastHit gravHit;

        if (Physics.Raycast(transform.position, -transform.up, out gravHit))
        {
            if(gravHit.distance > 0.1)
            {
                rigBod.velocity += -transform.up * myGravity * Time.deltaTime;
            }
            
        }
    }

    void OnCollisionStay(Collision col)
    {
        Floor();
    }
    
}
