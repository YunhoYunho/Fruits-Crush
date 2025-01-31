using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private LevelData[] levelData;
    [SerializeField]
    private GameObject stageUI;

    private void OnEnable()
    {
        InitStars();
        ShowStageUI();
    }

    private void InitStars()
    {
        foreach (var level in levelData)
            for (int i = 0; i < level.stars.Length; i++)
                level.stars[i].SetActive(false);
    }

    private void ShowStageUI()
    {
        if (DataManager.Instance.playerData.scoreStar[0] >= 1)
            stageUI.SetActive(true);
        else
            stageUI.SetActive(false);
    }

    private void Start()
    {
        SetButtonActive();
    }

    private void SetButtonActive()
    {
        DataManager.Instance.LoadData();
        for (int i = 0; i < levelData.Length; i++)
        {
            levelData[i].button.interactable = i <= DataManager.Instance.playerData.myLevel;
            int levelIdx = i;
            levelData[i].button.onClick.AddListener(() => SelectGame(levelIdx));
        }
        UpdateStars();
    }

    private void UpdateStars()
    {
        if (null == DataManager.Instance.playerData)
            return;

        for (int i = 0; i < levelData.Length; i++)
        {
            if (i <= DataManager.Instance.playerData.myLevel)
            {
                int cnt = DataManager.Instance.playerData.scoreStar[i];
                for (int j = 0; j < levelData[i].stars.Length; j++)
                    levelData[i].stars[j].SetActive(j < cnt);
            }
            else
                for (int j = 0; j < levelData[i].stars.Length; j++)
                    levelData[i].stars[j].SetActive(false);
        }
    }

    public void NewGame()
    {
        DataManager.Instance.ClearData();
        SetButtonActive();
    }

    public void LoadGame()
    {
        DataManager.Instance.LoadData();
        SetButtonActive();
    }

    public void SelectGame(int levelIndex)
    {
        DataManager.Instance.playerData.selectLevel = levelIndex;
        DataManager.Instance.SaveData();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        DataManager.Instance.SaveData();
        Application.Quit();
    }
}

[Serializable]
public class LevelData
{
    public Button button;
    public GameObject[] stars;
}
