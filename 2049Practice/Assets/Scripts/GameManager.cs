using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private Node nodePrefab;

    public GameObject[] Blocks;
    public List<Node> Nodes;
    public int[,] Array = new int [8, 8];
    public Transform[] BlockPos;
    public event System.Action EditorRepaint = () => { };

    public GameObject BASE;

    private Vector3 Pos;
    private Vector2[] BlockShape;
    public List<GameObject> MoveBlocks;

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
        GameObject[] Blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in Blocks)
        {
            GameObject.Destroy(block);
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

    bool CanSpawn(Vector3[] RandomBlock)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = 0;
                for (int k = 0; k < RandomBlock.Length; k++)
                {
                    Vector3 CurShapePos = RandomBlock[k] + new Vector3(i, j, 0);
                    if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    ++count;
                }

                if (count == RandomBlock.Length)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    void SpawnBlock()
    {
        int random = Random.Range(0, Blocks.Length);
        int randomX = Random.Range(0, 7);
        int randomY = Random.Range(0, 7);
        var randomPos = new Vector3(randomX, randomY, 0);

        Vector3[] RandomBlock = Blocks[random].GetComponent<Block>().ShapePos;

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

                    for (int k = 0; k < RandomBlock.Length; k++)
                    {
                        Vector3 CurShapePos = RandomBlock[k] + new Vector3(i, j, 0);
                        if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                        if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                        ++count;

                    }

                    if (count == RandomBlock.Length)
                    {
                        validPlaces.Add(new Vector3(i, j, 0));
                    }

                }
            }

            int randomSpawn = Random.Range(0, validPlaces.Count);

            var SpawnPos = validPlaces[randomSpawn];
            
            var moveBlock = Instantiate(Blocks[random], SpawnPos, Quaternion.identity);
            MoveBlocks.Add(moveBlock);

            for (int k = 0; k < RandomBlock.Length; k++)
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

            var MovingBlock = MoveBlocks[i].GetComponent<Block>().BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.y).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].GetComponent<Block>().ShapePos;

            var Arraychange = blockMove[k].GetComponent<Block>().ShapePos;

            for (int n = 0; n < Arraychange.Length; n++)
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
            

            for (int n = 0; n < Arraychange.Length; n++)
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

            var MovingBlock = MoveBlocks[i].GetComponent<Block>().BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderBy(b => b.transform.position.y).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].GetComponent<Block>().ShapePos;

            var Arraychange = blockMove[k].GetComponent<Block>().ShapePos;

            for (int n = 0; n < Arraychange.Length; n++)
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

            for (int n = 0; n < Arraychange.Length; n++)
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

            var MovingBlock = MoveBlocks[i].GetComponent<Block>().BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderBy(b => b.transform.position.x).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].GetComponent<Block>().ShapePos;

            var Arraychange = blockMove[k].GetComponent<Block>().ShapePos;

            for (int n = 0; n < Arraychange.Length; n++)
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

            for (int n = 0; n < Arraychange.Length; n++)
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

            var MovingBlock = MoveBlocks[i].GetComponent<Block>().BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.x).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].GetComponent<Block>().ShapePos;

            var Arraychange = blockMove[k].GetComponent<Block>().ShapePos;

            for (int n = 0; n < Arraychange.Length; n++)
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

            for (int n = 0; n < Arraychange.Length; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
    }
    
    
    
    public bool IsValidPosition(GameObject block, Vector3 position)
    {
        var Block = block.GetComponent<Block>().ShapePos;
        
        // The position is only valid if every cell is valid
        for (int i = 0; i < Block.Length; i++)
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






