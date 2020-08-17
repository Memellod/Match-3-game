using Cell;
using GameBoards.CellManagement;
using GameBoards.CellPositionHandling;
using GameBoards.MatchFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainVars;


namespace GameBoards
{
    [RequireComponent(typeof(CellPositionHandler), typeof(MatchFinder), typeof(CellManager))]
    public class GameBoard : MonoBehaviour
    {
        [Range(0, 15)]
        [SerializeField] public int columns = 10, rows = 4; // number of cols and rows in board
        [SerializeField] public int scaleColumns = 2, scaleRows = 2;  // scale for calculating position
        [SerializeField] public gameStates gameState { get; private set; } // state of a game

        internal CellBase[,] board;
        internal CellBase selectedTile;

        CellPositionHandler cellPositionHandler;
        MatchFinder matchFinder;
        CellManager cellManager;

        private bool foundMatches; /// changes value by <see cref="IsMoveAvailable"/>

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
                cellManager.ClearBoard();
                cellManager.FillEmptyCells();
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
            // TODO: break this function to more function

           
            if (gameState != gameStates.calm) yield break;                                                             // cant make moves 
            if (!matchFinder.GetAdjacentTiles(tileToSwap.row, tileToSwap.column).Contains(selectedTile)) yield break; // if tile is too far - do nothing

            gameState = gameStates.falling;                                                                           // change gameState so player cant make moves
            selectedTile.OnDeselected();                                                                          

            yield return cellPositionHandler.SwapCellsWithAnimation(selectedTile, tileToSwap);                       // swap cells with animation
            yield return HandlePossibleMatches(selectedTile, tileToSwap);                                            // handle possible matches if exist
            if (!foundMatches)                                                                                       // if not found any match, end turn
            {
                yield return cellPositionHandler.SwapCellsWithAnimation(selectedTile, tileToSwap);                   // swap cells back
                selectedTile = null;
                gameState = gameStates.calm;
                yield break;
            }

            yield return PassiveLoop(); // WAIT TIL NO MORE MATCHES ARE FOUND EVEN AFTER REGENERATING BOARD

            if (!IsMoveAvailable()) // if regenerating board is needed
            {
                // TODO: make some message to appear before regenerating board
                yield return new WaitForSeconds(2f);
                //TODO: yield return show message(float time);

                while (!IsMoveAvailable()) // regenerate board to make available move
                {
                    cellManager.ClearBoard();    //firstly clear board
                    cellManager.FillEmptyCells(); // then fill with objects
                }
                yield return cellPositionHandler.StartCoroutine(cellPositionHandler.PlaceCells());
            }

            // turn is over
            selectedTile = null;
            gameState = gameStates.calm;
        }

        IEnumerator PassiveLoop()
        {
            do                                          // if matches are found and exploded
            {
                gameState = gameStates.falling;         // change state so no moves can be made
                cellPositionHandler.DescendCells();     // descend objects on empty cells
                cellManager.FillEmptyCells();            // generate new objects on empty cells
                yield return cellPositionHandler.StartCoroutine(cellPositionHandler.PlaceCells()); // wait for them to be placed
                yield return CheckForMatchForEveryTile();                          // check for more matches on the turn
            }
            while (foundMatches);
        }

        /// <summary>
        /// finds any match after 2 cells are swapped
        /// </summary>
        /// <param name="firstCell"></param>
        /// <param name="secondCell"></param>
        /// <returns></returns>
        IEnumerator HandlePossibleMatches(CellBase firstCell, CellBase secondCell)
        {
            // find matches if exist
            List<CellBase> matchedCells1 = matchFinder.FindMatch(firstCell.row, firstCell.column);
            List<CellBase> matchedCells2 = matchFinder.FindMatch(secondCell.row, secondCell.column);


            // if no match after turn - swap back
            if (matchedCells1.Count < 3 && matchedCells2.Count < 3)
            {
                foundMatches = false;
                yield break;
            }


            // if both have objects matches
            if (matchedCells1.Count > 2 && matchedCells2.Count > 2)
            {
                cellManager.StartCoroutine(cellManager.ExplodeTiles(matchedCells1));
                yield return cellManager.ExplodeTiles(matchedCells2);
            }
            else // only if first
            if (matchedCells1.Count > 2)
            {
                yield return cellManager.ExplodeTiles(matchedCells1);
            }
            else // only if second
            if (matchedCells2.Count > 2)
            {
                yield return cellManager.ExplodeTiles(matchedCells2);
            }

            foundMatches = true;

        }

        internal bool IsMoveAvailable()
        {
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
                    
                    // find at least one adjacent of same type
                    List<CellBase> adjacentCells = matchFinder.GetAdjacentTiles(board[i, j].row, board[i, j].column);
                    // leave only of same type
                    List<CellBase> adjacentSameCells = adjacentCells.FindAll(x => x.IsSame(board[i, j]) && !x.isViewed);
                    
                    //find that crossy thing
                    if (adjacentCells.FindAll(x => x.IsSame(adjacentCells[0])).Count > 2)
                    {
                        //adjacentCells.ForEach(x => x.GetComponent<CellVisuals>().image.color = Color.green);
                        return true;
                    }

                    adjacentSameCells.Add(board[i, j]);
                    // if no adjacent of same type found - continue
                    if (adjacentSameCells.Count < 2) continue;

                    // find a move that make a match
                    if (FindAvailableMove(adjacentSameCells))
                    {
                        //adjacentSameCells.ForEach(x => x.GetComponent<CellVisuals>().image.color = Color.green);
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
            foreach (CellBase cb in board)
            {
                cb.isViewed = false;
            }
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
                        if (board[i, j] == null || board[i, j].isViewed) continue;
                        

                        List<CellBase> matchedTiles = matchFinder.FindMatch(i, j);


                        if (matchedTiles.Count <= 2) continue;
                        
                        listOfMatches.Add(matchedTiles);
                        matchedTiles.ForEach(x => x.isViewed = true);
                        flag = true;
                        answer = true;
                    }
                }

                // explode every match found
                if (flag)
                {
                    //start them async
                    for (int i = 0; i < listOfMatches.Count - 1; i++)
                    {
                        cellManager.StartCoroutine(cellManager.ExplodeTiles(listOfMatches[i]));
                    }
                    //wait til last is ended
                    yield return cellManager.StartCoroutine(cellManager.ExplodeTiles(listOfMatches[listOfMatches.Count - 1]));
                }

            } while (flag);
            foundMatches = answer;
        }



    }
}