using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 8;
    public float spacingX;
    public float spacingY;

    public GameObject[] fruitPrefabs;
    public Node[,] fruitBoard;
    public GameObject fruitObject;

    public List<GameObject> destroyFruitList = new List<GameObject>();

    public static FruitBoard Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitBoard();
    }

    private void InitBoard()
    {
        DestroyObject();
        fruitBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 pos = new Vector2(x - spacingX, y - spacingY);
                int randomIdx = Random.Range(0, fruitPrefabs.Length);

                GameObject fruit = Instantiate(fruitPrefabs[randomIdx], pos, Quaternion.identity);
                fruit.GetComponent<Fruits>().SetPos(x, y);
                fruitBoard[x, y] = new Node(true, fruit);
                destroyFruitList.Add(fruit);
            }
        }
        if (CheckBoard())
            InitBoard();
    }

    private void DestroyObject()
    {
        if (null != destroyFruitList)
        {
            foreach (GameObject go in destroyFruitList)
                Destroy(go);

            destroyFruitList.Clear();
        }
    }

    public bool CheckBoard()
    {
        bool hasMatched = false;
        List<Fruits> removeFruitsList = new List<Fruits>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (fruitBoard[x, y].isUsable)
                {
                    Fruits curFruits = fruitBoard[x, y].fruit.GetComponent<Fruits>();
                    if (!curFruits.isMatched)
                    {
                        MatchResult matchF = IsConnected(curFruits);
                        if (matchF.matchedFruits.Count >= 3)
                        {
                            removeFruitsList.AddRange(matchF.matchedFruits);

                            foreach (Fruits f in matchF.matchedFruits)
                                f.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    private MatchResult IsConnected(Fruits nowFruit)
    {
        List<Fruits> matchedFruits = new List<Fruits>();
        FruitType fType = nowFruit.fruitType;
        matchedFruits.Add(nowFruit);

        // ÁÂ¿ì
        CheckDirection(nowFruit, new Vector2Int(1, 0), matchedFruits);
        CheckDirection(nowFruit, new Vector2Int(-1, 0), matchedFruits);

        if (matchedFruits.Count == 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruits,
                dir = MatchDirection.Horizontal
            };
        }
        else if (matchedFruits.Count > 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruits,
                dir = MatchDirection.LongHorizontal
            };
        }
        matchedFruits.Clear();
        matchedFruits.Add(nowFruit);
  
        // »óÇÏ
        CheckDirection(nowFruit, new Vector2Int(0, 1), matchedFruits);
        CheckDirection(nowFruit, new Vector2Int(0, -1), matchedFruits);

        if (matchedFruits.Count == 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruits,
                dir = MatchDirection.Vertical
            };
        }
        else if (matchedFruits.Count > 3)
        {
            return new MatchResult
            {
                matchedFruits = matchedFruits,
                dir = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                matchedFruits = matchedFruits,
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
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    None,

    Size,
}

public class MatchResult
{
    public List<Fruits> matchedFruits;
    public MatchDirection dir;
}
