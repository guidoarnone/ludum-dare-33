using UnityEngine;
using System.Collections;

public class DestroyT : MonoBehaviour
{
	public float time;

	void Start () 
	{
		Destroy (gameObject, time);
	}
}
