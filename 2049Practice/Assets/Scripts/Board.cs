using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using System.Linq;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; set; }
    public Piece activePiece { get; set; }
    public TetrominoData[] tetrominoes;

    public Vector3Int startPosition;
    public Vector2Int boardSize = new Vector2Int(8, 8);
    public List<Vector3Int> availablePlaces;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    public void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    public void SpawnOnRandomTile()
    {

        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }

        availablePlaces = new List<Vector3Int>();
        for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++)
        {
            for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, 0));
                if (!tilemap.HasTile(localPlace))
                {
                    availablePlaces.Add(localPlace);
                }
            }
        }
        
        
        var randomP = Random.Range(0, availablePlaces.Count());
        
        List<int> randomPlacesList = new List<int>();
        
        for (int i = 0; i < availablePlaces.Count();)
        {
            if (randomPlacesList.Contains(randomP))
            {
                randomP = Random.Range(0, availablePlaces.Count());
            }
            else
            {
                randomPlacesList.Add(randomP);
                i++;
            }
        }

        for (int i = 0; i < availablePlaces.Count(); i++)
        {
            if (!tilemap.HasTile(availablePlaces[randomPlacesList[i]]) && IsValidPosition(activePiece, availablePlaces[randomPlacesList[i]]))
            {
                int randomNum = Random.Range(0, tetrominoes.Length);
                TetrominoData data = tetrominoes[randomNum];
        
                activePiece.Initialize(this, availablePlaces[randomPlacesList[i]], data);
                Set(activePiece);
            }
        }
        
        
        

        /*int randomNum = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[randomNum];
        
        activePiece.Initialize(this, randomPos, data);
        Set(activePiece);*/

    }

    void Start()
    {
        StartSpawn();
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnOnRandomTile();
        }
        //Debug.Log("fuck");

    }


    
    public void StartSpawn()
    {
        int randomNum = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[randomNum];
        
        activePiece.Initialize(this, startPosition, data);
        Set(activePiece);
        
        
    }

/*    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        availablePlaces = new List<Vector3Int>();
        
        RectInt bounds = this.Bounds;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (!tilemap.HasTile(localPlace))
            {
                availablePlaces.Add(localPlace);
            }
        }

        //int randomPos = Random.Range(0, availablePlaces.Count);
        
        for (int i = 0; i < availablePlaces.Count; i++)
        {
            if (!tilemap.HasTile(availablePlaces[i]))
            {
                activePiece.Initialize(this, availablePlaces[i], data);
                Set(activePiece);
            }
        }

        if (availablePlaces.Count == 1)
        {
            GameOver();
        }*/
        
    
    public void GameOver()
    {
        tilemap.ClearAllTiles();
    }
    
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
            
        }
    }

    /*public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
            
        }
    }*/

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            
            if(this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }
    
    
}
