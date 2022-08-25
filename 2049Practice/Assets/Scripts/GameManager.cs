using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


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

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Blocks[0], new Vector3(3, 3, 0), Quaternion.identity);
        Array[3, 3] = 1;
        Array[2, 3] = 1;
        Array[4, 3] = 1;


        GenerateGrid();

    }

    // Update is called once per frame


    //생성할때 그리드추가 + 블락 번호 생성
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            SpawnBlock();

        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            MoveRight();

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
        //List<Vector3> validPlaces = new List<Vector3>();

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


            var SpawnPos = validPlaces[0];


            Debug.Log(SpawnPos);

            var moveBlock = Instantiate(Blocks[random], SpawnPos, Quaternion.identity);
            MoveBlocks.Add(moveBlock);

            for (int k = 0; k < RandomBlock.Length; k++)
            {
                Vector3 getposition = RandomBlock[k] + SpawnPos;
                Array[(int)getposition.x, (int)getposition.y] = 1;
            }
        }


    }

    private void HardRight()
    {


    }

    void MoveRight()
    {
        List<Vector3> MovingBlocks = new List<Vector3>();

        for (int i = 0; i < MoveBlocks.Count; i++)
        {

            var MovingBlock = MoveBlocks[i].GetComponent<Block>().BlockPos;
            MovingBlocks.Add(MovingBlock);

        }

        //var blockMove = MovingBlocks.OrderByDescending(x => x.x).ToList();
        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.x).ToList();

        //Debug.Log(blockMove[4]);


        bool valid = InRange((int)blockMove[0].transform.position.x, (int)blockMove[0].transform.position.y) &&
                     Possible((int)blockMove[0].transform.position.x, (int)blockMove[0].transform.position.y);


        var nn = blockMove[0].GetComponent<Block>().ShapePos;

        for (int n = 0; n < nn.Length; n++)
        {
            Array[(int)blockMove[0].transform.position.x + (int)nn[n].x,
                (int)blockMove[0].transform.position.y + (int)nn[n].y] = 0;
        }

        var presentPos = blockMove[0].transform.position;

        presentPos = blockMove[0].transform.position + new Vector3(1, 0, 0);









        for (int k = 0; k < blockMove.Count; k++)
        {
            var nowposition = blockMove[k].GetComponent<Block>().ShapePos;

            for (int m = 0; m < nowposition.Length; m++)
            {
                Vector3 CurShapePos = nowposition[m] + blockMove[k].transform.position;
                if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;

            }


        }

    }

    private bool Move(Vector3 translation, Vector3 position)
    {
        Vector3 newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = InRange((int)newPosition.x, (int)newPosition.y) &&
                     Possible((int)newPosition.x, (int)newPosition.y);

        // Only save the movement if the new position is valid
        if (valid)
        {
            position = newPosition;
        }

        return valid;
    }
}

/*bool CanMove()
{
    
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













