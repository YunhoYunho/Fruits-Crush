using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int curlevel;
    public bool isGameEnded;

    public BoardController boardController;
    public GameObject victoryPanel;
    public TextMeshProUGUI goal1Text;
    public Image goal1Image;
    public TextMeshProUGUI goal2Text;
    public Image goal2Image;

    private FruitType goal1Type;
    private FruitType goal2Type;
    private int goal1Cnt;
    private int goal2Cnt;
    public int Goal1Cnt { get { return goal1Cnt; } private set {  goal1Cnt = value; OnGoal1Changed?.Invoke(goal1Cnt); } }
    public int Goal2Cnt { get { return goal2Cnt; } private set {  goal2Cnt = value; OnGoal2Changed?.Invoke(goal2Cnt); } }
    private UnityEvent<int> OnGoal1Changed = new UnityEvent<int>(); 
    private UnityEvent<int> OnGoal2Changed = new UnityEvent<int>();

    public static GameManager Instance;

    private void Awake()
    {
        if (null == Instance)
            Instance = this;
        else
            Destroy(gameObject);
        boardController = GetComponent<BoardController>();
    }

    private void Start()
    {
        CheckCnt();
        InitGoal();
    }

    private void CheckCnt()
    {
        OnGoal1Changed.AddListener((goal1Cnt) => { goal1Text.text = goal1Cnt.ToString(); });
        OnGoal2Changed.AddListener((goal2Cnt) => { goal2Text.text = goal2Cnt.ToString(); });
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

        goal1Text.text = goal1Cnt.ToString();
        goal2Text.text = goal2Cnt.ToString();
        goal1Image.sprite = boardController.fruitPrefabs[r1].GetComponent<SpriteRenderer>().sprite;
        goal2Image.sprite = boardController.fruitPrefabs[r2].GetComponent<SpriteRenderer>().sprite;
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

    public void CheckClear()
    {
        isGameEnded = (goal1Cnt <= 0 && goal2Cnt <= 0);
        if (isGameEnded)
        {
            Debug.Log("게임 클리어");
            victoryPanel.SetActive(true);
            boardController.fruitParent.gameObject.SetActive(false);
            int nextLevel = curlevel + 1;
            if (nextLevel < 6)
                DataManager.Instance.SetUnlockedLevel(nextLevel);
        }
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}
