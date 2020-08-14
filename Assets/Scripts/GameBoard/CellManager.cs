using UnityEngine;
using System.Collections;
using Cell;
using System.Collections.Generic;
using static MainVars;
using ObjectPooling;
using GameBoards.MatchFinding;
using System;
using Cell.Visuals;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.NetworkInformation;

namespace GameBoards.CellManagement
{
    public class CellManager : MonoBehaviour
    {
        ObjectPool cellPool;
        int rows, columns;
        CellBase[,] board;
        MatchFinder matchFinder;
        [SerializeField] GameObject tileBase = null; // base of tile

        public event IsWorkEnded CheckForWork = () => { return true; };

        private void Initialize()
        {
            board = gameBoard.GetBoard();
            rows = board.GetLength(0);
            columns = board.GetLength(1);

            cellPool = new ObjectPool(columns * rows, tileBase);
            matchFinder = GetComponent<MatchFinder>();

        }
        private void InitCell(GameObject newCellGO)
        {
            newCellGO.transform.SetParent(gameObject.transform);
        }

        /// <summary>
        /// fills empty cells with random tiles
        /// </summary>
        internal void GenerateBoard()
        {
            if (board == null) Initialize();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (gameBoard.board[i, j] == null)
                    {
                        GameObject newTileGO = cellPool.GetObject();
                        CellBase newTile = newTileGO.GetComponent<CellBase>();

                        InitCell(newTileGO);

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
        }



        /// <summary>
        ///  delete matched tiles and gain points according to number of tiles exploded
        /// </summary>
        /// <param name="matchedTile"></param>
        internal IEnumerator ExplodeTiles(List<CellBase> matchedTile)
        {
            // get points for matching
            int points = (int)(matchedTile.Count * 40 + Mathf.Pow(2, matchedTile.Count) * 10);

            // send points to GameManager
            GameManager.Instance.AddPoints(points);


            foreach (CellBase i in matchedTile)
            {
                gameBoard.board[i.row, i.column] = null;
                var icv = i.GetComponent<CellVisuals>();
                icv.StartCoroutine(nameof(icv.PlayVFX));                
            }

            yield return WaitForVFX();

            foreach (CellBase i in matchedTile)
            {
                cellPool.AddObject(i.gameObject);
            }
        }

        private IEnumerator WaitForVFX()
        {
            bool flag;
            do
            {
                flag = true;

                foreach (var i in CheckForWork.GetInvocationList())
                {
                    if ((bool)i.DynamicInvoke() == false)
                    {
                        flag = false;
                        break;
                    }
                }
            } while (!flag);
            yield break;
        }
    }
}