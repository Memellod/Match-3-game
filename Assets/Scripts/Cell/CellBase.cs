using Cell.Visuals;
using UnityEngine;

namespace Cell
{
    [RequireComponent(typeof(CellVisuals))]
    public class CellBase : MonoBehaviour
    {
        [SerializeField] internal float scale = 1.25f;
        CellVisuals visual;
        public int row = -1, column = -1;


        private void  Awake()
        {
            visual = GetComponent<CellVisuals>();
        }
        /// <summary>
        /// randomezes type of tile
        /// </summary>
        public void RandomizeTile()
        {
            GetComponent<CellVisuals>().Randomize();
        }

        public void OnSelected()
        {
            visual.image.color = Color.grey;
        }
        public void OnDeselected()
        {
            visual.image.color = Color.white;

        }


        /// <summary>
        /// checks if this tile and provided are same
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsSame(CellBase cell)
        {
            return visual.sprite == cell.visual.sprite;
        }

        public void SetCell(int _row, int _column)
        {
            row = _row;
            column = _column;
        }

    }
}