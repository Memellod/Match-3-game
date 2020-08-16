using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cell;
using Cell.Visuals;
using static MainVars;
using GameBoards.CellPositionHandling;
using GameBoards.MatchFinding;
using GameBoards.CellManagement;

namespace GameBoards
{
    [RequireComponent(typeof(CellPositionHandler), typeof(MatchFinder), typeof(CellManager))]
    public class GameBoard : MonoBehaviour
    {
        [Range(0, 15)]
        [SerializeField] public int columns = 10, rows = 4; // number of cols and rows in board
        [SerializeField] public int scaleColumns = 2, scaleRows = 2;  // scale for calculating position
        [SerializeField] private gameStates gameState;

        internal CellBase[,] board;
        public CellBase selectedTile;

        CellPositionHandler cellPositionHandler;
        MatchFinder matchFinder;
        CellManager cellManager;

        private bool foundMoreMatches;

        private void Awake()
        {
            gameState = gameStates.falling;

            cellPositionHandler = GetComponent<CellPositionHandler>();
            matchFinder = GetComponent<MatchFinder>();
            cellManager = GetComponent<CellManager>();


            selectedTile = null;
            board = new CellBase[rows, columns];
            StartCoroutine(nameof(Initialize));
        }
        
        internal ref CellBase[,] GetBoard()
        {
            return ref board;
        }

        /// <summary>
        /// calculates position for given row and column in board matrix
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal Vector3 Position(int row, int column)
        {
            return new Vector3(column * scaleRows, row * scaleColumns, 0);
        }
        IEnumerator Initialize()
        {
            do
            {
                cellManager.DeleteBoard();
                cellManager.GenerateBoard();
            } while (!IsMoveAvailable());
            yield return cellPositionHandler.StartCoroutine(nameof(cellPositionHandler.PlaceCells));
            gameState = gameStates.calm;
        }

        /// <summary>
        ///  try to swap tiles
        /// </summary>
        /// <param name="tileToSwap"></param>
        public IEnumerator TrySwapTiles(CellBase tileToSwap)
        {

            // cant make moves 
            if (gameState != gameStates.calm) yield break;

            // if tile is too far - do nothing
            if (!matchFinder.GetAdjacentTiles(tileToSwap.row, tileToSwap.column).Contains(selectedTile)) yield break;


            // change gameState so player cant make moves
            gameState = gameStates.falling;

            selectedTile.OnDeselected();

            yield return cellPositionHandler.SwapTilesWithAnimation(selectedTile, tileToSwap);

            
            // find matches if exist
            List<CellBase> matchedTiles1 = matchFinder.FindMatch(selectedTile.row, selectedTile.column);
            List<CellBase> matchedTiles2 = matchFinder.FindMatch(tileToSwap.row, tileToSwap.column);


            // if no match after turn - swap back
            if (matchedTiles1.Count < 3 && matchedTiles2.Count < 3)
            {
                yield return cellPositionHandler.SwapTilesWithAnimation(selectedTile, tileToSwap);
                selectedTile = null;
                gameState = gameStates.calm;
                yield break;
            }


            // if both have objects matches
            if (matchedTiles1.Count > 2 && matchedTiles2.Count > 2)
            {
                cellManager.StartCoroutine(nameof(cellManager.ExplodeTiles), matchedTiles1);
                yield return cellManager.ExplodeTiles(matchedTiles2);
            }
            else // only if first
            if (matchedTiles1.Count > 2 )
            {
                yield return cellManager.ExplodeTiles(matchedTiles1);
            }
            else // only if second
            if (matchedTiles2.Count > 2)
            {
                yield return cellManager.ExplodeTiles(matchedTiles2);
            }


            do
            {
                gameState = gameStates.falling;         // change state so no moves can be made
                cellPositionHandler.DescendCells();     // descend objects on empty cells
                cellManager.GenerateBoard();            // generate new objects on empty cells
                yield return StartCoroutine(cellPositionHandler.PlaceCells()); // wait for them to be placed
                yield return StartCoroutine(CheckForMatchForEveryTile());    // check for more matches on the turn
            }
            while (foundMoreMatches);

            if (!IsMoveAvailable())
            {
                // TODO: make some message to appear before regenerating board
                yield return new WaitForSeconds(2f);
                //show message
                while (!IsMoveAvailable())
                {
                    cellManager.DeleteBoard();
                    cellManager.GenerateBoard();
                }
            }

            // turn is over
            selectedTile = null;
            gameState = gameStates.calm;
            yield return null;
        }

