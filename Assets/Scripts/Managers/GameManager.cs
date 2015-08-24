using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//Enviroment Variables
	public GameObject character;
	public SkinnedMeshRenderer body;
	public SkinnedMeshRenderer scarf;
	public MeshRenderer horns;

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
	public void killCharacter(float t) {
		Debug.Log ("killed!");

		if (!MovementHandler.getInstance().checkIfDead())
		{
			if (StealthManager.getInstance ().isHidden ()) 
			{
				StealthManager.getInstance ().show();
			}

			MovementHandler.getInstance ().setMovementLock (true);
			MovementHandler.getInstance ().playDeath ();
			Invoke ("actuallyKill", t);
		}
	}

	private void actuallyKill()
	{
		this.resetPositionToLastCheckpoint ();
		MovementHandler.getInstance ().playRespawn();
	}

	private void resetPositionToLastCheckpoint() {
		character.transform.position = CheckpointManager.getInstance ().lastCheckpoint ().position;
	}

}
