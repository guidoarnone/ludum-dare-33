using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnCollision : MonoBehaviour {

	public GameObject targetObject;
	private System.Type activableType = typeof(Activable);

	void OnTriggerEnter(Collider c) {
		Component[] components = targetObject.GetComponents(activableType);
		foreach (Component component in components) {
			(component as Activable).activate();
		}
	}
		
}
