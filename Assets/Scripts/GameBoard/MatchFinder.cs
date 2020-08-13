using UnityEngine;
using System.Collections;
using Cell;
using System.Collections.Generic;
using static MainVars;

namespace GameBoards.MatchFinding
{
    [RequireComponent(typeof(GameBoard))]
    public class MatchFinder : MonoBehaviour
    {
        CellBase[,] board;
        int columns, rows;


        void Initialize()
        {
            board =  gameBoard.GetBoard();
            rows = board.GetLength(0);
            columns = board.GetLength(1);
        }

        /// <summary>
        /// returns list of tiles that makes match
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal List<CellBase> FindMatch(int row, int column)
        {
            List<CellBase> answer = new List<CellBase>();
            // find 3-match to any direction 
            if (Find3StraightMatch(row, column))
            {
                // if found, find every adjacent block and return list
                Queue<CellBase> queue = new Queue<CellBase>();

                queue.Enqueue( board[row, column]);
                answer.Add( board[row, column]);
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
        internal bool Find3StraightMatch(int row, int column)
        {
            if (board == null) Initialize();
            bool answer = false;
            CellBase tile =  board[row, column];


            // if 2 up
            if (row < rows - 2
                &&  board[row + 1, column] != null &&  board[row + 2, column] != null)
            {
                answer =  board[row + 1, column].IsSame(tile) &&  board[row + 2, column].IsSame(tile);
                if (answer)
                {
                    return true;
                }
            }

            // centered up and down
            if (row < rows - 1 && row > 0
                &&  board[row + 1, column] != null &&  board[row - 1, column] != null)
            {
                answer =  board[row + 1, column].IsSame(tile) &&  board[row - 1, column].IsSame(tile);
                if (answer)
                {
                    return true;
                }
            }

            // 2 right
            if (column < columns - 2
                &&  board[row, column + 1] != null &&  board[row, column + 2] != null)
            {
                answer =  board[row, column + 1].IsSame(tile) &&  board[row, column + 2].IsSame(tile);
                if (answer)
                {
                    return true;
                }
            }

            // centered left and right
            if (column < columns - 1 && column > 0
                &&  board[row, column + 1] != null &&  board[row, column - 1] != null)
            {
                answer =  board[row, column + 1].IsSame(tile) &&  board[row, column - 1].IsSame(tile);
                if (answer)
                {
                    return true;
                }
            }

            // 2 lower
            if (row > 1
                &&  board[row - 1, column] != null &&  board[row - 2, column] != null)
            {
                answer =  board[row - 1, column].IsSame(tile) &&  board[row - 2, column].IsSame(tile);
                if (answer)
                {
                    return true;
                }
            }

            // 2 left
            if (column > 1
                &&  board[row, column - 1] != null &&  board[row, column - 2] != null)
            {
                answer =  board[row, column - 1].IsSame(tile) &&  board[row, column - 2].IsSame(tile);
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
        internal List<CellBase> GetAdjacentTiles(int row, int column)
        {
            if (board == null) Initialize();
            List<CellBase> answer = new List<CellBase>();

            // add down
            if (row != 0
                &&  board[row - 1, column] != null) answer.Add( board[row - 1, column]);

            // add up
            if (row != rows - 1
                &&  board[row + 1, column] != null) answer.Add( board[row + 1, column]);

            // add left
            if (column != 0
                &&  board[row, column - 1] != null) answer.Add( board[row, column - 1]);

            // add right
            if (column != columns - 1
                &&  board[row, column + 1] != null) answer.Add( board[row, column + 1]);


            return answer;
        }
    }
}