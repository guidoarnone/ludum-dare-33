using UnityEngine;
using System.Collections;

public class StealthManager : MonoBehaviour {

	private static StealthManager self;
	public const KeyCode hideKey = KeyCode.Q;
	private StealthState hidden;

	public static StealthManager getInstance() {
		return self;
	}

	void Start () {
		self = this;
		hidden = StealthState.PLAINSIGHT;
	}

	public bool canHide() {
		return hidden.Equals(StealthState.PLAINSIGHT);
	}

	public bool canShow() {
		return hidden.Equals(StealthState.HIDDEN);
	}

	public void hide(GameObject hideObject) {
		hidden = StealthState.TRANSITION;
		GameManager.getInstance ().character.GetComponent<StealthAnimation> ().hide (hideObject);
	}

	public void show() {
		hidden = StealthState.TRANSITION;
		GameManager.getInstance ().character.GetComponent<StealthAnimation> ().show ();
	}

	public void setHidden(StealthState s) {
		hidden = s;
	}

}
