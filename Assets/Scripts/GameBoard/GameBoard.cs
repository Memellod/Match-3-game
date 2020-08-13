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
            // TODO: F if no match after turn - do not do it
            //matchFinder.IsAbleToMove()

            // else swap it

            // change gameState so player cant make moves
            gameState = gameStates.falling;

            selectedTile.OnDeselected();

            yield return cellPositionHandler.SwapTilesWithAnimation(selectedTile, tileToSwap);

            List<CellBase> matchedTiles = matchFinder.FindMatch(selectedTile.row, selectedTile.column);
            if (matchedTiles.Count > 2)
            {
                cellManager.ExplodeTiles(matchedTiles);
            }

            matchedTiles = matchFinder.FindMatch(tileToSwap.row, tileToSwap.column);
            if (matchedTiles.Count > 2)
            {
                cellManager.ExplodeTiles(matchedTiles);
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
                                cellManager.ExplodeTiles(matchedTiles);
                            }
                        }
                    }
                }
            } while (flag);
            return answer;
        }



    }
}