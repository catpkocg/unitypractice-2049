using System;
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
    [SerializeField] private Block blockPrefab;


    private List<Node> nodes;
    private List<Block> blocks;

    private List<Block> OqNodes;




    void Start()
    {
        GenerateGrid();

        SpawnBlocks();

    }

    void GenerateGrid()
    {
        nodes = new List<Node>();
        blocks = new List<Block>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), quaternion.identity);
                nodes.Add(node);
            }
        }

        var center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);


        Camera.main.transform.position = new Vector3(center.x, center.y, -10);
    }

    void SpawnBlocks()
    {
        
        var freeNodes = nodes.Where(n => n.OccupiedBlock == false).OrderBy(b => Random.value).ToList();
 
        
        var Iv1 = new Vector2(-1, -0);
        var Iv2 = new Vector2(0, 0);
        var Iv3 = new Vector2(1, 0);

        List<Vector2> Iv = new List<Vector2>();

        Iv.Add(Iv1);
        Iv.Add(Iv2);
        Iv.Add(Iv3);
        
        List<Node> OqNodes = new List<Node>();
        for (int i = 0; i < Iv.Count; i++)
        {
            var block = Instantiate(blockPrefab, freeNodes[1].Pos + Iv[i], Quaternion.identity);
            List<Node> OqNodes = new List<Node>();


        }
        


    }

    public void asdf()
    {
        var adsf = new Vector2(0, 0);
        
    }
}
    
    