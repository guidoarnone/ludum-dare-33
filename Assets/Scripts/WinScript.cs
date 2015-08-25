using UnityEngine;
using System.Collections;

public class WinScript : MonoBehaviour {

	public AudioSource Win;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{



	}
	void OnTriggerEnter(Collider c) {
		if (c.tag == GameManager.CHARACTER_TAG) 
		{
			Win.Play();
		}
	}
}
