using UnityEngine;
using System.Collections;
using ObjectPooling;

namespace Cell.Mover
{
    [RequireComponent(typeof(CellBase))]
    public class CellMover : MonoBehaviour, IPoolable
    {
        // component CellBase
        CellBase cellBase;
        // offset for changing position during using in pool
        static private float Yoffset = 0;
        // speed of falling after spawning and emptying lower cells
        float fallSpeed = 1;
        // for falling down tile if it is just spawned
        private bool IsJustSpawnedCell = true; 


        private void Awake()
        {
            cellBase = GetComponent<CellBase>();
            if (Yoffset == 0)
            {
                var gm = MainVars.gameBoard;
                Yoffset = gm.scaleColumns * gm.rows + gm.transform.position.y;
            }
        }


        /// <summary>
        /// placing tile in cell and defining its row and column
        /// </summary>
        /// <param name="localposition"></param>
        /// <param name="_row"></param>
        /// <param name="_column"></param>
        public void PlaceInCell(Vector3 localposition, int _row, int _column)
        {
            // if considering it is as new tile - make it fall from up to its position
            if (IsJustSpawnedCell)
            {
                IsJustSpawnedCell = false;
                transform.localPosition = localposition;
                StartCoroutine(nameof(FallDown));
            }
            // else just move (right, left , up or down)
            else
            {
                StartCoroutine(nameof(MoveTo), localposition);
            }
            // set its cell
            cellBase.SetCell(_row, _column);
        }

        /// <summary>
        /// Coroutine making object to fall on its position AFTER spawning
        /// </summary>
        /// <returns></returns>
        private IEnumerator FallDown()
        {
            Vector3 end = transform.localPosition;
            transform.localPosition += new Vector3(0, Yoffset, 0);
            Vector3 start = transform.localPosition;
            float t = 0;
            while (transform.localPosition != end)
            {
                yield return new WaitForEndOfFrame();
                t += fallSpeed * Time.deltaTime;
                transform.localPosition = new Vector3(start.x, Mathf.Lerp(start.y, end.y, t), start.z);
            }
            yield return null;
        }
        /// <summary>
        /// Coroutine making object to fall on its position after changing state of board <b>(not for new spawned)</b>
        /// </summary>
        /// <returns></returns>
        public IEnumerator MoveTo(Vector3 localPosition)
        {
            Vector3 end = localPosition;
            Vector3 start = transform.localPosition;
            float t = 0;
            while (transform.localPosition != end)
            {
                yield return new WaitForEndOfFrame();
                t += fallSpeed * Time.deltaTime;
                transform.localPosition = new Vector3(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t), Mathf.Lerp(start.z, end.z, t));
            }
            yield return null;
        }

        void IPoolable.ResetState()
        {
            IsJustSpawnedCell = true;
        }

        GameObject IPoolable.GetGO()
        {
            return ((IPoolable)cellBase).GetGO();
        }
    }
}