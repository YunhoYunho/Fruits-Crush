using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Grape,

    Size,
}

public class Fruits : MonoBehaviour
{
    public FruitType fruitType;

    public int xPos;
    public int yPos;

    private Vector2 curPos;
    private Vector2 targetPos;

    public Fruits(int xPosition, int yPosition)
    {
        xPos = xPosition;
        yPos = yPosition;
    }

    public void SetPos(int xPosition, int yPosition)
    {
        xPos = xPosition;
        yPos = yPosition;
    }
}
