using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//Enviroment Variables
	public GameObject character;
	public const string CHARACTER_TAG = "Character";

	private static GameManager self;

	void Awake() {
		self = this;
	}

	public static GameManager getInstance() {
		return self;
	}

	//TODO
	public void characterWasSeen() {
		if(!StealthManager.getInstance().isHidden()){
			Debug.Log("seen!");
			this.resetPositionToLastCheckpoint ();
		}
	}

	//TODO
	public void killCharacter() {
		Debug.Log ("killed!");
		if (StealthManager.getInstance ().isHidden ()) {
			StealthManager.getInstance ().show();
		}
		this.resetPositionToLastCheckpoint ();
	}

	private void resetPositionToLastCheckpoint() {
		character.transform.position = CheckpointManager.getInstance ().lastCheckpoint ().position;
	}

}
