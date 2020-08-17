using Cell;
using ObjectPooling;
using System.Collections.Generic;
using UnityEngine;
using static MainVars;

public class GameManager : MonoBehaviour
{    
    public static GameManager Instance { get; private set; }
    // earned points on level
    int points; 
    // event being invoked on points change during play
    public event ValueChanged<int> PointsChanged;
    // floating text
    [SerializeField] public GameObject floatingText;
    private ObjectPool textPool;
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        
    }
    void Initialize()
    {
        PointsChanged = (int value) => { };         // creating delegate
        points = 0;
        int maxMatches = gameBoard.rows * gameBoard.columns / 3;
        textPool = new ObjectPool(maxMatches, floatingText);
    }

    private void AddPoints(int value)
    {
        points += value;
        PointsChanged?.Invoke(points);

    }

    public void CalculatePointsOfMatch(List<CellBase> matchedTile)
    {
        // get points for matching
        int points = (int)(matchedTile.Count * 40 + Mathf.Pow(2, matchedTile.Count) * 10);
        AddPoints(points);

        Vector3 position = Vector3.zero;
        foreach (CellBase cell in matchedTile)
        {
            position += gameBoard.Position(cell.row, cell.column);
        }
        position /= matchedTile.Count;

        var textGO = textPool.GetObject();
        textGO.transform.SetParent(gameBoard.transform);
        textGO.GetComponent<TextFading>().StartAnimation("+" + points, position); 

    }

    public void AddToPool(GameObject go)
    {
        textPool.AddObject(go);
    }
    


}
