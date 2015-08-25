using UnityEngine;
using System.Collections;

public class StealthAnimation : MonoBehaviour {

	public GameObject particles;
	private GameObject hideObject;

	public void hide(GameObject hideObject) {
		MovementHandler.getInstance ().setMovementLock (true);
		MovementHandler.getInstance ().anim.SetTrigger ("Transformation");
		this.hideObject = hideObject;
	}

	public void show() {
		Destroy (hideObject);
		Instantiate (particles, transform.position, Quaternion.identity);
		GameManager.getInstance ().scarf.enabled = true;
		GameManager.getInstance ().body.enabled = true;
		GameManager.getInstance ().horns.enabled = true;
		MovementHandler.getInstance ().setMovementLock (false);
		Invoke ("delayThingy", 1f);
	}

	private void delayThingy()
	{
		StealthManager.getInstance().setHidden(StealthState.PLAINSIGHT);
	}

	//Triggered from animation
	public void postAnimationHide() {
		Instantiate (particles, transform.position, Quaternion.identity);
		GameManager.getInstance ().scarf.enabled = false;
		GameManager.getInstance ().body.enabled = false;
		GameManager.getInstance ().horns.enabled = false;
		hideObject = (GameObject)Instantiate(hideObject, GameManager.getInstance ().character.transform.position, Quaternion.identity);
		StealthManager.getInstance().setHidden(StealthState.HIDDEN);
	}
	
}
