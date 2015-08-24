using UnityEngine;
using System.Collections;

public class AnimActivable : MonoBehaviour, Activable 
{
	public void activate() 
	{
		transform.GetComponent<Animator> ().SetTrigger ("Boom");
	}
}
