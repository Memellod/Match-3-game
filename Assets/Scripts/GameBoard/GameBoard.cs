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

        float visualDuration = 0.5f; // duration of visual effects to wait before continue 



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
            cellManager.GenerateBoard();
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


            visualDuration = selectedTile.GetComponent<CellVisuals>().duration;

            // change gameState so player cant make moves
            gameState = gameStates.falling;

            selectedTile.OnDeselected();

            yield return cellPositionHandler.SwapTilesWithAnimation(selectedTile, tileToSwap);

            

            // else swap it


            List<CellBase> matchedTiles1 = matchFinder.FindMatch(selectedTile.row, selectedTile.column);
            List<CellBase> matchedTiles2 = matchFinder.FindMatch(tileToSwap.row, tileToSwap.column);


            // TODO: if no match after turn - swap back
            if (matchedTiles1.Count < 3 && matchedTiles2.Count < 3)
            {
                yield return cellPositionHandler.SwapTilesWithAnimation(selectedTile, tileToSwap);
                selectedTile = null;
                gameState = gameStates.calm;
                yield break;
            }


            if (matchedTiles1.Count > 2 && matchedTiles2.Count > 2)
            {
                cellManager.StartCoroutine(nameof(cellManager.ExplodeTiles), matchedTiles1);
                yield return cellManager.ExplodeTiles(matchedTiles2);
            }
            else
            if (matchedTiles1.Count > 2 )
            {
                yield return cellManager.ExplodeTiles(matchedTiles1);
            }
            else
            if (matchedTiles2.Count > 2)
            {

                yield return cellManager.ExplodeTiles(matchedTiles2);
            }


            do
            {
                gameState = gameStates.falling;
                yield return new WaitForSeconds(visualDuration);
                cellPositionHandler.DescendCells();
                cellManager.GenerateBoard();
                yield return cellPositionHandler.StartCoroutine(nameof(cellPositionHandler.PlaceCells));
            }
            while (CheckForMatchForEveryTile());


            // after this no  more matches possible


            selectedTile = null;
            gameState = gameStates.calm;
        }

        /// <summary>
        ///  checks for matches for every tile (after descending for ex.)
        /// </summary>
        /// <returns></returns>
        internal bool CheckForMatchForEveryTile()
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
                            List<CellBase> matchedTiles = matchFinder.FindMatch(i, j);
                            if (matchedTiles.Count > 2)
                            {
                                flag = true;
                                answer = true;
                                cellManager.StartCoroutine(nameof(cellManager.ExplodeTiles), matchedTiles); 
                            }
                        }
                    }
                }
            } while (flag);
            return answer;
        }



    }
}