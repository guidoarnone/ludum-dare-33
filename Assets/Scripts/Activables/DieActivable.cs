using UnityEngine;
using System.Collections;

public class DieActivable : MonoBehaviour, Activable {

	public void activate() {
		GameManager.getInstance ().killCharacter();
	}
}
