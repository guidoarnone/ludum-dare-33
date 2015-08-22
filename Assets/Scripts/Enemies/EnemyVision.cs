using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour {

	void OnTriggerEnter(Collider c) {
		if (c.tag == GameManager.CHARACTER_TAG) {
			GameManager.getInstance().characterWasSeen();
		}
	}

}
