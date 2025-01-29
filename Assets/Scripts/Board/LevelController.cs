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
    private int unlockedLevel;
    private bool isPlaying = false;

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
        isPlaying = true;
    }

    private void ShowStageUI()
    {
        if (isPlaying)
            stageUI.SetActive(true);
    }

    private void Start()
    {
        SetButtonActive();
    }

    private void SetButtonActive()
    {
        DataManager.Instance.LoadData();
        for (int i = 0; i < levelData.Length; i++)
            levelData[i].button.interactable = i <= DataManager.Instance.playerData.myLevel;
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

    public void SelectGame()
    {
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
