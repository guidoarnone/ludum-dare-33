using UnityEngine;
using System.Collections;

public class MovementHandler : MonoBehaviour
{
	CharacterController characterController;

	public Transform lookAt;
	public float moveSpeed;
	public float groundedBias;
	public float jumpForce;
	public float gravityForce;
	public float maxFallSpeed;
	public float deadZone;

	//DEBUG BLOCK
	public float _Y;
	public bool _isMoving;
	public bool _isGrounded;
	public bool _isJumping;
	public bool _landed;
	public bool _stopped;
	public string _animState;
	//TESTING END

	private bool canJump;
	private bool canMove;

	private bool isMoving;
	private bool isGrounded;
	private bool isJumping;
	private bool landed;
	private bool stopped;

	private float lastY;
	private float lastXZ;
	private Vector3 movement;
	private AnimationState.State animState;
	private Animator animator;

	private static MovementHandler self;

	private float speedModifier;

	public static MovementHandler getInstance() {
	return self;
	}

	void Start ()
	{
		characterController = GetComponent<CharacterController>();
		self = this;
		canJump = true;

		//We're so sorry (not sorry at all)
		CameraController.ExistingOrCreateNewMainCamera();
	}

	void Update ()
	{
		//Uncomment afetr adding Animator component
		//animator = transform.GetComponent<Animator>;
		//DEBUG BLOCK
		_Y = movement.y;
		_isMoving = isMoving;
		_isJumping = isJumping;
		_isGrounded = isGrounded;
		_landed = landed;
		_stopped = stopped;
		_animState = animState.ToString();
		//TESTING END

		setFlags ();
		checkInput();
		currentAnimationState ();
		applyGravity ();
		move ();
	}

	private void setFlags()
	{
		if (movement.x + movement.z < deadZone && movement.x + movement.z > -deadZone)
		{
			isMoving = false;
		}
		
		else
		{
			isMoving = true;
		}
		
		if (movement.y < 0.1f)
		{
			canJump = true;
			isJumping = false;
		}
		else
		{
			isJumping = true;
		}
		
		int layermask;
		
		int layer = 8;
		layermask = 1 << layer;
		
		Vector3 point1 = transform.position + Vector3.down * characterController.height / 2 + Vector3.up * 0.2f + characterController.radius * transform.forward + characterController.radius * transform.right;
		Vector3 point2 = transform.position + Vector3.down * characterController.height / 2 + Vector3.up * 0.2f - characterController.radius * transform.forward + characterController.radius * transform.right;
		Vector3 point3 = transform.position + Vector3.down * characterController.height / 2 + Vector3.up * 0.2f + characterController.radius * transform.forward - characterController.radius * transform.right;
		Vector3 point4 = transform.position + Vector3.down * characterController.height / 2 + Vector3.up * 0.2f - characterController.radius * transform.forward - characterController.radius * transform.right;
		Vector3 point5 = transform.position + Vector3.down * characterController.height / 2 + Vector3.up * 0.2f;
		
		if(testRay(point1, layermask))
		{
			isGrounded = true;
			return;
		}
		if(testRay(point2, layermask))
		{
			isGrounded = true;
			return;
		}
		if(testRay(point3, layermask))
		{
			isGrounded = true;
			return;
		}
		if(testRay(point4, layermask))
		{
			isGrounded = true;
			return;
		}
		if(testRay(point5, layermask))
		{
			isGrounded = true;
			return;
		}
		isGrounded = false;
		return;
	}

	private void checkInput()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		
		// to do: gouge out my eyes, then fix this junk
		Vector3 forward = CameraController.Instance.getViewVector();
		Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;
		
		movement = forward * y + right * x;
		movement.Normalize();

		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z))
		{
			if (canJump && isGrounded)
			{
				jump();
			}
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			speedModifier = 0.5f;
		}
		
		else
		{
			speedModifier = 1f;
		}
	}

	private void currentAnimationState ()
	{
		if (landed)
		{
			//setTrigger landed;
			landed = false;
		}

		if (stopped)
		{
			//setTrigger stopped;
			stopped = false;
		}

		if (!isGrounded)
		{
			if (isJumping)
			{
				animState = AnimationState.State.Jumping;
			}
			else
			{
				animState = AnimationState.State.Falling;
			}
		}
		else
		{
			if (Mathf.Abs(movement.x + movement.z) < 0.05f)
			{
				animState = AnimationState.State.Idle;
			}
			else if(Mathf.Abs(movement.x + movement.z) < (Time.deltaTime * moveSpeed / 3))
			{
				animState = AnimationState.State.Tiptoeing;
			}
			else if(Mathf.Abs(movement.x + movement.z) < (Time.deltaTime * moveSpeed /2))
			{
				animState = AnimationState.State.Walking;
			}
			else
			{
				animState = AnimationState.State.Running;
			}
		}
	}

	private void applyGravity ()
	{
		movement.y += lastY;
		
		if (!characterController.isGrounded)
		{
			movement.y -= gravityForce * Time.deltaTime;
		}
		else
		{
			movement.y = 0;
			if (lastY < -1f)
			{
				landed = true;
			}
		}
		
		lastY = movement.y;
	}

	private void move()
	{
		if (isMoving)
		{
		Vector3 nonYMovement = new Vector3 (movement.x, 0, movement.z);
		transform.LookAt (transform.position + nonYMovement);
		}

		if()
		{
			characterController.Move(movement * Time.deltaTime * moveSpeed * speedModifier);
			if (Mathf.Abs(movement.x) + Mathf.Abs(movement.z) < 0.05f && lastXZ > Time.deltaTime * moveSpeed * 0.75f && isGrounded)
			{
				stopped = true;
			}
		}

		lastXZ = Mathf.Abs(movement.x) + Mathf.Abs(movement.z);
	}
	
	private void jump()
	{
		movement += new Vector3 (0, jumpForce, 0);
		canJump = false;
	}

	bool testRay(Vector3 v, int l)
	{
		Debug.DrawRay(v, Vector3.down, Color.red);
		if(Physics.Raycast(v, Vector3.down, groundedBias, l))
		{
			return true;
		}
		return false;
	}

	public void placeAt(Vector3 position)
	{
		gameObject.transform.position = position;
	}

	public void deathTrigger(string cause)
	{

	}
}
