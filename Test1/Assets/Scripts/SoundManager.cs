using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource[] destroyNoice;

    public void PlayRandomDestroyNoice()
    {
        //choose a random number
        int clipToPlay = Random.Range(0, destroyNoice.Length);
        //play the clip
        destroyNoice[clipToPlay].Play();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
