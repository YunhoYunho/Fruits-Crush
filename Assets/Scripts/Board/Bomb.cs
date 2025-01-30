using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private BoardController controller;
    
    public void OnClickBombButton()
    {
        controller.CrossCheck();
    }
}
