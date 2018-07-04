using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType {

    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements {

    public GameType gameType;
    public int counterValue;
}



public class EndGameManager : MonoBehaviour {

    public GameObject moveLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public GameObject gameInfoPanel;
    public GameObject gameInfoPanelNewGame;
    public Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private Board board;
    private float timerSeconds;

	// Use this for initialization
	void Start () {
        board = FindObjectOfType<Board>();
        SetupGame();
	}
	
	// Update is called once per frame
	void SetupGame () {

        currentCounterValue = requirements.counterValue;
		if(requirements.gameType == GameType.Moves)
        {
            moveLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            moveLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
	}

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
                
            }
        }
        
        }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("Lose!");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }
    public void LostrestartGame()
    {
        
        
        FadePanelController newFade = FindObjectOfType<FadePanelController>();
        newFade.LostnewGame();
        StartCoroutine(WaitCo());


    }
    public void WinrestartGame()
    {


        FadePanelController newFade = FindObjectOfType<FadePanelController>();
        newFade.WinnewGame();
        StartCoroutine(WaitCo());


    }
    IEnumerator WaitCo()
    {
        yield return new WaitForSeconds(1f);
        gameInfoPanelNewGame.SetActive(true);
    }


    private void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
