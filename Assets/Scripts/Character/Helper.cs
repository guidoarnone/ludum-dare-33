using UnityEngine;

public static class Helper
{
	public struct ClipPlanePoints
	{
		public Vector3 UpperRight;
		public Vector3 UpperLeft;
		public Vector3 BottomRight;
		public Vector3 BottomLeft;
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		angle = angle % 360; //Tried something different if its not working come back
		angle = Mathf.Clamp (angle, min, max);
		return angle;
	}
	
	public static float PickClosest(float position, float min, float max)
	{
		float minD = Mathf.Abs(position - min);
		float maxD = Mathf.Abs(position - max);
		if (minD < maxD) {return min;}
		else {return max;}
	}
	
	public static Vector2 SnapToGrid(float X, float Z, float TowerSize)
	{
		float max = Mathf.RoundToInt (X / TowerSize) * TowerSize;
		float min = max - TowerSize;
		X = PickClosest (X, min, max) + TowerSize / 2;
		max = Mathf.RoundToInt (Z / TowerSize) * TowerSize;
		min = max - TowerSize;
		Z = PickClosest (Z, min, max) + TowerSize / 2;
		
		return new Vector2 (X, Z);
	}
	
	public static ClipPlanePoints ClipPlaneAtNear(Vector3 Pos)
	{
		ClipPlanePoints CPP = new ClipPlanePoints ();
		
		if (Camera.main == null)
		{
			return CPP;
		}
		
		Transform T = Camera.main.transform;
		float HalfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
		float aspect = Camera.main.aspect;
		float distance = Camera.main.nearClipPlane;
		float height = distance * Mathf.Tan (HalfFOV);
		float width = height * aspect;
		
		CPP.BottomRight = Pos + T.right * width;
		CPP.BottomRight -= T.up * height;
		CPP.BottomRight += T.forward * distance;
		
		CPP.BottomLeft = Pos - T.right * width;
		CPP.BottomLeft -= T.up * height;
		CPP.BottomLeft += T.forward * distance;
		
		CPP.UpperRight = Pos + T.right * width;
		CPP.UpperRight += T.up * height;
		CPP.UpperRight += T.forward * distance;
		
		CPP.UpperLeft = Pos - T.right * width;
		CPP.UpperLeft += T.up * height;
		CPP.UpperLeft += T.forward * distance;
		
		return CPP;
	}
}

