using UnityEngine;
using System.Collections;

//UGLY CODE WARNING :'(

public class CameraController : MonoBehaviour 
{
	public static CameraController Instance;
	public int collisionLayermask = 10;
	public Transform TargetLookAt;
	public float Distance = 10.0f;
	public float DistanceMin = 2f;
	public float DistanceMax = 10f;
	public float DistanceSmooth = 0.05f;
	public float DistanceResumeSmooth = 1f;

	private float VelDistance = 0.0f;
	private float MouseX = 0.0f;
	private float MouseY = 0.0f;
	private float StartDistance = 0.0f;
	private float DesiredDistance = 0.0f;
	private float VelX = 0.0f;
	private float VelY = 0.0f;
	private float VelZ = 0.0f;
	private Vector3 Position = Vector3.zero;

	public float X_MouseSensitivity = 5.0f;
	public float Y_MouseSensitivity = 5.0f;
	public float X_ControllerSensitivity = 2.0f;
	public float Y_ControllerSensitivity = 2.0f;
	public float MouseWheelSensitivity = 15.0f;
	public float Y_MinLimit = 5f; //5
	public float Y_MaxLimit = 80.0f; //80
	public float OcclusionDistanceStep = 0.1f;
	public int MaxOcclusionChecks = 100;

	private float distanceSmooth = 0f;
	private float PreOccludedDistance = 0f;
	
	public float XSmooth = 0.05f;
	public float YSmooth = 0.05f;

	private Vector3 DesiredPosition = Vector3.zero;
	
	void Awake () 
	{
		Instance = this;
	}
	
	void Start () 
	{
		Camera.main.farClipPlane = 3000f;
		Distance = Mathf.Clamp (Distance,DistanceMin,DistanceMax);
		StartDistance = Distance;
		Reset ();
	}
	
	void LateUpdate ()
	{
		HandlePlayerInput ();
		
		int counter = 0;
		
		do
		{
			CalculateDesiredPosition ();
			counter++;
		} 
		while(CheckIfOccluded(counter));
		
		UpdatePosition ();
	}
	
	public void HandlePlayerInput()
	{
		var DeadZone = 0.1;
		
		MouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
		MouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
		
		MouseY = Helper.ClampAngle(MouseY, Y_MinLimit, Y_MaxLimit);
		
		if (Input.GetAxis("Mouse ScrollWheel") > DeadZone || Input.GetAxis("Mouse ScrollWheel") < -DeadZone)
		{
			DesiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel")*MouseWheelSensitivity, DistanceMin, DistanceMax);
			PreOccludedDistance = DesiredDistance;
			distanceSmooth = DistanceSmooth;
		}
	}
	
	void CalculateDesiredPosition()
	{
		ResetDesiredDistance ();
		Distance = Mathf.SmoothDamp (Distance, DesiredDistance, ref VelDistance, distanceSmooth);
		
		DesiredPosition = CalculatePosition (MouseX, MouseY, Distance);
	}
	
	Vector3 CalculatePosition(float RotationX, float RotationY, float Distance)
	{
		Vector3 Direction = new Vector3 (0, 0, -Distance);
		Quaternion Rotation = Quaternion.Euler (RotationY, RotationX, 0);
		return TargetLookAt.position + Rotation * Direction;
	}
	
	bool CheckIfOccluded(int count)
	{
		bool isOccluded = false;
		
		float nearestDistance = CheckCameraPoints(TargetLookAt.position, DesiredPosition);
		
		if (nearestDistance != -1)
		{
			if (count < MaxOcclusionChecks)
			{
				isOccluded = true;
				Distance -= OcclusionDistanceStep;
				
				if(Distance < 0.25f)
				{
					Distance = 0.25f;
				}
			}
			
			else
			{
				Distance = nearestDistance - Camera.main.nearClipPlane;
			}
			
			DesiredDistance = Distance;
			distanceSmooth = DistanceResumeSmooth;
		}
		
		return isOccluded;
	}
	
	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		float nearestDistance = -1f;
		int layerMask = 1 << collisionLayermask;
		
		RaycastHit hitInfo;
		
		Helper.ClipPlanePoints CPP = Helper.ClipPlaneAtNear (to);
		
