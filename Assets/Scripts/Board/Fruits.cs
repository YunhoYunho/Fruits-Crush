using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruits : MonoBehaviour
{
    public FruitType fruitType;

    public int xPos;
    public int yPos;

    private Vector2 curPos;
    private Vector2 targetPos;

    public bool isMatched;
    public bool isMoving;

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

    public void MoveToTarget(Vector2 movePos)
    {
        StartCoroutine(MoveCoroutine(movePos));
    }

    private IEnumerator MoveCoroutine(Vector2 movePos)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPos = transform.position;
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float time = elaspedTime / duration;
            transform.position = Vector2.Lerp(startPos, movePos, time);
            elaspedTime += Time.deltaTime;
            
            yield return null;
        }
        transform.position = movePos;
        isMoving = false;
    }
}

public enum FruitType
{
    Apple,
    Banana,
    Grape,

    Size,
}

