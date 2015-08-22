using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {
	
	public List<Transform> flags;
	public float moveSpeed;
	public float minDistanceToChangeFlags;
	
	private int currentFlagIndex;
	private Transform getCurrentFlag() {
		return flags[currentFlagIndex];
	}
	
	private void nextFlag() {
		int nextIndex = currentFlagIndex + 1;
		currentFlagIndex = nextIndex < flags.Count ? nextIndex : 0;
	}
	
	void Start () {
		currentFlagIndex = 0;
		this.lookAtCurrentFlag ();
	}
	
	void Update () {
		float moveDistance = moveSpeed * Time.deltaTime;
		transform.Translate (Vector3.forward * moveDistance, Space.Self);
		if(this.distanceToCurrentFlag() <= minDistanceToChangeFlags) {
			this.nextFlag();
			this.lookAtCurrentFlag();
		}
	}
	
	private float distanceToCurrentFlag() {
		return Vector3.Distance(transform.position, getCurrentFlag().position);
	}
	
	private void lookAtCurrentFlag() {
		transform.LookAt (this.getCurrentFlag ());
	}
	
}

