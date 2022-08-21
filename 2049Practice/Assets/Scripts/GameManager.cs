using System.Collections;
using System.Collections.Generic;
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

    public List<Vector3> Good = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Blocks[0], new Vector3(3, 3, 0), Quaternion.identity);
        Array[3, 3] = 1;
        Array[2, 3] = 1;
        Array[4, 3] = 1;
        //초기 array 값은 모두 0;
        // block 위치대로 array 값 1 주기.

        //블락별로 index 다르게 주기
        //점수에 영향

        GenerateGrid();
        Debug.Log(Nodes[1].Pos);
        Debug.Log(Nodes[2].Pos);
        Debug.Log(Nodes[64].Pos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int random = Random.Range(0, Blocks.Length);
            var RandomBlock = Blocks[random].GetComponent<Block>().ShapePos;
            
            if (CanSpawn(RandomBlock))
            {
                SpawnBlock();
                
            }
            else
            {
                GameOver();
            }

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
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);



    }

    void GameOver()
    {
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

    //랜덤 블락 고른후에 대입. 
    bool CanSpawn(Vector3[] ShapePos)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = 0;
                for (int k = 0; k < ShapePos.Length; k++)
                {
                    Vector3 CurShapePos = ShapePos[k] + new Vector3(i, j, 0);
                    if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    ++count;
                }

                if (count == ShapePos.Length)
                {
                    return true;
                }
            }
        }

        return false;
    }


    /*
    bool IsValid(Vector3[] ShapePos)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = 0;
                for (int k = 0; k < ShapePos.Length; k++)
                {
                    Vector3 CurShapePos = ShapePos[k] + new Vector3(i,j,0);
                    if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    ++count;
                }

                if (count == ShapePos.Length)
                {
                    return true;
                }
            }
        }
        return false;
    }
    */

    void SpawnBlock()
    {
        int random = Random.Range(0, Blocks.Length);
        var RandomBlock = Blocks[random].GetComponent<Block>().ShapePos;
        for (int x = 0; x < width; x++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 vaildPos = new Vector3(x, j, 0);

                if (!IsValidPosition(RandomBlock,vaildPos))
                {
                    Instantiate(Blocks[random], vaildPos, Quaternion.identity);
                }

                for (int i = 0; i < RandomBlock.Length; i++)
                {
                    Vector3 Save = RandomBlock[i] + vaildPos;
                    Array[(int)Save.x, (int)Save.y] = 1;
                }
            }
        }

    }

    public bool IsValidPosition(Vector3[] ShapePos)
    {

        // The position is only valid if every cell is valid
        for (int i = 0; i < ShapePos.Length; i++)
        {
            Vector3 Position = ShapePos[i]

            if (!InRange((int)Position.x, (int)Position.y))
            {
                return false;
            }
            
            if (!Possible((int)Position.x, (int)Position.y))
            {
                return false;
            }
        }

        return true;

    }

    void FindValidPlace()
    {
        int random = Random.Range(0, Blocks.Length);
        var RandomBlock = Blocks[random].GetComponent<Block>().ShapePos;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 place = new Vector3(x, y, 0);
                List<Vector3> validPlaces = new List<Vector3>();
                
                
            }
        }
    }
}






