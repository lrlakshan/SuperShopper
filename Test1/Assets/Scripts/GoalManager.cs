using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour {

    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalIntroParentNewGame;
    public GameObject goalGameParent;
    private EndGameManager endGame;

	// Use this for initialization
	void Start () {
        endGame = FindObjectOfType<EndGameManager>();
        SetupGoals();
	}

    public void SetupGoals()
    {
        for(int i=0; i < levelGoals.Length; i++)
        {
            //create a new goal panel
            //create game Intro panel
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisStirng = "0/" + levelGoals[i].numberNeeded;

            GameObject goalNewGame = Instantiate(goalPrefab, goalIntroParentNewGame.transform.position, Quaternion.identity);
            goalNewGame.transform.SetParent(goalIntroParentNewGame.transform);
            panel = goalNewGame.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisStirng = "0/" + levelGoals[i].numberNeeded;

            //create game goal panel
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisStirng = "0/" + levelGoals[i].numberNeeded;
        }
    }
	
	public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for(int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
                Debug.Log("You won the first item");
                endGame.WinGame();
            }
            
        }
        if(goalsCompleted >= levelGoals.Length)
        {
            if(endGame != null)
            {
                //endGame.WinGame();
            }
            Debug.Log("Won");
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            if(goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }

    }
    public void RestartGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            levelGoals[i].numberCollected = 0;
            //currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
        }
    }
}
