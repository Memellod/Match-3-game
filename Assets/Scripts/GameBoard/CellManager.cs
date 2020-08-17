using Cell;
using Cell.Visuals;
using GameBoards.MatchFinding;
using ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainVars;

namespace GameBoards.CellManagement
{
    public class CellManager : MonoBehaviour
    {
        ObjectPool cellPool;
        int rows, columns;
        CellBase[,] board;
        MatchFinder matchFinder;
        [SerializeField] GameObject tileBase = null; // base of tile


        public List<CellVisuals> playingAnimationsList;

        private void Initialize()
        {
            board = gameBoard.GetBoard();
            rows = board.GetLength(0);
            columns = board.GetLength(1);

            cellPool = new ObjectPool(columns * rows + 1, tileBase);
            matchFinder = GetComponent<MatchFinder>();
            playingAnimationsList = new List<CellVisuals>();

        }

        internal void ClearBoard()
        {
            if (board == null) Initialize();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (board[i, j] == null) continue;

                    cellPool.AddObject(board[i, j].gameObject);
                    board[i, j] = null;
                }
            }
        }

        /// <summary>
        /// fills empty cells with random tiles
        /// </summary>
        internal void FillEmptyCells()
        {
            if (board == null) Initialize();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (gameBoard.board[i, j] != null) continue;

                    GameObject newTileGO = cellPool.GetObject();
                    CellBase newTile = newTileGO.GetComponent<CellBase>();

                    newTile.transform.SetParent(gameObject.transform);
                    newTile.RandomizeTile();
                    newTile.SetCell(i, j);
                    gameBoard.board[i, j] = newTile;

                    // while there is match with that type of tile - randomize it
                    while (matchFinder.Find3StraightMatch(newTile.row, newTile.column))
                    {
                        newTile.RandomizeTile();
                    }

                    // after defining type of tile - put in board

                    gameBoard.board[i, j] = newTile;
                }

            }
        }



        /// <summary>
        ///  delete matched tiles and gain points according to number of tiles exploded
        /// </summary>
        /// <param name="matchedTile"></param>
        internal IEnumerator ExplodeTiles(List<CellBase> matchedTile)
        {
            foreach (CellBase i in matchedTile)
            {
                gameBoard.board[i.row, i.column] = null;
                var icv = i.GetComponent<CellVisuals>();
                icv.StartCoroutine(nameof(icv.PlayVFX));                
            }

            yield return WaitForVFX();

            HandlePoints(matchedTile);

            foreach (CellBase i in matchedTile)
            {
                cellPool.AddObject(i.gameObject);
            }

        }

        private static void HandlePoints(List<CellBase> matchedTile)
        {
            // send points to GameManager
            GameManager.Instance.CalculatePointsOfMatch(matchedTile);
        }

        private IEnumerator WaitForVFX()
        {
            bool flag;
            do
            {
                yield return new WaitForSeconds(0.1f);
                flag = playingAnimationsList.Count == 0;
            } while (!flag);
        }
    }
}