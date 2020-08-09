using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static Sprite[] spriteList;
    public static GameBoard gameBoard;
    public static Canvas canvas;
    public static RectTransform CanvasRect; 
    static Constants()
    {
        if (spriteList == null)
        {
            spriteList = Resources.LoadAll<Sprite>("Sprites");
        }
        if (gameBoard == null)
        {
            gameBoard = Object.FindObjectOfType<GameBoard>();
        }
        if (canvas == null)
        {
            canvas = Object.FindObjectOfType<Canvas>();
            CanvasRect = canvas.GetComponent<RectTransform>();
        }
    }
}
