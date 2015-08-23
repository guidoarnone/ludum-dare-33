using UnityEngine;
using System.Collections;

public class StealthAnimation : MonoBehaviour {

	private GameObject hideObject;

	public void hide(GameObject hideObject) {
		MovementHandler.getInstance ().setMovementLock (true);
		this.hideObject = hideObject;
		//TODO: animation logic
	}

	public void show() {
		Destroy (hideObject);
		//TODO: particles
		GameManager.getInstance ().character.GetComponent<MeshRenderer> ().enabled = true;
		MovementHandler.getInstance ().setMovementLock (false);
		StealthManager.getInstance().setHidden(StealthState.PLAINSIGHT);
	}

	//Triggered from animation
	public void postAnimationHide() {
		//TODO: particles
		GameManager.getInstance ().character.GetComponent<MeshRenderer> ().enabled = false;
		hideObject = (GameObject)Instantiate(hideObject, GameManager.getInstance ().character.transform.position, Quaternion.identity);
		StealthManager.getInstance().setHidden(StealthState.HIDDEN);
	}
	
}
