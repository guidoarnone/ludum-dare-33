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
	public int checkRayNumber;
	public float deadZone;

	//DEBUG BLOCK
	public bool _isMoving;
	public bool _isGrounded;
	public bool _isJumping;
	public bool _isSliding;
	public bool _landed;
	public bool _stopped;
	public string _animState;
	public float _X;
	public float _Z;
	public float _Y;
	//TESTING END

	private bool externalMovementLock;
	private bool canJump;
	private bool canMove;

	private bool isMoving;
	private bool isGrounded;
	private bool isJumping;
	private bool isSliding;
	private bool landed;
	private bool stopped;
	
	private float acceleration;
	private float standNormalAngle;
	private float lastY;
	private float lastXZ;
	private Vector3 standNormal;
	private Vector3 movement;
	private AnimationState.State lastAnimState;
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
		_Y = movement.y;
		_isMoving = isMoving;
		_isJumping = isJumping;
		_isGrounded = isGrounded;
		_isSliding = isSliding;
		_landed = landed;
		_stopped = stopped;
		_animState = animState.ToString();
		//TESTING END

		setFlags ();
		checkCanMove ();
		Debug.Log (standNormalAngle);

		checkInput();
		currentAnimationState ();
		applyGravity ();
		move ();

		lastAnimState = animState;
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

		checkNormals ();
	}

	private void checkNormals()
	{
		int layermask;
		
		int layer = 8;
		layermask = 1 << layer;
		
		Vector3[] checkRays = new Vector3[checkRayNumber + 1];
		
		for (int i = 0; i < checkRayNumber; i++)
		{
			float angle = 360f / checkRayNumber * i;
			Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.right;
			Vector3 v = (transform.position + Vector3.down * characterController.height / 2 + Vector3.up * characterController.height / 3f + characterController.radius * 1.2f * direction);
			checkRays[i] = v;
		}
		
		checkRays[checkRayNumber] = transform.position + Vector3.down * characterController.height / 2 + Vector3.up * characterController.height / 6;
		Vector3 dir = Vector3.down;
		
		standNormalAngle = 180f;
		RaycastHit r;
		
		for (int i = 0; i < checkRayNumber + 1; i++)
		{
			//DEBUG
			bool l = testRay(checkRays[i], dir, layermask, groundedBias);
			if (Physics.Raycast (checkRays[i], dir, out r, groundedBias, layermask) && (Vector3.Angle(r.normal, Vector3.up) < maxStandAngle))
			{
				isGrounded = true;
				saveMinorStandNormal(r.normal, Vector3.Angle (r.normal, Vector3.up));
				return;
			}
			
			saveMinorStandNormal(r.normal, Vector3.Angle (r.normal, Vector3.up));
		}
		
		checkExcessNormal ();
		isGrounded = false;
		return;
	}

	private void checkCanMove()
	{
		if ((standNormalAngle <= maxStandAngle && externalMovementLock == false) || movement.y > 0.1f)
		{
			canMove = true;
			isSliding = false;
		}
		else
		{
			isSliding = true;
			canMove = false;
		}
	}

	/*public void startDetransform()
	{
		Invoke("actualDetransform", 0.5f);
	}

	public void actualDetransform()
	{
		externalMovementLock = false;
		isTransformed = false;
	}

	public void startTransform()
	{
		Invoke("actualTransform", 0.5f);
	}

	public void actualTransform()
	{
		isTransformed = true;
	}*/

	private void checkInput()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		
		// to do: gouge out my eyes, then fix this junk
		Vector3 forward = CameraController.Instance.getViewVector();
		Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;
		
		movement = (forward * y + right * x);
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
			else if (isSliding)
			{
				animState = AnimationState.State.Sliding;
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
			if (lastAnimState == AnimationState.State.Sliding && animState != lastAnimState)
			{
				movement.y = 0;
			}

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
				if (lastY < maxFallSpeed * -0.75f)
				{
					externalMovementLock = true;
					Invoke("revokeMoveLock", 0.25f);
				}
			}
		}
		
		lastY = movement.y;
	}

	private void revokeMoveLock()
	{
		externalMovementLock = false;
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
		else if (externalMovementLock == false && isSliding)
		{
			Vector3 slideMove = new Vector3(standNormal.x, -standNormalAngle / 10f, standNormal.z) / Mathf.Sqrt(standNormalAngle) * 5;
			characterController.Move(slideMove * Time.deltaTime * moveSpeed);
		}

		lastXZ = Mathf.Abs(movement.x) + Mathf.Abs(movement.z);
	}
	
	private void jump()
	{
		movement += new Vector3 (0, jumpForce, 0);
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

	void saveMinorStandNormal(Vector3 r, float n)
	{
		if ( n < 90f && n < standNormalAngle)
		{
			standNormalAngle = n;
			standNormal = r;
		}
	}

	void checkExcessNormal()
	{
		if (standNormalAngle > 120f)
		{
			standNormalAngle = 0;
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
