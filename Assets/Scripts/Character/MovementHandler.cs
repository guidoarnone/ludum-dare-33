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
	public float maxStandAngle;
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

	private bool externalMovementLock;
	private bool canJump;
	private bool canMove;

	private bool isMoving;
	private bool isGrounded;
	private bool isJumping;
	private bool landed;
	private bool stopped;

	private float standNormal;
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
		canMove = true;
		externalMovementLock = false;

		//We're so sorry (not sorry at all)
		CameraController.ExistingOrCreateNewMainCamera();
	}

	void Update ()
	{
		//Uncomment afetr adding Animator component
		//animator = transform.GetComponent<Animator>;

		//DEBUG BLOCK
		Debug.Log (standNormal);
		_Y = movement.y;
		_isMoving = isMoving;
		_isJumping = isJumping;
		_isGrounded = isGrounded;
		_landed = landed;
		_stopped = stopped;
		_animState = animState.ToString();
		//TESTING END

		setFlags ();
		checkCanMove ();
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

		Vector3 dir = Vector3.down;

		standNormal = 180f;
		RaycastHit r;
		Physics.Raycast (point1, dir, out r,groundedBias, layermask);
		saveMinorStandNormal(Vector3.Angle (r.normal, Vector3.up));
		if(testRay(point1, dir, layermask, groundedBias) && (Vector3.Angle(r.normal, Vector3.up) < maxStandAngle))
		{
			isGrounded = true;
			return;
		}

		Physics.Raycast (point2, dir, out r,groundedBias, layermask);
		saveMinorStandNormal(Vector3.Angle (r.normal, Vector3.up));
		if(testRay(point2, dir, layermask, groundedBias) && (Vector3.Angle(r.normal, Vector3.up) < maxStandAngle))
		{
			isGrounded = true;
			return;
		}

		Physics.Raycast (point3, dir, out r,groundedBias, layermask);
		saveMinorStandNormal(Vector3.Angle (r.normal, Vector3.up));
		if(testRay(point3, dir, layermask, groundedBias) && (Vector3.Angle(r.normal, Vector3.up) < maxStandAngle))
		{
			isGrounded = true;
			return;
		}

		Physics.Raycast (point4, dir, out r,groundedBias, layermask);
		saveMinorStandNormal(Vector3.Angle (r.normal, Vector3.up));
		if(testRay(point4, dir, layermask, groundedBias) && (Vector3.Angle(r.normal, Vector3.up) < maxStandAngle))
		{
			isGrounded = true;
			return;
		}

		Physics.Raycast (point5, dir, out r,groundedBias, layermask);
		saveMinorStandNormal(Vector3.Angle (r.normal, Vector3.up));
		if(testRay(point5, dir, layermask, groundedBias) && (Vector3.Angle(r.normal, Vector3.up) < maxStandAngle))
		{
			isGrounded = true;
			return;
		}
		checkExcessNormal ();
		isGrounded = false;
		Debug.Log ("kinda works");
		return;
	}

	private void checkCanMove()
	{
		if (standNormal <= maxStandAngle && externalMovementLock == false)
		{
			canMove = true;
		}
		else
		{
			canMove = false;
		}
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
		
		if (!isGrounded)
		{
			if (movement.y > 0.1f)
			{
				int layermask;
				
				int layer = 8;
				layermask = 1 << layer;
				Vector3 point = transform.position + Vector3.up * characterController.height / 2 + Vector3.down * 0.1f;

				if (testRay(point, Vector3.up, layermask, groundedBias / 2))
				{
					movement.y = 0;
				}
			}

			movement.y -= gravityForce * Time.deltaTime;
			movement.y = Mathf.Clamp(movement.y, -maxFallSpeed, 100f);
		}

		else
		{
			if (lastY < -1f)
			{
				movement.y = 0;
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

		if(canMove)
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
		Debug.Log ("J");
		canJump = false;
	}

	bool testRay(Vector3 v, Vector3 d, int l, float bias)
	{
		Debug.DrawRay(v, d, Color.red);
		if(Physics.Raycast(v, d, bias, l))
		{
			return true;
		}
		return false;
	}

	void saveMinorStandNormal(float n)
	{
		if ( n < 90f && n < standNormal)
		{
			standNormal = n;
		}
	}

	void checkExcessNormal()
	{
		if (standNormal > 120f)
		{
			standNormal = 0;
		}
	}

	public void placeAt(Vector3 position)
	{
		gameObject.transform.position = position;
	}

	public void setMovementLock(bool b)
	{
		externalMovementLock = b;
	}

	public void deathTrigger(string cause)
	{

	}
}
