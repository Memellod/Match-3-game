using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainVars
{
    public static Sprite[] spriteList;
    public static GameBoard gameBoard;
    public delegate void ValueChanged<T>(T new_value);
    public enum gameStates { falling, exploding, calm };
    static MainVars()
    {

        spriteList = Resources.LoadAll<Sprite>("Sprites");
        gameBoard = Object.FindObjectOfType<GameBoard>();

    }
}
