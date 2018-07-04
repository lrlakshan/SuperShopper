using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour {

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offset;//sliding down objects
    public GameObject tilePrefabs;
    public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    public int BasePieceValue = 1;
    public int BaseValue = 1;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refilDelay = 0.5f;
    public int[] scoreGoals;
	// Use this for initialization
	public void Start () {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
        currentState = GameState.pause;
	}

    public void GenerateBlankSpaces()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        //look at all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if a tile is a jelly tile
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                //craete a jelly tile at the position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    }
	
	public void SetUp(){
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for(int i = 0; i<width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    Vector2 tilePosition = new Vector2(i,j);
                    GameObject backgroundTile = Instantiate(tilePrefabs, tilePosition, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + "," + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxTimes = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxTimes < 100)
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
    }
    //check whether there are matches at the begining
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }else if(column < +1 || row <= 1){
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firsstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firsstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if(dot.row == firsstPiece.row)
                {
                    numberHorizontal++;
                }
                if (dot.column == firsstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return(numberVertical == 5 || numberHorizontal == 5);
}

    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7){
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //Make a color Bomb
                //Debug.Log("Make a color bomb");
                //is the current dot matched
                if(currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }else
                    {
                        if(currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                //Make an adjecent bomb
                //Debug.Log("Make a Adjecent bomb");
                //is the current dot matched
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjecentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjecentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjecentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjecentBomb();
                                }
                            }
                        }
                    }
                }

            }
        }
        
     }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //how many elements are in the matched pieces list from findmatch?
            if(findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            //Does a tile need to break
            if(breakableTiles[column, row] != null)
            {
                //if it does, get one damage
                breakableTiles[column, row].TakeDamage(1);
                if(breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

            if(goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            //does the sound manager exist
            if(soundManager != null)
            {
                soundManager.PlayRandomDestroyNoice();
            }
            
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f); 
            Destroy(allDots[column, row]);  
            GameObject obj1 = allDots[column, row]; 
            if (obj1.gameObject.CompareTag("Axe"))
            {
                scoreManager.IncreaseScore1(BaseValue*streakValue);
            }
            else if (obj1.gameObject.CompareTag("Cargills ice cream"))
            {
                scoreManager.IncreaseScore2(BaseValue*streakValue);
            }
            else if (obj1.gameObject.CompareTag("Cloguard"))
            {
                scoreManager.IncreaseScore3(BaseValue * streakValue);
            }
            else if (obj1.gameObject.CompareTag("Closeup"))
            {
                scoreManager.IncreaseScore4(BaseValue * streakValue);
            }
            else if (obj1.gameObject.CompareTag("EH ice cream"))
            {
                scoreManager.IncreaseScore5(BaseValue * streakValue);
            }
            else if (obj1.gameObject.CompareTag("Signal"))
            {
                scoreManager.IncreaseScore6(BaseValue * streakValue);
            }

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
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if the current spot is not blank and is empty
                if(!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    //loop from the space above to the top of the column
                    for(int k = j+1; k < height; k++)
                    {
                        //if a dot is found 
                        if(allDots[i,k] != null)
                        {
                            //move that dot tot the empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //set that spot to null
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refilDelay*0.5f);
        StartCoroutine(FillBoardCo());
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
        yield return new WaitForSeconds(refilDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    //refill the board aftere collapse
    private void RefilBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxTimes = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxTimes < 100)
                    {
                        maxTimes++;
                        dotToUse = Random.Range(0, dots.Length);
                        
                    }
                    
                    maxTimes = 0;
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
        yield return new WaitForSeconds(refilDelay);

        while (MatchesOnBoard())
        {
            //streakValue ++;
            DestroyMatches();
            yield return new WaitForSeconds(2*refilDelay);
            
        }
        findMatches.currentMatches.Clear();
        currentDot = null;

        yield return new WaitForSeconds(refilDelay);

        if (IsDeadlocked())
        {
            ShuffleBoard();
            Debug.Log("Deadlocked!!!");
        }

        currentState = GameState.move;
        //streakValue = 1;

    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //take the first piece and save it in a folder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //switching the first dot to be the second position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        // set the first dot to be second dot
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //Make sure that one and two to the right are in the board
                    if (i < width - 2)
                    {
                        //check if the dots to the right and two to the right exist
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2)
                    {

                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }

                        }
                    }
                }
            }
        }

                    return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }
    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {

                    if (i < width - 2)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 2)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }

                        return true;
    }

    public void ShuffleBoard()
    {
        //create a list of gameobjects
        List<GameObject> newBoard = new List<GameObject>();
        //add every piece to the list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        //for every spot on the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot should not be blank
                if (!blankSpaces[i, j])
                {
                    int PieceToUse = Random.Range(0, newBoard.Count);
                    //make container for the piece
                    int maxTimes = 0;
                    while (MatchesAt(i, j, newBoard[PieceToUse]) && maxTimes < 100)
                    {
                        PieceToUse = Random.Range(0, newBoard.Count);
                        maxTimes++;
                    }
                    Dot piece = newBoard[PieceToUse].GetComponent<Dot>();
                    maxTimes = 0;
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[PieceToUse];
                    newBoard.Remove(newBoard[PieceToUse]);
                }
            }
        }
        //check if it is still deadlocked
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}
  