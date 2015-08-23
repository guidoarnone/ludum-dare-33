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
		Debug.Log("seen!");
		this.resetPositionToLastCheckpoint ();
	}

	//TODO
	public void killCharacter() {
		Debug.Log ("killed!");
		this.resetPositionToLastCheckpoint ();
	}

	private void resetPositionToLastCheckpoint() {
		character.transform.position = CheckpointManager.getInstance ().lastCheckpoint ().position;
	}

}
