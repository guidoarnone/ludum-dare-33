﻿using UnityEngine;
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

	private bool canJump;

	private bool isMoving;
	private bool isJumping;
	private bool isFalling;

	private float lastY;
	private Vector3 movement;
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
		checkInput();
		setFlags ();
		applyGravity ();
		move ();
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
			if (canJump)
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

	private void move()
	{
		if (isMoving)
		{
		Vector3 nonYMovement = new Vector3 (movement.x, 0, movement.z);
		transform.LookAt (transform.position + nonYMovement);
		}

		characterController.Move(movement * Time.deltaTime * moveSpeed * speedModifier);
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
		}

		lastY = movement.y;
	}

	private void jump()
	{
		movement +=  new Vector3 (0, jumpForce, 0);
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

		int layermask;

		int layer = 8;
		layermask = 1 << layer;

		Vector3 point1 = transform.position + Vector3.up * 0.2f + characterController.radius * transform.forward / 2f + characterController.radius * transform.right / 2;
		Vector3 point2 = transform.position + Vector3.up * 0.2f - characterController.radius * transform.forward / 2f + characterController.radius * transform.right / 2;
		Vector3 point3 = transform.position + Vector3.up * 0.2f + characterController.radius * transform.forward / 2f - characterController.radius * transform.right / 2;
		Vector3 point4 = transform.position + Vector3.up * 0.2f - characterController.radius * transform.forward / 2f - characterController.radius * transform.right / 2;

		if(testRay(point1, layermask))
		{
			canJump = true;
			return;
		}
		else
		{
			canJump = false;
		}
		if(testRay(point2, layermask))
		{
			canJump = true;
			return;
		}
		else
		{
			canJump = false;
		}
		if(testRay(point3, layermask))
		{
			canJump = true;
			return;
		}
		else
		{
			canJump = false;
		}
		if(testRay(point4, layermask))
		{
			canJump = true;
			return;
		}
		else
		{
			canJump = false;
		}
	}

	bool testRay(Vector3 v, int l)
	{
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

}
