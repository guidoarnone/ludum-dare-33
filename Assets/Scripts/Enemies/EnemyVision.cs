using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour {
	
	public GameObject raycastOrigin;
	private const int charLayerMask = 1 << 9;
	private const int allLayerMask = 1 << 8;

	void OnTriggerStay(Collider c) {
		if (c.tag == GameManager.CHARACTER_TAG) {
			int raycastMask = charLayerMask | allLayerMask;
			RaycastHit r;
			if(Physics.Raycast(raycastOrigin.transform.position, this.getRaycastDirection(),out r, 100f, raycastMask)) {
				if(r.collider.tag == GameManager.CHARACTER_TAG) {
					GameManager.getInstance().characterWasSeen();
				}
			}
		}
	}

	Vector3 getRaycastDirection() {
		Vector3 charPosition = GameManager.getInstance().character.transform.position;
		return (charPosition - raycastOrigin.transform.position).normalized;
	}
	
}