		//Draw lines in the editor to make it easier to visualize this was my mechanism to debug or test blah blah blah
		
		Debug.DrawLine (from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, Color.red);
		
		Debug.DrawLine (CPP.BottomLeft, CPP.BottomLeft + transform.forward * 0.1f);
		Debug.DrawLine (CPP.BottomRight, CPP.BottomRight + transform.forward * 0.1f);
		Debug.DrawLine (CPP.UpperLeft, CPP.UpperLeft + transform.forward * 0.1f);
		Debug.DrawLine (CPP.UpperRight, CPP.UpperRight + transform.forward * 0.1f);
		
		Debug.DrawLine (from, CPP.BottomLeft);
		Debug.DrawLine (from, CPP.BottomRight);
		Debug.DrawLine (from, CPP.UpperLeft);
		Debug.DrawLine (from, CPP.UpperRight);
		
		Debug.DrawLine (CPP.UpperRight, CPP.UpperLeft);
		Debug.DrawLine (CPP.BottomRight, CPP.BottomLeft);
		
		Debug.DrawLine (CPP.UpperRight, CPP.BottomRight);
		Debug.DrawLine (CPP.UpperLeft, CPP.BottomLeft);
		
		if (Physics.Linecast(from, CPP.BottomLeft, out hitInfo, layerMask))
		{
			nearestDistance = hitInfo.distance;
		}
		
		if (Physics.Linecast(from, CPP.BottomRight, out hitInfo, layerMask))
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		
		if (Physics.Linecast(from, CPP.UpperLeft, out hitInfo, layerMask))
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		
		if (Physics.Linecast(from, CPP.UpperRight, out hitInfo, layerMask))
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		
		if (Physics.Linecast(from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, out hitInfo, layerMask))
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		
		return nearestDistance;
	}
	
	void ResetDesiredDistance()
	{
		if (DesiredDistance < PreOccludedDistance)
		{
			Vector3 pos = CalculatePosition(MouseY, MouseX, PreOccludedDistance);
			float nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);
			
			if (nearestDistance == -1 || nearestDistance > PreOccludedDistance)
			{
				DesiredDistance = PreOccludedDistance;
			}
		}
	}
	
	void UpdatePosition()
	{
		var PosX = Mathf.SmoothDamp (Position.x, DesiredPosition.x, ref VelX, XSmooth);
		var PosY = Mathf.SmoothDamp (Position.y, DesiredPosition.y, ref VelY, YSmooth);
		var PosZ = Mathf.SmoothDamp (Position.z, DesiredPosition.z, ref VelZ, XSmooth);
		
		Position = new Vector3 (PosX, PosY, PosZ);
		
		transform.position = Position;
		transform.LookAt (TargetLookAt);
	}
	
	void Reset ()
	{
		MouseX = 0.0f;
		MouseY = 10.0f;
		Distance = StartDistance;
		DesiredDistance = Distance;
		PreOccludedDistance = Distance;
	}
	
	public static void ExistingOrCreateNewMainCamera()
	{
		GameObject TempCamera;
		Transform targetLookAt;
		CameraController MyCamera;
		
		if (Camera.main != null)
		{
			TempCamera = Camera.main.gameObject;
		}
		
		else
		{
			TempCamera = new GameObject("Main Camera");
			TempCamera.AddComponent<Camera>();
			TempCamera.AddComponent<AudioListener>();
			TempCamera.AddComponent<AudioSource>();
			TempCamera.AddComponent<FlareLayer>();
			TempCamera.tag = ("MainCamera");
		}
		
		TempCamera.AddComponent <CameraController>();
		MyCamera = TempCamera.GetComponent ("CameraController") as CameraController;
		targetLookAt = MovementHandler.getInstance().lookAt;
		
		if (targetLookAt == null)
		{
			GameObject targetObject = new GameObject("cameraTarget");
			targetLookAt = targetObject.transform;
			targetLookAt.transform.parent = MovementHandler.getInstance().gameObject.transform;
			targetLookAt.transform.localPosition = new Vector3(0f, 0.25f, 0f);
		}
		
		MyCamera.TargetLookAt = targetLookAt.transform;
	}

	public Vector3 getViewVector()
	{
		return  new Vector3 (transform.forward.x, 0, transform.forward.z).normalized;
	}
}
