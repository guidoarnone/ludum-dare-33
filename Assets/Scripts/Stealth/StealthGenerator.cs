using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class StealthGenerator : MonoBehaviour {

	public List<GameObject> stealthObjects;
	private StealthManager manager;

	void OnTriggerStay() {
		if (Input.GetKeyDown(StealthManager.hideKey)) {
			if(StealthManager.getInstance ().canHide()) {
				StealthManager.getInstance ().hide (this.getStealthObject());
				Debug.Log("Hide");
			} 
			else if(StealthManager.getInstance().canShow()) 
			{
				Debug.Log("Show");
				StealthManager.getInstance().show();
			}
		}
	}

	private GameObject getStealthObject() {
		float random = Random.Range (0, 1f);
		int randomIndex = (int)Mathf.Round(random * (stealthObjects.Count -1)); 
		Debug.Log (stealthObjects[randomIndex].name);
		return stealthObjects[randomIndex];
	}
}
