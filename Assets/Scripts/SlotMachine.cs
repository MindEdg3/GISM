using UnityEngine;
using System.Collections;

public class SlotMachine : MonoBehaviour
{
	public int[] steps;
	public SlotReel[] reels;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void RollReels ()
	{
		if (reels.Length <= steps.Length) {
			for (int i = 0; i < reels.Length; i++) {
				reels [i].RandomizeReelUVs ();
				reels [i].Roll (steps [i]);
			}
		}
	}
	
	void OnMouseDown ()
	{
		Camera.main.animation.clip = Camera.main.animation.GetClip ("Lever Focus");
		Camera.main.animation.Play ();
	}
}
