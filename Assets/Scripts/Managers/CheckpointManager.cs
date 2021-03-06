﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour {

	public List<GameObject> checkpoints;
	private static CheckpointManager self;

	private Checkpoint getCheckpoint(int checkpointIndex) {
		return checkpoints[checkpointIndex].GetComponent<Checkpoint>();
	}

	public static CheckpointManager getInstance() {
		return self;
	}

	void Start () {
		self = this;
		this.getCheckpoint(0).activate();
	}

	public Transform lastCheckpoint() {
		int lastIndex = 0;
		for(int i = 0; i < checkpoints.Count; i++) {
			if(this.getCheckpoint(i).hasBeenActivated()) {
				lastIndex = i;
			}
		}
		return checkpoints[lastIndex].transform;
	}

}
