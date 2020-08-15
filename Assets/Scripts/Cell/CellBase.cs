using Cell.Visuals;
using ObjectPooling;
using UnityEngine;

namespace Cell
{
    [RequireComponent(typeof(CellVisuals))]
    public class CellBase : MonoBehaviour, IPoolable
    {
        // scale of a image
        [SerializeField] internal float scale = 1.5f;
        // visual component
        CellVisuals visual;
        // row and column in board 
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
            // now its type defined by visual component
           visual.Randomize();
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
            // now its type defined by visual component
            return visual.sprite == cell.visual.sprite;
        }

        /// <summary>
        /// sets row and column of a cell
        /// </summary>
        /// <param name="_row"></param>
        /// <param name="_column"></param>
        public void SetCell(int _row, int _column)
        {
            row = _row;
            column = _column;
        }

        /// <summary>
        ///  reset state for further using with no instantiating
        /// </summary>
        void IPoolable.ResetState()
        {
            SetCell(-1, -1);
            //transform.position = Vector3.one * 10000;
            
        }

        /// method for getting game object script attached to
        GameObject IPoolable.GetGO()
        {
            return gameObject;
        }
    }
}