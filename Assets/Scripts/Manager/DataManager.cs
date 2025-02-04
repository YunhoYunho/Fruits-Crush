using System;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static GameObject container;
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<DataManager>();
                if (null == instance)
                {
                    container = new GameObject("DataManager");
                    instance = container.AddComponent<DataManager>();
                    DontDestroyOnLoad(container);
                }
            }
            return instance;
        }
    }

    private const string GameDataFileName = "GameData.json";
    public PlayerData playerData = new PlayerData();

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;
        if (File.Exists(filePath))
        {
            string fromJsonData = File.ReadAllText(filePath);
            playerData = JsonUtility.FromJson<PlayerData>(fromJsonData);
        }
    }

    public void SaveData()
    {
        string toJsonData = JsonUtility.ToJson(playerData, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;
        File.WriteAllText(filePath, toJsonData);
    }

    public void ClearData()
    {
        playerData = new PlayerData
        {
            myLevel = 0,
            scoreStar = new[] { 0, 0, 0, 0, 0, 0 },
            selectLevel = 0
        };
        SaveData();
    }
}

[Serializable]
public class PlayerData
{
    public int myLevel;
    public int[] scoreStar = new int[6];
    public int selectLevel;
}
