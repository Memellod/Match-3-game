using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static MainVars;

public class GameManager : MonoBehaviour
{    
    public static GameManager Instance { get; private set; }
    // earned points on level
    int points; 
    // event being invoked on points change during play
    public event ValueChanged<int> PointsChanged;
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
    }
    public void AddPoints(int value)
    {
        points += value;
        if (PointsChanged != null)
        {
            PointsChanged.Invoke(points);
        }
    }

}
