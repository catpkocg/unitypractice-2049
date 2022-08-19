using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2 Pos => transform.position;

    public Node Node;
    public Block block;

    public void SetBlock(Block block)
    {
        if (block != null)
        {
            Node.OccupiedBlock = null;
        }
        Node = node;
        Node.OccupiedBlock = this;

    }
}
