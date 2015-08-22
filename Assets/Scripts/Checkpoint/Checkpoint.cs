using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	private bool activated;

	void Awake() {
		activated = false;
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == GameManager.CHARACTER_TAG) {
			activated = true;
		}
	}

	public void activate() {
		activated = true;
	}

	public bool hasBeenActivated() {
		return activated;
	}

}
