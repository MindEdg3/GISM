using UnityEngine;
using System.Collections;

public class SlotMachineLever : MonoBehaviour
{
	public SlotMachine machine;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnMouseDown ()
	{
		animation.Play ();
		Camera.main.animation.clip = Camera.main.animation.GetClip ("Camera Focus");
		Camera.main.animation.Play ();
		machine.RollReels ();
	}
}