        internal bool IsMoveAvailable()
        {
            // TODO: add case when its like cross with no core
            foreach (CellBase cell in board)
            {
                cell.isViewed = false;
            }
            // find 2 in row or column cells of one type
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (board[i, j].isViewed) continue;

                    board[i, j].isViewed = true;
                    // find one adjacent of same type
                    List<CellBase> adjacent = matchFinder.GetAdjacentTiles(board[i, j].row, board[i, j].column);
                    List<CellBase> adjacentCells = adjacent.FindAll(x => x.IsSame(board[i, j]) && !x.isViewed);
                    adjacentCells.Add(board[i, j]);
                    // if no adjacent of same type found - continue
                    if (adjacentCells.Count < 2) continue;

                    // find a move that make a match
                    if (FindAvailableMove(adjacentCells))
                    {
                        //adjacentCells.ForEach(x => x.GetComponent<CellVisuals>().image.color = Color.green);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool FindAvailableMove(List<CellBase> setCellBases)
        {
            // setCellBases commonly is 2 cells
            CellBase firstCell = setCellBases[0];
            CellBase secondCell = setCellBases.Find(x => x.row == firstCell.row && x != firstCell);

            if (secondCell == null) secondCell = setCellBases.Find(x => x.column == firstCell.column && x != firstCell);

            // if its a row
            if (firstCell.row == secondCell.row)
            {
                int columnToCheck = firstCell.column - secondCell.column;
                List<CellBase> adjacentList = matchFinder.GetAdjacentTiles(firstCell.row, firstCell.column + columnToCheck);
                CellBase moveableForMatch = adjacentList.Find(x => x.IsSame(firstCell) && x != firstCell);

                if (moveableForMatch != null)
                {
                    return true;
                }

                columnToCheck = secondCell.column - firstCell.column;
                adjacentList = matchFinder.GetAdjacentTiles(secondCell.row, secondCell.column + columnToCheck);
                moveableForMatch = adjacentList.Find(x => x.IsSame(secondCell) && x != secondCell);

                if (moveableForMatch != null)
                {
                    return true;
                }

            }
            // its a column
            else
            {
                int rowToCheck = firstCell.row - secondCell.row;
                List<CellBase> adjacentList = matchFinder.GetAdjacentTiles(firstCell.row + rowToCheck, firstCell.column);
                CellBase moveableForMatch = adjacentList.Find(x => x.IsSame(firstCell) && x != firstCell);

                if (moveableForMatch != null)
                {
                    return true;
                }

                rowToCheck = secondCell.row - firstCell.row;
                adjacentList = matchFinder.GetAdjacentTiles(secondCell.row + rowToCheck, firstCell.column);
                moveableForMatch = adjacentList.Find(x => x.IsSame(secondCell) && x != secondCell);

                if (moveableForMatch != null)
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        ///  checks for matches for every tile (after descending for ex.)
        /// </summary>
        /// <returns></returns>
        internal IEnumerator CheckForMatchForEveryTile()
        {
            List<List<CellBase>> listOfMatches = new List<List<CellBase>>();
            bool answer = false;
            bool flag;
            do
            {
                listOfMatches.Clear();
                flag = false;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (board[i, j] != null)
                        {
                            List<CellBase> matchedTiles = matchFinder.FindMatch(i, j);
                            if (matchedTiles.Count > 2)
                            {
                                listOfMatches.Add(matchedTiles);
                                flag = true;
                                answer = true;
                            }
                        }
                    }
                }

                // explode every match found
                if (flag)
                {
                    //start them async
                    for (int i = 0; i < listOfMatches.Count - 1; i++)
                    {
                        cellManager.StartCoroutine(nameof(cellManager.ExplodeTiles), listOfMatches[i]);
                    }
                    //wait til last is ended
                    yield return cellManager.ExplodeTiles(listOfMatches[listOfMatches.Count - 1]);
                }

            } while (flag);
            foundMoreMatches = answer;
        }



    }
}