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

        controller.CheckCrossLine();
        GameManager.Instance.isBombUsed = true;
        bombButton.interactable = false;
    }

}
