using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isGameEnded;

    public BoardController boardController;
    public GameObject victoryPanel;
    public TextMeshProUGUI goal1Text;
    public Image goal1Image;
    public TextMeshProUGUI goal2Text;
    public Image goal2Image;
    public TextMeshProUGUI curLevelText;
    public TextMeshProUGUI curScoreText;
    public FruitType goal1Type;
    public FruitType goal2Type;

    public bool isBombUsed = false;
    private int goal1Cnt;
    private int goal2Cnt;
    private int curScore;
    public int Goal1Cnt { get { return goal1Cnt; } private set { goal1Cnt = value; OnGoal1Changed?.Invoke(goal1Cnt); } }
    public int Goal2Cnt { get { return goal2Cnt; } private set { goal2Cnt = value; OnGoal2Changed?.Invoke(goal2Cnt); } }
    public int CurScore { get { return curScore; } private set { curScore = value; OnScoreChanged?.Invoke(curScore); } }
    private UnityEvent<int> OnGoal1Changed = new UnityEvent<int>();
    private UnityEvent<int> OnGoal2Changed = new UnityEvent<int>();
    private UnityEvent<int> OnScoreChanged = new UnityEvent<int>();

    public static GameManager Instance;

    private void Awake()
    {
        if (null == Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CheckCnt();
        InitUITexts();
        ShowCurLevelUI();
    }

    private void CheckCnt()
    {
        OnGoal1Changed.AddListener((goal1Cnt) => { goal1Text.text = goal1Cnt.ToString(); });
        OnGoal2Changed.AddListener((goal2Cnt) => { goal2Text.text = goal2Cnt.ToString(); });
        OnScoreChanged.AddListener((scoreCnt) => { curScoreText.text = scoreCnt.ToString(); });
    }

    private void InitUITexts()
    {
        int r1 = Random.Range(0, 5);
        int r2 = Random.Range(0, 5);
        while (r1 == r2)
            r2 = Random.Range(0, 5);

        goal1Type = (FruitType)r1;
        goal2Type = (FruitType)r2;
        goal1Cnt = Random.Range(10, 13);
        goal2Cnt = Random.Range(10, 13);
        curScore = 0;

        goal1Text.text = goal1Cnt.ToString();
        goal2Text.text = goal2Cnt.ToString();
        goal1Image.sprite = boardController.fruitPrefabs[r1].GetComponent<SpriteRenderer>().sprite;
        goal2Image.sprite = boardController.fruitPrefabs[r2].GetComponent<SpriteRenderer>().sprite;
    }

    private void ShowCurLevelUI()
    {
        int level = DataManager.Instance.playerData.selectLevel + 1;
        curLevelText.text = $"Level : {level}";
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
            StartCoroutine(VictoryPanelRoutine());

            int selectedLevel = DataManager.Instance.playerData.selectLevel;
            if (selectedLevel == DataManager.Instance.playerData.myLevel)
                if (selectedLevel < DataManager.Instance.playerData.scoreStar.Length - 1)
                    DataManager.Instance.playerData.myLevel = selectedLevel + 1;
            if (StarCnt(curScore) > DataManager.Instance.playerData.scoreStar[selectedLevel])
                DataManager.Instance.playerData.scoreStar[selectedLevel] = StarCnt(curScore);
            DataManager.Instance.SaveData();
        }
    }

    private IEnumerator VictoryPanelRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        boardController.fruitParent.gameObject.SetActive(false);
        boardController.ReleaseAllFruits();
        victoryPanel.SetActive(true);
    }

    public void AddScore(int score)
    {
        CurScore += score;
        curScoreText.text = curScore.ToString();
    }

    public int StarCnt(int score)
    {
        if (score <= 100)
            return 1;
        else if (score > 100 && score <= 200)
            return 2;
        else if (score > 200)
            return 3;
        return 1;
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
