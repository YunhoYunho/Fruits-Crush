using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
    public int level;
    public bool isGameEnded;

    private FruitType goal1Type;
    private FruitType goal2Type;
    private int goal1Cnt;
    private int goal2Cnt;
    public int Goal1Cnt { get { return goal1Cnt; } private set {  goal1Cnt = value; OnGoal1Changed?.Invoke(goal1Cnt); } }
    public int Goal2Cnt { get { return goal2Cnt; } private set {  goal2Cnt = value; OnGoal2Changed?.Invoke(goal2Cnt); } }
    private UnityEvent<int> OnGoal1Changed = new UnityEvent<int>(); 
    private UnityEvent<int> OnGoal2Changed = new UnityEvent<int>();

    private void OnEnable()
    {
        InitGoal();
    }

    private void Start()
    {
        CheckCnt();
    }

    private void CheckCnt()
    {
        OnGoal1Changed.AddListener((goal1Cnt) => { UIManager.Instance.goal1Text.text = goal1Cnt.ToString(); });
        OnGoal2Changed.AddListener((goal2Cnt) => { UIManager.Instance.goal2Text.text = goal2Cnt.ToString(); });
    }

    private void InitGoal()
    {
        int r1 = Random.Range(0, 5);
        int r2 = Random.Range(0, 5);
        while (r1 == r2)
            r2 = Random.Range(0, 5);

        goal1Type = (FruitType)r1;
        goal2Type = (FruitType)r2;
        goal1Cnt = Random.Range(10, 15);
        goal2Cnt = Random.Range(10, 15);

        UIManager.Instance.goal1Text.text = goal1Cnt.ToString();
        UIManager.Instance.goal2Text.text = goal2Cnt.ToString();
        UIManager.Instance.goal1Image.sprite = BoardManager.Instance.fruitPrefabs[r1].GetComponent<SpriteRenderer>().sprite;
        UIManager.Instance.goal2Image.sprite = BoardManager.Instance.fruitPrefabs[r2].GetComponent<SpriteRenderer>().sprite;
    }

    public void MatchFruitsCount(FruitType type, int cnt)
    {
        if (type == goal1Type)
        {
            Goal1Cnt -= cnt;
            if (Goal1Cnt < 0)
                Goal1Cnt = 0;
        }
        else if (type == goal2Type)
        {
            Goal2Cnt -= cnt;
            if (Goal2Cnt < 0)
                Goal2Cnt = 0;
        }
        CheckClear();
    }

    private void CheckClear()
    {
        isGameEnded = (goal1Cnt <= 0 && goal2Cnt <= 0);
        if (isGameEnded)
            Debug.Log("게임 클리어");
    }
}
