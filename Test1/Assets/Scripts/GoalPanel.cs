﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour {

    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisStirng;

	// Use this for initialization
	void Start () {
        Setup();
	}
	
	// Update is called once per frame
	void Setup () {
        thisImage.sprite = thisSprite;
        thisText.text = thisStirng;
	}
}
