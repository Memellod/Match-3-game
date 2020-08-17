using UnityEngine;
using System.Collections;
//using System.Numerics;
using UnityEngine.EventSystems;
using GameBoards;
using static MainVars;

namespace Cell.Selector
{
    /// <summary>
    /// Class for handling tile selection
    /// </summary>
    [RequireComponent(typeof(CellBase))]
    public class CellSelector : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler
    {
        CellBase thisTile;
        private float dragDistance;

        private void Start()
        {
            dragDistance = Mathf.Min(Screen.height, Screen.width) * 10 / 100; //dragDistance is 10% width of the screen
            thisTile = GetComponent<CellBase>();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (gameBoard.selectedTile == null)           // no selected tile
            {
                thisTile.OnSelected();
                gameBoard.selectedTile = thisTile;
            }
            else
            {
                if (gameBoard.selectedTile == thisTile)       // if clicking on same tile as selected
                {
                    thisTile.OnDeselected();
                    gameBoard.selectedTile = null;
                }
                else                                             // some tile is selected and its not this so try to swap this and selected tile 
                {
                    gameBoard.StartCoroutine(gameBoard.TrySwapTiles(thisTile)); 
                }
            }
        }


#if UNITY_ANDROID
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (gameBoard.gameState != gameStates.calm) return;

            Vector2 tapPos = GameManager.Instance.lastTapPosition;
            gameBoard.selectedTile = thisTile;
            if (eventData.position.x - tapPos.x > dragDistance && thisTile.column + 1 != gameBoard.columns)
            {
                // swap selected with right
                CellBase anotherCell = gameBoard.board[thisTile.row, thisTile.column + 1];
                if (anotherCell != null)
                {
                    gameBoard.StartCoroutine(gameBoard.TrySwapTiles(anotherCell));
                }

            }
            else if (tapPos.x - eventData.position.x > dragDistance && thisTile.column - 1 != -1)
            {
                // swap selected with left
                CellBase anotherCell = gameBoard.board[thisTile.row, thisTile.column - 1];
                if (anotherCell != null)
                {
                    gameBoard.StartCoroutine(gameBoard.TrySwapTiles(anotherCell));
                }
            }
            else if (eventData.position.y - tapPos.y > dragDistance && thisTile.row + 1 != gameBoard.rows)
            {
                // swap selected with upper
                CellBase anotherCell = gameBoard.board[thisTile.row + 1, thisTile.column];
                if (anotherCell != null)
                {
                    gameBoard.StartCoroutine(gameBoard.TrySwapTiles(anotherCell));
                }
            }
            else if (tapPos.y - eventData.position.y > dragDistance && thisTile.row - 1 != -1)
            {
                // swap selected with lower
                CellBase anotherCell = gameBoard.board[thisTile.row - 1, thisTile.column];
                if (anotherCell != null)
                {
                    gameBoard.StartCoroutine(gameBoard.TrySwapTiles(anotherCell));
                }

            }
        }


        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            GameManager.Instance.lastTapPosition = eventData.position;
        }
#endif
    }
}