using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    Iv,
    Ih,
    Lone,
    Ltwo,
    Lthree,
    Lfour,
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells{ get; private set; }

    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino];
    }
}
