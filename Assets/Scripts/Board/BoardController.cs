using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Board")]
    [SerializeField, Range(1, 6)]
    private int width = 6;
    [SerializeField, Range(3, 8)]
    private int height = 8;
    public GameObject fruitParent;
    public GameObject[] fruitPrefabs;

    [Header("Debug")]
    [SerializeField]
    private Fruits selectedFruit;
    [SerializeField]
    private bool isSwapping;
    [SerializeField]
    private List<Fruits> removeFruitsList = new List<Fruits>();
    [SerializeField]
    private GetPoolObject getPool;

    private float spacingX;
    private float spacingY;
    private Node[,] fruitBoard;
    private List<GameObject> activeFruitList = new List<GameObject>();
    private List<FruitType> typeKindList = new List<FruitType>();
    private List<Fruits> crossList = new List<Fruits>();
    private int[] goalCntList = new int[2];

    private void Awake()
    {
        getPool = FindObjectOfType<GetPoolObject>();
    }

    private void Start()
    {
        InitBoard();
        InitGoalType();
    }

    private void Update()
    {
        ClickFruit();
    }

    private void ClickFruit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (null != hit.collider && hit.collider.gameObject.GetComponent<Fruits>())
            {
                if (isSwapping)
                    return;

                Fruits clickfruit = hit.collider.gameObject.GetComponent<Fruits>();
                SelectFruit(clickfruit);
            }
        }
    }

    private void InitBoard()
    {
        ClearBoard();

        fruitBoard = new Node[width, height];
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + 1.5f;

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                SpawnFruit(x, y);

        if (HasMatchingFruits())
            InitBoard();
    }

    private void SpawnFruit(int x, int y)
    {
        Vector2 pos = new Vector2(x - spacingX, y - spacingY);
        int randomIdx = Random.Range(0, fruitPrefabs.Length);
        GameObject fruit = getPool.GetPool(fruitPrefabs[randomIdx].name, pos, Quaternion.identity);
        fruit.transform.SetParent(fruitParent.transform);
        fruit.GetComponent<Fruits>().SetPos(x, y);
        fruitBoard[x, y] = new Node(true, fruit);
        activeFruitList.Add(fruit);
    }

    private void InitGoalType()
    {
        typeKindList.Add(GameManager.Instance.goal1Type);
        typeKindList.Add(GameManager.Instance.goal2Type);
    }

    private void ClearBoard()
    {
        if (null != activeFruitList)
        {
            foreach (GameObject go in activeFruitList)
                PoolManager.Instance.Release(go);
            activeFruitList.Clear();
        }
    }

    private bool HasMatchingFruits()
    {
        if (GameManager.Instance.isGameEnded)
            return false;

        bool hasMatched = false;
        removeFruitsList.Clear();
        foreach (Node nodeFruit in fruitBoard)
            if (null != nodeFruit.fruit)
                nodeFruit.fruit.GetComponent<Fruits>().isMatched = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (fruitBoard[x, y].isUsable)
                {
                    Fruits curFruits = fruitBoard[x, y].fruit.GetComponent<Fruits>();
                    if (!curFruits.isMatched)
                    {
                        MatchResult matchF = IsMatched(curFruits);
                        if (matchF.matchedFruits.Count >= 3)
                        {
                            MatchResult specialMatching = SpecialMatch(matchF);
                            removeFruitsList.AddRange(specialMatching.matchedFruits);

                            foreach (Fruits f in specialMatching.matchedFruits)
                                f.isMatched = true;
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    private IEnumerator MatchingFruitsRoutine()
    {
        foreach (Fruits removeFruit in removeFruitsList)
        {
            removeFruit.SetMatching();
            if (removeFruit.fruitType == typeKindList[0])
                goalCntList[0]++;
            else if (removeFruit.fruitType == typeKindList[1])
                goalCntList[1]++;
        }
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 2; i++)
        {
            if (goalCntList[i] == 0)
                continue;

            int score = 0;
            if (goalCntList[i] == 3)
                score += (goalCntList[i] * 10);
            else if (goalCntList[i] == 4)
                score += (goalCntList[i] * 20);
            else if (goalCntList[i] >= 5)
                score += (goalCntList[i] * 30);
            GameManager.Instance.AddScore(score);
            GameManager.Instance.MatchFruitsCount(typeKindList[i], goalCntList[i]);
            goalCntList[i] = 0;
        }
        yield return new WaitForSeconds(0.7f);

        foreach (Fruits removeFruit in removeFruitsList)
        {
            removeFruit.isMatched = false;
            PoolManager.Instance.Release(removeFruit.gameObject);
        }

        RefillBoard(removeFruitsList);
        yield return new WaitForSeconds(0.4f);

        if (HasMatchingFruits())
            StartCoroutine(MatchingFruitsRoutine());
    }

    private void RefillBoard(List<Fruits> removeList)
    {
        foreach (Fruits f in removeList)
        {
            int xPos = f.xPos;
            int yPos = f.yPos;
            fruitBoard[xPos, yPos] = new Node(true, null);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (null == fruitBoard[x, y].fruit)
                    RefillNewFruit(x, y);
            }
        }
    }

    private void RefillNewFruit(int x, int y)
    {
        int yOffset = 1;
        while (y + yOffset < height && null == fruitBoard[x, y + yOffset].fruit)
            yOffset++;

        if (y + yOffset < height && null != fruitBoard[x, y + yOffset].fruit)
        {
            Fruits upFruit = fruitBoard[x, y + yOffset].fruit.GetComponent<Fruits>();
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, upFruit.transform.position.z);
            upFruit.MoveToTarget(targetPos);
            upFruit.SetPos(x, y);
            fruitBoard[x, y] = fruitBoard[x, y + yOffset];
            fruitBoard[x, y + yOffset] = new Node(true, null);
        }

        if (y + yOffset == height)
            SpawnNewFruit(x);
    }

    private void SpawnNewFruit(int x)
    {
        int newPos = CheckLowestEmptyPos(x);
        int moveToPos = height - newPos;
        int randomIdx = Random.Range(0, fruitPrefabs.Length);
        GameObject newFruit = getPool.GetPool(fruitPrefabs[randomIdx].name, new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newFruit.transform.SetParent(fruitParent.transform);
        newFruit.GetComponent<Fruits>().SetPos(x, newPos);
        fruitBoard[x, newPos] = new Node(true, newFruit);
        Vector3 targetPos = new Vector3(newFruit.transform.position.x, newFruit.transform.position.y - moveToPos, newFruit.transform.position.z);
        newFruit.GetComponent<Fruits>().MoveToTarget(targetPos);
    }

    private int CheckLowestEmptyPos(int x)
    {
        int lowestEmpty = 99;
        for (int y = height - 1; y >= 0; y--)
        {
            if (null == fruitBoard[x, y].fruit)
                lowestEmpty = y;
        }
        return lowestEmpty;
    }

    private MatchResult SpecialMatch(MatchResult matchResult)
    {
        if (matchResult.dir == MatchDirection.Horizontal ||  matchResult.dir == MatchDirection.LongHorizontal)
        {
            foreach (Fruits f in matchResult.matchedFruits)
            {
                List<Fruits> extraConnectedFruits = new List<Fruits>();

                CheckDirection(f, new Vector2Int(0, 1), extraConnectedFruits);
                CheckDirection(f, new Vector2Int(0, -1), extraConnectedFruits);

                if (extraConnectedFruits.Count >= 2)
                {
                    extraConnectedFruits.AddRange(matchResult.matchedFruits);
                    return new MatchResult
                    {
                        matchedFruits = extraConnectedFruits,
                        dir = MatchDirection.Special
                    };
                }
            }
            return new MatchResult
            {
                matchedFruits = matchResult.matchedFruits,
                dir = matchResult.dir
            };
        }
        else if (matchResult.dir == MatchDirection.Vertical || matchResult.dir == MatchDirection.LongVertical)
        {
            foreach (Fruits f in matchResult.matchedFruits)
            {
                List<Fruits> bonusFruitsList = new List<Fruits>();

                CheckDirection(f, new Vector2Int(1, 0), bonusFruitsList);
                CheckDirection(f, new Vector2Int(-1, 0), bonusFruitsList);

                if (bonusFruitsList.Count >= 2)
                {
                    bonusFruitsList.AddRange(matchResult.matchedFruits);
                    return new MatchResult
                    {
                        matchedFruits = bonusFruitsList,
                        dir = MatchDirection.Special
                    };
                }
            }
            return new MatchResult
            {
                matchedFruits = matchResult.matchedFruits,
                dir = matchResult.dir
            };
        }
        return null;
    }

    private MatchResult IsMatched(Fruits nowFruit)
    {
        List<Fruits> matchedFruitsList = new List<Fruits>();
        FruitType fType = nowFruit.fruitType;
        matchedFruitsList.Add(nowFruit);

        CheckDirection(nowFruit, new Vector2Int(1, 0), matchedFruitsList);
        CheckDirection(nowFruit, new Vector2Int(-1, 0), matchedFruitsList);

        if (matchedFruitsList.Count == 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruitsList,
                dir = MatchDirection.Horizontal
            };
        }
        else if (matchedFruitsList.Count > 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruitsList,
                dir = MatchDirection.LongHorizontal
            };
        }
        matchedFruitsList.Clear();
        matchedFruitsList.Add(nowFruit);
  
        CheckDirection(nowFruit, new Vector2Int(0, 1), matchedFruitsList);
        CheckDirection(nowFruit, new Vector2Int(0, -1), matchedFruitsList);

        if (matchedFruitsList.Count == 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruitsList,
                dir = MatchDirection.Vertical
            };
        }
        else if (matchedFruitsList.Count > 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruitsList,
                dir = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                matchedFruits = matchedFruitsList,
                dir = MatchDirection.None
            };
        }
    }

    private void CheckDirection(Fruits fruits, Vector2Int dir, List<Fruits> matchedFruits)
    {
        FruitType fruitType = fruits.fruitType;
        int x = fruits.xPos + dir.x;
        int y = fruits.yPos + dir.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (fruitBoard[x, y].isUsable)
            {
                Fruits nearFruits = fruitBoard[x, y].fruit.GetComponent<Fruits>();
                if (!nearFruits.isMatched && nearFruits.fruitType == fruitType)
                {
                    matchedFruits.Add(nearFruits);
                    x += dir.x;
                    y += dir.y;
                }
                else
                    break;
            }
            else
                break;
        }
    }

    private void SelectFruit(Fruits curFruit)
    {
        if (isSwapping)
            return;

        if (null == selectedFruit)
        {
            selectedFruit = curFruit;
            selectedFruit.ActiveSelectedUI(true);
        }
        else if (selectedFruit == curFruit)
        {
            selectedFruit.ActiveSelectedUI(false);
            selectedFruit = null;
        }
        else if (selectedFruit != curFruit)
        {
            selectedFruit.ActiveSelectedUI(false);
            SwapFruit(selectedFruit, curFruit);
            selectedFruit = null;
        }
    }

    private void SwapFruit(Fruits curFruit, Fruits targetFruit)
    {
        if (!IsNearFruit(curFruit, targetFruit))
            return;

        SwapTwoFruits(curFruit, targetFruit);
        isSwapping = true;
        StartCoroutine(MatchingRoutine(curFruit, targetFruit));
    }

    private bool IsNearFruit(Fruits curFruit, Fruits targetFruit)
    {
        return Mathf.Abs(curFruit.xPos - targetFruit.xPos) +
            Mathf.Abs(curFruit.yPos - targetFruit.yPos) == 1;
    }

    private void SwapTwoFruits(Fruits curFruit, Fruits targetFruit)
    {
        GameObject temp = fruitBoard[curFruit.xPos, curFruit.yPos].fruit;
        fruitBoard[curFruit.xPos, curFruit.yPos].fruit = fruitBoard[targetFruit.xPos, targetFruit.yPos].fruit;
        fruitBoard[targetFruit.xPos, targetFruit.yPos].fruit = temp;

        int tempXPos = curFruit.xPos;
        int tempYPos = curFruit.yPos;
        curFruit.xPos = targetFruit.xPos;
        curFruit.yPos = targetFruit.yPos;
        targetFruit.xPos = tempXPos;
        targetFruit.yPos = tempYPos;

        curFruit.MoveToTarget(fruitBoard[targetFruit.xPos, targetFruit.yPos].fruit.transform.position);
        targetFruit.MoveToTarget(fruitBoard[curFruit.xPos, curFruit.yPos].fruit.transform.position);
    }

    private IEnumerator MatchingRoutine(Fruits curFruit, Fruits targetFruit)
    {
        yield return new WaitForSeconds(0.2f);

        if (HasMatchingFruits())
            StartCoroutine(MatchingFruitsRoutine());
        else
            SwapTwoFruits(curFruit, targetFruit);
        isSwapping = false;
    }

    public void CheckCrossLine()
    {
        if (null == selectedFruit)
            return;

        crossList.Clear();
        int xPos = selectedFruit.xPos;
        int yPos = selectedFruit.yPos;
        for (int x = 0; x < width; x++)
            crossList.Add(fruitBoard[x, yPos].fruit.GetComponent<Fruits>());
        for (int y = 0; y < height; y++)
        {
            if (y == yPos)
                continue;
            crossList.Add(fruitBoard[xPos, y].fruit.gameObject.GetComponent<Fruits>());
        }
        StartCoroutine(CrossCheckRoutine(crossList));
        selectedFruit.ActiveSelectedUI(false);
        selectedFruit = null;
    }

    private IEnumerator CrossCheckRoutine(List<Fruits> crossList)
    {
        foreach (Fruits removeFruit in crossList)
        {
            removeFruit.SetMatching();
            if (removeFruit.fruitType == typeKindList[0])
                goalCntList[0]++;
            else if (removeFruit.fruitType == typeKindList[1])
                goalCntList[1]++;
        }
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 2; i++)
        {
            if (goalCntList[i] == 0)
                continue;

            GameManager.Instance.MatchFruitsCount(typeKindList[i], goalCntList[i]);
        }
        int score = crossList.Count * 10;
        GameManager.Instance.AddScore(score);
        yield return new WaitForSeconds(0.7f);

        foreach (Fruits removeFruit in crossList)
        {
            removeFruit.isMatched = false;
            PoolManager.Instance.Release(removeFruit.gameObject);
        }

        RefillBoard(crossList);
        yield return new WaitForSeconds(0.4f);

        if (HasMatchingFruits())
            StartCoroutine(MatchingFruitsRoutine());
    }
    
    public void ReleaseAllFruits()
    {
        foreach (Transform tr in fruitParent.transform)
            PoolManager.Instance.Release(tr.gameObject);
    }
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Special,
    None,

    Size,
}
