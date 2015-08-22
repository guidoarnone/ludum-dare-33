using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour {

	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == GameManager.CHARACTER_TAG) {
			GameManager.getInstance().characterWasSeen();
		}
	}

}
