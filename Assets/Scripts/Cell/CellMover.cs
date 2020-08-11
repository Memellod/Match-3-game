using UnityEngine;
using System.Collections;

namespace Cell.Mover
{
    [RequireComponent(typeof(CellBase))]
    public class CellMover : MonoBehaviour
    {
        CellBase cellBase;

        static private float Yoffset = 0;
        float fallSpeed = 1;
        private bool IsJustSpawnedCell = true; // for falling down tile if it is just spawned


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
            if (IsJustSpawnedCell)
            {
                IsJustSpawnedCell = false;
                transform.localPosition = localposition;
                StartCoroutine("FallDown"); /// see <see cref="FallDown()"/>
            }
            else
            {
                StartCoroutine("MoveTo", localposition);/// see <see cref="MoveTo(Vector3)"/>
            }
            cellBase.SetCell(_row, _column);
        }

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
    }
}