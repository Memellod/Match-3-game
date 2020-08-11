using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cell;
using Cell.Mover;
using static MainVars;
using System;

public class GameBoard : MonoBehaviour
{
    [Range(0,15)]
    [SerializeField] public int columns = 10, rows = 4; // number of cols and rows in board
    [SerializeField] GameObject tileBase = null; // base of tile
    CellBase[,] board;          
    public CellBase selectedTile;                   
    [SerializeField] public int  scaleColumns = 2, scaleRows = 2;  // scale for calculating position
    [SerializeField] private gameStates gameState;

    /// <summary>
    /// calculates position for given row and column in board matrix
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private Vector3 Position(int row, int column)
    {
        return new Vector3(column * scaleRows, row * scaleColumns, 0);
    }

    private void Awake()
    {
        gameState = gameStates.falling;
        StartCoroutine("Initialize");
        
    }

    IEnumerator Initialize()
    {
        selectedTile = null;
        board = new CellBase[rows, columns];
        GenerateBoard();
        yield return StartCoroutine("PlaceCells");
        gameState = gameStates.calm;
    }

    /// <summary>
    /// placing tiles to its cells
    /// </summary>
    IEnumerator PlaceCells()
    {
        Vector3 position;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (board[i, j] != null)
                {
                    position = Position(i, j);
                    board[i, j].GetComponent<CellMover>().PlaceInCell(position, i, j);
                }
            }
        }
        // TODO: change this to be sure that NO ONE TILE IS FALLING
        yield return new WaitForSeconds(1.5f);
    }

    /// <summary>
    /// fills empty cells with random tiles
    /// </summary>
    void GenerateBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (board[i, j] == null)
                {
                    GameObject newTileGO = Instantiate(tileBase);
                    CellBase newTile = newTileGO.GetComponent<CellBase>();

                    InitCell(newTileGO);

                    newTile.RandomizeTile();
                    newTile.SetCell(i, j);
                    board[i, j] = newTile;

                    // while there is match with that type of tile - randomize it
                    while (Find3StraightMatch(newTile.row, newTile.column))
                    {
                        newTile.RandomizeTile();
                    }

                    // after defining type of tile - put in board

                    board[i, j] = newTile;
                }
            }
        }
    }

    private void InitCell(GameObject newCellGO)
    {
        newCellGO.transform.parent = gameObject.transform;
    }

    /// <summary>
    /// returns list of tiles that makes match
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    List<CellBase> FindMatch(int row, int column)
    {
        List<CellBase> answer = new List<CellBase>();
        // find 3-match to any direction 
        if (Find3StraightMatch(row, column))
        {
            // if found, find every adjacent block and return list
            Queue<CellBase> queue = new Queue<CellBase>();

            queue.Enqueue(board[row, column]);
            answer.Add(board[row, column]);
            while (queue.Count > 0)
            {
                // getting object from queue
                CellBase tile = queue.Dequeue();
                // getting adjacent Tiles of it
                List<CellBase> adjacent = GetAdjacentTiles(tile.row, tile.column);
                // removing intersection (removing checked tiles) and @bad@ tiles
                adjacent.RemoveAll(x => !x.IsSame(answer[0]) || answer.Contains(x));
                // adding to set of result
                answer.AddRange(adjacent);
                // add to queue all unchecked tiles
                adjacent.ForEach(x => queue.Enqueue(x));
            }
        }
        return answer;
    }

    /// <summary>
    /// returns bool indication that theres is 3 match for given row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private bool Find3StraightMatch(int row, int column)
    {
        bool answer = false;
        CellBase tile = board[row, column];
        

        // if 2 up
        if (row < rows - 2 
            && board[row + 1, column] != null && board[row + 2, column] != null)
        {
            answer = board[row + 1, column].IsSame(tile) && board[row + 2, column].IsSame(tile);
            if (answer)
            {
                return true;
            }
        }

        // centered up and down
        if (row < rows - 1 && row > 0 
            && board[row + 1, column] != null && board[row - 1, column] != null)
        {
            answer = board[row + 1, column].IsSame(tile) && board[row - 1, column].IsSame(tile);
            if (answer)
            {
                return true;
            }
        }

        // 2 right
        if (column < columns - 2 
            && board[row, column + 1] != null && board[row, column + 2] != null)
        {
            answer = board[row, column + 1].IsSame(tile) && board[row, column + 2].IsSame(tile);
            if (answer)
            {
                return true;
            }
        }

        // centered left and right
        if (column < columns - 1 && column > 0
            && board[row, column + 1] != null && board[row, column - 1] != null)
        {
            answer = board[row, column + 1].IsSame(tile) && board[row, column - 1].IsSame(tile);
            if (answer) 
            {
                return true;
            }
        }

        // 2 lower
        if (row > 1 
            && board[row - 1, column] != null && board[row - 2, column] != null)
        {
            answer = board[row - 1, column].IsSame(tile) && board[row - 2, column].IsSame(tile);
            if (answer)
            {
                return true;
            }
        }

        // 2 left
        if (column > 1
            && board[row, column - 1] != null && board[row, column - 2] != null)
        {
            answer = board[row, column - 1].IsSame(tile) && board[row, column - 2].IsSame(tile);
            if (answer)
            {
                return true;
            }
        }

        // no matches found
        return false;
    }


    /// <summary>
    /// returns list of adjacent tiles
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private List<CellBase> GetAdjacentTiles(int row, int column)
    {
        List<CellBase> answer = new List<CellBase>();

        // add down
        if (row != 0 
            && board[row - 1, column] != null) answer.Add(board[row - 1, column]);

        // add up
        if (row != rows - 1
            && board[row + 1, column] != null) answer.Add(board[row + 1, column]);

        // add left
        if (column != 0
            && board[row, column - 1] != null) answer.Add(board[row, column - 1]);

        // add right
        if (column != columns - 1
            && board[row, column + 1] != null) answer.Add(board[row, column + 1]);


        return answer;
    }


    /// <summary>
    ///  try to swap tiles
    /// </summary>
    /// <param name="tileToSwap"></param>
    public IEnumerator TrySwapTiles(CellBase tileToSwap)
    {
        // TODO: make explosion for tiles as coroutine
        //

        // cant make moves 
        if (gameState != gameStates.calm) yield break;

        // if tile is too far - do nothing
        if (!GetAdjacentTiles(tileToSwap.row, tileToSwap.column).Contains(selectedTile)) yield break;

        // else swap it

        // change gameState so player cant make moves
        gameState = gameStates.falling;

        selectedTile.OnDeselected();

        yield return SwapTilesWithAnimation(selectedTile, tileToSwap);

        List<CellBase> matchedTiles = FindMatch(selectedTile.row, selectedTile.column);
        if (matchedTiles.Count > 2)
        {
            ExplodeTiles(matchedTiles);
        }

        matchedTiles = FindMatch(tileToSwap.row, tileToSwap.column);
        if (matchedTiles.Count > 2)
        {
            ExplodeTiles(matchedTiles);
        }

        do
        {
            gameState = gameStates.falling;
            DescendCells();
            GenerateBoard();
            yield return StartCoroutine("PlaceCells");
        }
        while (CheckForMatchForEveryTile());


        // after this no  more matches possible


        selectedTile = null;
        gameState = gameStates.calm;
    }

    IEnumerator SwapTilesWithAnimation(CellBase selectedTile, CellBase tileToSwap)
    {

        board[selectedTile.row, selectedTile.column] = tileToSwap;
        board[tileToSwap.row, tileToSwap.column] = selectedTile;

        int old_row = selectedTile.row;
        int old_column = selectedTile.column;

        selectedTile.row = tileToSwap.row;
        selectedTile.column = tileToSwap.column;

        tileToSwap.row = old_row;
        tileToSwap.column = old_column;


        // swap positions with animation
        selectedTile.GetComponent<CellMover>().StartCoroutine("MoveTo", tileToSwap.transform.localPosition);
        // wait for 2nd to end
        yield return tileToSwap.GetComponent<CellMover>().StartCoroutine("MoveTo", selectedTile.transform.localPosition);
    }


    /// <summary>
    ///  checks for matches for every tile (after descending for ex.)
    /// </summary>
    /// <returns></returns>
    private bool CheckForMatchForEveryTile()
    {
        bool answer = false;
        bool flag;
        do
        {
            flag = false;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (board[i, j] != null)
                    {
                        List<CellBase> matchedTiles = FindMatch(i, j);
                        if (matchedTiles.Count > 2)
                        {
                            flag = true;
                            answer = true;
                            ExplodeTiles(matchedTiles);
                        }
                    }
                }
            }
        } while (flag);
        return answer;
    }



    /// <summary>
    ///  delete matched tiles and gain points according to number of tiles exploded
    /// </summary>
    /// <param name="matchedTile"></param>
    private void ExplodeTiles(List<CellBase> matchedTile)
    {
        // get points for matching
        int points = (int)(matchedTile.Count * 40 + Mathf.Pow(2, matchedTile.Count) * 10);

        // send points to GameManager
        GameManager.Instance.AddPoints(points);
       
        
        foreach (CellBase i in matchedTile)
        {
            board[i.row, i.column] = null;
            Destroy(i.gameObject);
        }
    }



    /// <summary>
    ///  descends tiles to empty positions
    /// </summary>
    private void DescendCells()
    {
        for (int i = 0; i < rows;  i++)                            // from 2nd row
        {   
            for (int j = 0; j < columns; j++)                    // through all columns
            {
                if (board[i, j] == null)                        // if lower is null then descend whole column
                {
                    int p = i;                    
                    while (p < rows && board[p, j] == null)     // find first not-null in column and descend 
                    {
                        p++;
                    }
                    
                    if (p != rows)                              // if found any - descend column to down
                    {
                        for (int k = 0; k < rows - p; k++)      // rows - p = number of descending tiles
                        {
                            board[i + k, j] = board[p + k, j];
                            board[p + k, j] = null;
                        }
                    }
                }
            }
        }

        PlaceCells();

    }


}
