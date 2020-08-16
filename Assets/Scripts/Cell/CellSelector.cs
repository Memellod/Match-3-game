using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using GameBoards;
using static MainVars;

namespace Cell.Selector
{
    /// <summary>
    /// Class for handling tile selection
    /// </summary>
    [RequireComponent(typeof(CellBase))]
    public class CellSelector : MonoBehaviour, IPointerClickHandler
    {
        CellBase thisTile;

        private void Start()
        {
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
    }
}