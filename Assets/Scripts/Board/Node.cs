using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isUsable;
    public GameObject fruit;

    public Node(bool isUsableNode, GameObject fruitObject)
    {
        isUsable = isUsableNode;
        fruit = fruitObject;
    }
}
