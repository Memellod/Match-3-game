using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;


public class GameBoard : MonoBehaviour
{
    [Range(0,15)]
    [SerializeField] public int columns = 10, rows = 4; // number of cols and rows in board
    [SerializeField] GameObject tileBase = null; // base of tile
    Tile[,] board;          
    public Tile selectedTile;                   
    [SerializeField] public int  scaleColumns = 2, scaleRows = 2;  // scale for calculating position
    [SerializeField] GameObject panel;  // UI element (parent to gameBoard)
    [SerializeField] private MainVars.gameStates gameState;

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
        gameState = MainVars.gameStates.falling;
        StartCoroutine("Initialize");
    }

    IEnumerator Initialize()
    {
        selectedTile = null;
        board = new Tile[rows, columns];
        GenerateBoard();
        yield return StartCoroutine("PlaceCells");
        gameState = MainVars.gameStates.calm;
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
                    board[i, j].PlaceInCell(position, i, j);
                }
            }
        }
        // TODO: change this to be sure that NO ONE TILE IS FALLING
        yield return new WaitForSeconds(2f);
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
                    Tile newTile = newTileGO.GetComponent<Tile>();
                    newTileGO.transform.parent = gameObject.transform;

                    newTile.RandomizeTile();
                    newTile.row = i;
                    newTile.column = j;
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

    /// <summary>
    /// returns list of tiles that makes adjacent to match
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    List<Tile> FindMatch(int row, int column)
    {
        List<Tile> answer = new List<Tile>();
        // find 3-match to any direction 
        if (Find3StraightMatch(row, column))
        {
            // if found, find every adjacent block and return list
            Queue<Tile> queue = new Queue<Tile>();

            queue.Enqueue(board[row, column]);
            answer.Add(board[row, column]);
            while (queue.Count > 0)
            {
                // getting object from queue
                Tile tile = queue.Dequeue();
                // getting adjacent Tiles of it
                List<Tile> adjacent = GetAdjacentTiles(tile.row, tile.column);
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
        Tile tile = board[row, column];
        

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
    private List<Tile> GetAdjacentTiles(int row, int column)
    {
        List<Tile> answer = new List<Tile>();

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
    public IEnumerator TrySwapTiles(Tile tileToSwap)
    {
        // TODO: make explosion for tiles as coroutine
        //

        // cant make moves 
        if (gameState != MainVars.gameStates.calm) yield break;

        // if tile is too far - do nothing
        if (!GetAdjacentTiles(tileToSwap.row, tileToSwap.column).Contains(selectedTile)) yield break;

        // else swap it
        board[selectedTile.row, selectedTile.column] = tileToSwap;
        board[tileToSwap.row, tileToSwap.column] = selectedTile;

        // change gameState so player cant make moves
        gameState = MainVars.gameStates.falling;

        selectedTile.OnDeselected();

        yield return SwapTiles(selectedTile, tileToSwap);

        List<Tile> matchedTiles = FindMatch(selectedTile.row, selectedTile.column);
        if (matchedTiles.Count > 2)
        {
            ExplodeTiles(matchedTiles);
        }

        if (tileToSwap != null)
        {
            matchedTiles = FindMatch(tileToSwap.row, tileToSwap.column);
            if (matchedTiles.Count > 2)
            {
                ExplodeTiles(matchedTiles);
            }
        }

        do
        {
            gameState = MainVars.gameStates.falling;
            DescendCells();
            GenerateBoard();
            yield return StartCoroutine("PlaceCells");
        }
        while (CheckForMatchForEveryTile());


        // after this no  more matches possible


        selectedTile = null;
        gameState = MainVars.gameStates.calm;
    }

    IEnumerator SwapTiles(Tile selectedTile, Tile tileToSwap)
    {
        int old_row = selectedTile.row;
        int old_column = selectedTile.column;

        selectedTile.row = tileToSwap.row;
        selectedTile.column = tileToSwap.column;

        tileToSwap.row = old_row;
        tileToSwap.column = old_column;
        // swap positions with animation
        selectedTile.StartCoroutine("MoveTo", tileToSwap.transform.localPosition);
        // wait for 2nd to end
        yield return tileToSwap.StartCoroutine("MoveTo", selectedTile.transform.localPosition);
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
                        List<Tile> matchedTiles = FindMatch(i, j);
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
    private void ExplodeTiles(List<Tile> matchedTile)
    {
        // get points for matching
        int points = (int)(matchedTile.Count * 40 + Mathf.Pow(2, matchedTile.Count) * 10);

        // send points to GameManager
        GameManager.Instance.AddPoints(points);
       
        
        foreach (Tile i in matchedTile)
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
