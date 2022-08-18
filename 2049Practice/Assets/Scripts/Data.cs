using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.Iv, new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.Ih, new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int( 0, -1) } },
        { Tetromino.Lone, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.Ltwo, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 0, 0), new Vector2Int( 0, -1) } },
        { Tetromino.Lthree, new Vector2Int[] { new Vector2Int( 0, -1), new Vector2Int( 0, 0), new Vector2Int(-1, 0) } },
        { Tetromino.Lfour, new Vector2Int[] { new Vector2Int( -1, 0), new Vector2Int(0, 0), new Vector2Int( 0, 1) } },
    };
    
}
