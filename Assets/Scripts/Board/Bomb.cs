using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private BoardController controller;
    [SerializeField]
    private Button bombButton;

    private void OnEnable()
    {
        if (GameManager.Instance.isBombUsed)
            bombButton.interactable = false;
    }

    public void OnClickBombButton()
    {
        if (GameManager.Instance.isBombUsed)
            return;

        controller.CrossCheck();
        GameManager.Instance.isBombUsed = true;
        bombButton.interactable = false;
    }

}
