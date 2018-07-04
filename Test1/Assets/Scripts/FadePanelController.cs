using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour {

    public Animator panelAnim;
    public Animator GameInfoAnim;
    public Animator TryAgainAnim;
    public Animator newGameAnim;
    public Animator WinAnim;
    public Board board;
    private GoalManager goalManager;
    private ScoreManager scoreManager;

    //private Board board;

    public void OK()
    {
        if (panelAnim != null && GameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            GameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStartCo());
        }
    }

    public void Restart()
    {
        if(newGameAnim != null && panelAnim != null)
        {
            newGameAnim.SetBool("Out", true);
            panelAnim.SetBool("Out", true);
            panelAnim.SetBool("Game Over", false);
            //GameInfoAnim.SetBool("Out", false);
            //board.SetUp();
            //GameIntroPanel.SetActive(true);
            board = FindObjectOfType<Board>();
            goalManager = FindObjectOfType<GoalManager>();
            board.ShuffleBoard();
            goalManager.RestartGoals();
            goalManager.UpdateGoals();
            scoreManager.score1 = 0;
            //scoreManager.UpdateBar1();
        }
    }

    public void GameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);
    }

    public void LostnewGame()
    {
        panelAnim.SetBool("Out", false);
        TryAgainAnim.SetBool("Out", true);
        Debug.Log("new game");


    }
    public void WinnewGame()
    {
        panelAnim.SetBool("Out", false);
        WinAnim.SetBool("Out", true);
        Debug.Log("new game");


    }
    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f) ;
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }

}
