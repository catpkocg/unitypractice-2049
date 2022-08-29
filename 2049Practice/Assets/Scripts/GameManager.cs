using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class GameManager : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private Node nodePrefab;

    public Block[] BlockPrefabs;
    public Block[] OneBlock;
    public List<Node> Nodes;
    public int[,] Array = new int [8, 8];
    public event System.Action EditorRepaint = () => { };

    public GameObject BASE;

    private Vector3 Pos;
    private Vector2[] BlockShape;
    public List<Block> MoveBlocks;

    public List<Vector3> Good = new List<Vector3>();

    void Start()
    {
        GenerateGrid();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBlock();
        }

        if (Input.GetMouseButtonDown(0))
        {
            LineClear();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
        
    }
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                node.transform.SetParent(BASE.transform);
                Nodes.Add(node);
            }
        }

        var center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);

        Camera.main.transform.position = new Vector3(center.x, center.y, -20);
    }

    void GameOver()
    {
        foreach (Block block in MoveBlocks)
        {
            Destroy(block);
        }

        Debug.Log("Game Over");
    }
    
    bool InRange(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return false;
        }

        return true;
    }
    
    bool Possible(int x, int y)
    {
        if (Array[x, y] != 0)
        {
            return false;
        }

        return true;
    }

    bool CanSpawn(List<Vector3> RandomBlock)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = 0;
                for (int k = 0; k < RandomBlock.Count; k++)
                {
                    Vector3 CurShapePos = RandomBlock[k] + new Vector3(i, j, 0);
                    if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    ++count;
                }

                if (count == RandomBlock.Count)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    void SpawnBlock()
    {
        var random = Random.Range(0, BlockPrefabs.Length);
        var randomX = Random.Range(0, 7);
        var randomY = Random.Range(0, 7);
        var randomPos = new Vector3(randomX, randomY, 0);

        var RandomBlock = BlockPrefabs[random].ShapePos;

        if (CanSpawn(RandomBlock) == false)
        {
            GameOver();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Array[x, y] = 1;
                }
            }
        }

        else
        {
            List<Vector3> validPlaces = new List<Vector3>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int count = 0;

                    for (int k = 0; k < RandomBlock.Count; k++)
                    {
                        Vector3 CurShapePos = RandomBlock[k] + new Vector3(i, j, 0);
                        if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                        if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                        ++count;

                    }

                    if (count == RandomBlock.Count)
                    {
                        validPlaces.Add(new Vector3(i, j, 0));
                    }

                }
            }

            int randomSpawn = Random.Range(0, validPlaces.Count);

            var SpawnPos = validPlaces[randomSpawn];
            
            var moveBlock = Instantiate(BlockPrefabs[random], SpawnPos, Quaternion.identity);
            MoveBlocks.Add(moveBlock);

            for (int k = 0; k < RandomBlock.Count; k++)
            {
                Vector3 getposition = RandomBlock[k] + SpawnPos;
                Array[(int)getposition.x, (int)getposition.y] = 1;
            }
        }


    }
    
    void MoveUp()
    {
        List<Vector3> MovingBlocks = new List<Vector3>();

        for (int i = 0; i < MoveBlocks.Count; i++)
        {

            var MovingBlock = MoveBlocks[i].BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.y).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].ShapePos;

            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.y;
            Vector3 goodPos = presentPos;
            for (int i = current; i < height; i++)
            {
                var position = new Vector3(presentPos.x, i, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.2f);
            

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
    }
    void MoveDown()
    {
        List<Vector3> MovingBlocks = new List<Vector3>();

        for (int i = 0; i < MoveBlocks.Count; i++)
        {

            var MovingBlock = MoveBlocks[i].BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderBy(b => b.transform.position.y).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].ShapePos;

            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.y;
            Vector3 goodPos = presentPos;
            for (int i = current; i >= 0; i--)
            {
                var position = new Vector3(presentPos.x, i, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.2f);

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
    }
    void MoveLeft()
    {
        List<Vector3> MovingBlocks = new List<Vector3>();

        for (int i = 0; i < MoveBlocks.Count; i++)
        {

            var MovingBlock = MoveBlocks[i].BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderBy(b => b.transform.position.x).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].ShapePos;

            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.x;
            Vector3 goodPos = presentPos;
            for (int i = current; i >= 0; i--)
            {
                var position = new Vector3(i, presentPos.y, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.2f);

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
    }
    
    void MoveRight()
    {
        List<Vector3> MovingBlocks = new List<Vector3>();

        for (int i = 0; i < MoveBlocks.Count; i++)
        {

            var MovingBlock = MoveBlocks[i].BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.x).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.x;
            Vector3 goodPos = presentPos;
            for (int i = current; i < width; i++)
            {
                var position = new Vector3(i, presentPos.y, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.2f);

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
        
        
    }

    public void LineClear()
    {
        int oneLine = 0;

        for (int i= 0; i < width; i++)
        {
            int verticalCount = 0;
            for (int j = 0; j < height; j++) 
            {
                if (Array[i, j] != 0)
                {
                    ++verticalCount;
                }
            }
            
            if (verticalCount == 8)
            {
                ++oneLine;
                for (int j = 0; j < height; j++)
                {
                    Array[i, j] = -1;
                }
                Debug.Log(oneLine);
                
            }
        }
        
        List<GameObject> children = new List<GameObject>();
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Array[i, j] == -1)
                {
                    Array[i, j] = 0;
                    
                    for (int k = 0; k < MoveBlocks.Count; k++)
                    {
                        for (int n = MoveBlocks[k].transform.childCount -1; n >= 0; n--)
                        {
                            
                            GameObject child = MoveBlocks[k].transform.GetChild(n).gameObject;
                            
                            if (child.transform.position == new Vector3(i, j, 0))
                            {
                                Destroy(child);
                                MoveBlocks[k].ShapePos.Remove(new Vector3(i, j, 0)-MoveBlocks[k].Pos);

                            }
                            
                        }

                        if (MoveBlocks[k].transform.childCount == 2 &&
                            MoveBlocks[k].ShapePos[0] != new Vector3(0, 0, 0))
                        {
                            var one = Instantiate(OneBlock[0], MoveBlocks[k].ShapePos[0], Quaternion.identity);
                            var two = Instantiate(OneBlock[0], MoveBlocks[k].ShapePos[1], Quaternion.identity);
                            Destroy(MoveBlocks[k]);
                            MoveBlocks.Add(one);
                            MoveBlocks.Add(two);
                        }

                    }
                    
                }
            }
        }

        
    }
    
    
    
    public bool IsValidPosition(Block block, Vector3 position)
    {
        var Block = block.ShapePos;
        
        // The position is only valid if every cell is valid
        for (int i = 0; i < Block.Count; i++)
        {
            Vector3 nowPosition = Block[i] + position;

            // An out of bounds tile is invalid
            if (!InRange((int)nowPosition.x, (int)nowPosition.y))
            {
                return false;
            }

            if (!Possible((int)nowPosition.x, (int)nowPosition.y)) {
                return false;
            }
        }

        return true;
    }

    
}






