using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour {

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offset;//sliding down objects
    public GameObject tilePrefabs;
    public GameObject[] dots;
    public GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
	// Use this for initialization
	void Start () {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
	}
	
	private void SetUp(){
        for(int i = 0; i<width; i++)
        {
            for(int j = 0; j<height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offset);
                GameObject backgroundTile = Instantiate(tilePrefabs, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + "," + j + " )";
                int dotToUse = Random.Range(0, dots.Length);
                int maxTimes = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxTimes <100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxTimes++;
                }
                maxTimes = 0;
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                dot.transform.parent = this.transform;
                dot.name = "( " + i + "," + j + " )";
                allDots[i, j] = dot;
            }
        }
    }
    //check whether there are matches at the begining
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots [column -2,row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row -1].tag == piece.tag && allDots[column , row - 2].tag == piece.tag)
            {
                return true;
            }
        }else if(column < +1 || row <= 1){
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if(column > 1)
            { 
                if (allDots[column -1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //how many elements are in the matched pieces list from findmatch?
            if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }
            
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f); 
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for(int i =0; i<width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        int nullCounter = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCounter++;
                } else if(nullCounter > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCounter;
                    allDots[i, j] = null;
                }
            }
            nullCounter = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    //refill the board aftere collapse
    private void RefilBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;

                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefilBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;

        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;

    }
}
