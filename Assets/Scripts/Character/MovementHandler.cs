using UnityEngine;
using System.Collections;

public class MovementHandler : MonoBehaviour
{
	CharacterController characterController;

	public Transform lookAt;
	public float moveSpeed;
	private static MovementHandler self;

	private float speedModifier;

	public static MovementHandler getInstance() {
		return self;
	}

	void Start ()
	{
		characterController = GetComponent<CharacterController>();
		self = this; 
		//We're so sorry
		CameraController.ExistingOrCreateNewMainCamera();
	}

	void Update ()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		checkInput();

		Vector3 forward = CameraController.Instance.getViewVector();
		Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;

		Vector3 movement = forward * y + right * x;
		movement.Normalize();

		characterController.Move(movement * Time.deltaTime * moveSpeed * speedModifier);
	}

	private void checkInput()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speedModifier = 0.5f;
		}

		else
		{
			speedModifier = 1f;
		}
	}

	public void placeAt(Vector3 position)
	{
		gameObject.transform.position = position;
	}

}
