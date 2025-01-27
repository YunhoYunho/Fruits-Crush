using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private GameObject levelButtons;
    [SerializeField]
    private Button[] buttons;
    private int unlockedLevel;

    private void OnEnable()
    {
        InitButton();
    }

    private void InitButton()
    {
        int cnt = levelButtons.transform.childCount;
        buttons = new Button[cnt];
        for (int i = 0; i < cnt; i++)
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        SetLevel();
    }

    private void SetLevel()
    {
        DataManager.Instance.LoadData();
        for (int i = 0; i < DataManager.Instance.playerData.isUnlock.Length; i++)
            if (DataManager.Instance.playerData.isUnlock[i])
                unlockedLevel = i;
        SetButtonActive();
    }

    private void SetButtonActive()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].interactable = i <= unlockedLevel;
    }

    public void NewGame()
    {
        DataManager.Instance.ClearData();
        SetLevel();
    }

    public void LoadGame()
    {
        DataManager.Instance.LoadData();
        SetLevel();
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
