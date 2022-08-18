using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    
    public void Initialize(Board board,Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    /*private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HardLeft();
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HardRight();
            
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            HardDown();
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            HardUp();
        }
    }*/

    private void HardDown()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
    }
    
    private void HardUp()
    {
        while (Move(Vector2Int.up))
        {
            continue;
        }
    }
    
    private void HardLeft()
    {
        while (Move(Vector2Int.left))
        {
            continue;
        }
    }
    
    private void HardRight()
    {
        while (Move(Vector2Int.right))
        {
            continue;
        }
    }


    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
        }

        return valid;
    }
    
    
}

