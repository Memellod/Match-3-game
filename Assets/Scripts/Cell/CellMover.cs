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
        // for falling down tile if it is just spawned
        private bool IsJustSpawnedCell = true;


        [SerializeField] public float swapSpeed, fallSpeed;

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
                StartCoroutine(MoveTo(fallSpeed, localposition, Yoffset * Vector3.up));
            }
            // else just fall
            else
            {
                StartCoroutine(MoveTo(fallSpeed, localposition, Vector3.zero));
            }
            // set its cell
            cellBase.SetCell(_row, _column);
        }

        /// <summary>
        /// Coroutine making object to fall on its position AFTER spawning
        /// </summary>
        /// <returns></returns>
        public IEnumerator MoveTo(float speed, Vector3 end, Vector3 offset)
        {
            transform.localPosition += offset;
            Vector3 start = transform.localPosition;
            float t = 0;
            while (transform.localPosition != end)
            {
                yield return new WaitForEndOfFrame();
                t += speed * Time.deltaTime;
                transform.localPosition = new Vector3(Mathf.Lerp(start.x, end.x,t ),
                                                    Mathf.Lerp(start.y, end.y, t), 
                                                      Mathf.Lerp(start.z, end.z, t));
            }
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