using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] float scale = 1.25f;
    static private float Yoffset = 0;
    float fallSpeed = 1;
    public Sprite sprite;
    Image image;

    public int row = -1, column = -1;

    private bool IsJustSpawnedTile = true; // for falling down tile if it is just spawned

    private void Awake()
    {
        if (Yoffset == 0)
        {
            GameBoard gm = FindObjectOfType<GameBoard>();
            Yoffset = gm.scaleColumns * gm.rows + gm.transform.position.y;
        }
    }

    /// <summary>
    /// randomezes type of tile
    /// </summary>
    public void RandomizeTile()
    {
        sprite = MainVars.spriteList[Random.Range(0, MainVars.spriteList.Length)];
        name = sprite.name;
    }

    public void OnSelected()
    {
        image.color = Color.grey;
    }
    public void OnDeselected()
    {
        if (image != null)
        {
            image.color = Color.white;
        }
    }


    /// <summary>
    /// checks if this tile and provided are same
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsSame(Tile tile)
    {
        return sprite == tile.sprite;
    }

    /// <summary>
    /// placing tile in cell and defining its row and column
    /// </summary>
    /// <param name="localposition"></param>
    /// <param name="_row"></param>
    /// <param name="_column"></param>
    public void PlaceInCell(Vector3 localposition, int _row, int _column)
    {
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
            image.sprite = sprite;

            image.transform.localScale = Vector3.one * scale;
        }
        if (IsJustSpawnedTile)
        {
            transform.localPosition = localposition;
            IsJustSpawnedTile = false;
            StartCoroutine("FallDown");
        }
        else
        {
            if (row != -1)
            {
                StartCoroutine("MoveTo", localposition);
            }
        }
        row = _row;
        column = _column;
    }

    IEnumerator FallDown()
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
    private void OnDisable()
    {
       // clip/animation of death
      
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (MainVars.gameBoard.selectedTile == null)           // no selected tile
        {
            OnSelected();
            MainVars.gameBoard.selectedTile = this;
        }
        else
        {
            if (MainVars.gameBoard.selectedTile == this)       // if clicking on same tile as selected
            {
                OnDeselected();
                MainVars.gameBoard.selectedTile = null;
            }
            else                                             // clicking on another tile 
            {
                MainVars.gameBoard.StartCoroutine("TrySwapTiles", this);
            }
        }
    }
}
