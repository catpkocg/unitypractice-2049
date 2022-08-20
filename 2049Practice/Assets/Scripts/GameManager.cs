using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int width = 8;
    [SerializeField]
    private int height = 8;
    [SerializeField]
    private Node nodePrefab;
    
    public GameObject[] Blocks;
    public List<Node> Nodes;
    public int[,] Array = new int [8, 8];
    public Transform[] BlockPos;
    public event System.Action EditorRepaint = () => { };



    private Vector2[] BlockShape;
    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Blocks[0], new Vector3(3, 3, 0), Quaternion.identity);
        //초기 array 값은 모두 0;
        // block 위치대로 array 값 1 주기.
        GenerateGrid();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                Nodes.Add(node);
                
            }
        }

        var center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

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
    bool ThisBlockCanSpawn(Vector3[] ShapePos)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = 0;
                for (int k = 0; k < ShapePos.Length; k++)
                {
                    Vector3 CurShapePos = ShapePos[i] + new Vector3(i, j, 0);
                    if(!InRange((int)CurShapePos.x,(int)CurShapePos.y)) break;
                    if(!Possible((int)CurShapePos.x,(int)CurShapePos.y)) break;
                    ++count;
                }

                if (count == ShapePos.Length) return true;
            }
        }

        return false;
    }
    
    
    
    
    
    
    
}
