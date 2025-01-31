using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fruits : MonoBehaviour
{
    public FruitType fruitType;

    public int xPos;
    public int yPos;

    public bool isMatched;
    public bool isMoving;

    private Animator animator;
    private Image selectImage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        selectImage = GetComponentInChildren<Image>();
    }

    public void SetPos(int xPosition, int yPosition)
    {
        xPos = xPosition;
        yPos = yPosition;
    }

    public void MoveToTarget(Vector2 movePos)
    {
        if (!gameObject.activeSelf || GameManager.Instance.isGameEnded)
            return;

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

    public void SetMatching()
    {
        animator.SetTrigger("Pang");
    }

    public void ActiveSelectedUI(bool isActive)
    {
        Color color = selectImage.color;
        color.a = isActive ? 1 : 0;
        selectImage.color = color;
    }
}

public enum FruitType
{
    Apple,
    Banana,
    Blueberry,
    Grape,
    Orange,

    Size,
}
