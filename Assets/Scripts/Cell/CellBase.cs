using Cell.Visuals;
using ObjectPooling;
using UnityEngine;

namespace Cell
{
    [RequireComponent(typeof(CellVisuals))]
    public class CellBase : MonoBehaviour, IPoolable
    {
        // visual component
        CellVisuals visual;
        // row and column in board 
        public int row = -1, column = -1;
        // enum for types
        internal enum Types { simple, blocking}

        private Types type;
        public bool isViewed = false; // used in function to find smth
        private void Awake()
        {
            visual = GetComponent<CellVisuals>();
        }
        /// <summary>
        /// randomizes type of tile
        /// </summary>
        public void RandomizeTile()
        {
            // now its type defined by visual component   
           visual.Randomize();
           type = Types.simple; 
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
            
        }

        /// method for getting game object script attached to
        GameObject IPoolable.GetGO()
        {
            return gameObject;
        }

        public bool IsMoveable()
        {
            return type != Types.blocking;
        }
    }
}