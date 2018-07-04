using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    private Board board;
    public Text scoreText1;
    public Text scoreText2;
    public Text scoreText3;
    public Text scoreText4;
    public Text scoreText5;
    public Text scoreText6;
    public int score1;
    public int score2;
    public int score3;
    public int score4;
    public int score5;
    public int score6;
    public Image scoreBar1;
    public Image scoreBar2;
    public Image scoreBar3;
    public Image scoreBar4;
    public Image scoreBar5;
    public Image scoreBar6;


    // Use this for initialization
    void Start () {
        board = FindObjectOfType<Board>();
        
        UpdateBar1();
        UpdateBar2();
        UpdateBar3();
        UpdateBar4();
        UpdateBar5();
        UpdateBar6();
    }
	
	// Update is called once per frame
	void Update () {
        scoreText1.text = "" + score1;
        scoreText2.text = "" + score2;
        scoreText3.text = "" + score3;
        scoreText4.text = "" + score4;
        scoreText5.text = "" + score5;
        scoreText6.text = "" + score6;
    }

    public void IncreaseScore1(int amountToIncrease)
    {

        score1 += amountToIncrease;
        UpdateBar1();
    }
    
    public void IncreaseScore2(int amountToIncrease)
    {

        
        score2 += amountToIncrease;
        UpdateBar2();
    }
    
    public void IncreaseScore3(int amountToIncrease)
    {


        score3 += amountToIncrease;
        UpdateBar3();
    }
    public void IncreaseScore4(int amountToIncrease)
    {


        score4 += amountToIncrease;
        UpdateBar4();
    }
    public void IncreaseScore5(int amountToIncrease)
    {


        score5 += amountToIncrease;
        UpdateBar5();
    }
    public void IncreaseScore6(int amountToIncrease)
    {


        score6 += amountToIncrease;
        UpdateBar6();
    }





    private void UpdateBar1()
    {
        if (board != null && scoreBar1 != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar1.fillAmount = (float)score1 / (float)10;
          
        }
    }
    private void UpdateBar2()
    {
        if (board != null && scoreBar2 != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar2.fillAmount = (float)score2 / (float)10;

        }
    }
    private void UpdateBar3()
    {
        if (board != null && scoreBar3 != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar3.fillAmount = (float)score3 / (float)10;

        }
    }
    private void UpdateBar4()
    {
        if (board != null && scoreBar4 != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar4.fillAmount = (float)score4 / (float)10;

        }
    }
    private void UpdateBar5()
    {
        if (board != null && scoreBar5 != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar5.fillAmount = (float)score5 / (float)10;

        }
    }
    private void UpdateBar6()
    {
        if (board != null && scoreBar6 != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar6.fillAmount = (float)score6 / (float)10;

        }
    }
}
