using UnityEngine;
using System.Collections;
using Cell;
using Cell.Mover;
using static MainVars;

namespace GameBoards.CellPositionHandling
{
    public class CellPositionHandler : MonoBehaviour
    {
        CellBase[,] board;
        int columns, rows;

        void Initialize()
        {
            board = gameBoard.GetBoard();
            rows = board.GetLength(0);
            columns = board.GetLength(1);
        }
       internal IEnumerator SwapCellsWithAnimation(CellBase selectedTile, CellBase tileToSwap)
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
            CellMover cellMover;
            cellMover = selectedTile.GetComponent<CellMover>();
            cellMover.StartCoroutine(cellMover.MoveTo(cellMover.swapSpeed, tileToSwap.transform.localPosition, Vector3.zero));

            // wait for 2nd to end
            cellMover = tileToSwap.GetComponent<CellMover>();
            yield return cellMover.StartCoroutine(cellMover.MoveTo(cellMover.swapSpeed, selectedTile.transform.localPosition, Vector3.zero));
        }



        /// <summary>
        ///  descends tiles to empty positions
        /// </summary>
        internal void DescendCells()
        {
            for (int i = 0; i < rows; i++)                            // from 2nd row
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

            StartCoroutine(nameof(PlaceCells));
        }

        /// <summary>
        /// placing tiles to its cells
        /// </summary>
        internal IEnumerator PlaceCells()
        {
            if (board == null) Initialize();
            Vector3 position;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (board[i, j] != null)
                    {
                        position = gameBoard.Position(i, j);
                        CellMover cm = board[i, j].GetComponent<CellMover>();
                        cm.PlaceInCell(position, i, j);
                    }
                }
            }
            // TODO: change this to be sure that NO ONE TILE IS FALLING
            yield return new WaitForSeconds(1.5f);
        }
    }


}