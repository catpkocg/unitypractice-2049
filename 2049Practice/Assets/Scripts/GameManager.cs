using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private TetrisBlock blockPrefab;

    [SerializeField]
    private GameObject[] TetrisBlcoks;

    [SerializeField]
    private List<Node> nodes;
    [SerializeField]
    private List<Block> blocks;
    //[SerializeField]
    //private Dictionary<Vector2, Node> Positions;
    [SerializeField]
    private GameObject gridCellPrefab;
    private GameObject[,] gameGrid;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        
        SpawnBlocks();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGrid()
    {
        nodes = new List<Node>();
        blocks = new List<Block>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                nodes.Add(node);
            }
        }
        
        
        var center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);
    }

    void SpawnBlocks()
    {
        var position = new Dictionary<Vector2,Node>();
        for (int i = 1; i < nodes.Count; i++)
        {
            position.Add(nodes[i].Pos,nodes[i]);
        }
        
        int randomX = Random.Range(0, 8);
        int randomY = Random.Range(0, 8);
        int randomBlcok = Random.Range(0, TetrisBlcoks.Length);
        Instantiate(TetrisBlcoks[randomBlcok], new Vector2(randomX, randomY),
            Quaternion.identity);
        if(nodes)
        if (randomBlcok == 0)
        {
            var block1 = Instantiate(TetrisBlcoks[Random.Range(0, TetrisBlcoks.Length)], new Vector2(randomX-1, randomY),
                Quaternion.identity);
            var block2 = Instantiate(TetrisBlcoks[Random.Range(0, TetrisBlcoks.Length)], new Vector2(randomX+1, randomY),
                Quaternion.identity);
        } else if (randomBlcok == 1)
        {
            var block1 = Instantiate(TetrisBlcoks[Random.Range(0, TetrisBlcoks.Length)], new Vector2(randomX, randomY-1),
                Quaternion.identity);
            var block2 = Instantiate(TetrisBlcoks[Random.Range(0, TetrisBlcoks.Length)], new Vector2(randomX, randomY+1),
                Quaternion.identity);
        }
    }
}
