using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2 Pos => transform.position;

    public Node Node;

    public void SetBlock(Node node)
    {
        if (Node != null)
        {
            Node.OccupiedBlock = null;
        }
        Node = node;
        Node.OccupiedBlock = this;

    }
}
