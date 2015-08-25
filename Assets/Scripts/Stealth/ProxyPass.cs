using UnityEngine;
using System.Collections;

public class ProxyPass : MonoBehaviour 
{
	public StealthAnimation stealthAnim;
	
	public void finishtransform()
	{
		stealthAnim.postAnimationHide ();
	}
}
