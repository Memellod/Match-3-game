using Cell.Visuals;
using ObjectPooling;
using UnityEngine;

namespace Cell
{
    [RequireComponent(typeof(CellVisuals))]
    public class CellBase : MonoBehaviour, IPoolable
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
            return visual.sprite == cell.visual.sprite;
        }

        public void SetCell(int _row, int _column)
        {
            row = _row;
            column = _column;
        }

        void IPoolable.ResetState()
        {
            SetCell(-1, -1);
            
           // transform.position = Vector3.one * 10000;
        }

        GameObject IPoolable.GetGO()
        {
            return gameObject;
        }
    }
}