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
    [SerializeField] private List<Block> blocks;



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
        
        var freeNodes = nodes.Where(n => n.OccupiedBlock = null).OrderBy(b => Random.value);
        
        var Iv1 = new Vector2(-1, -0);
        var Iv2 = new Vector2(0, 0);
        var Iv3 = new Vector2(1, 0);

        List<Vector2> Iv = new List<Vector2>();

        Iv.Add(Iv1);
        Iv.Add(Iv2);
        Iv.Add(Iv3);

       
            for (int i = 0; i < Iv.Count; i++)
            {
                var block = Instantiate(blockPrefab, Iv[i], quaternion.identity);
                blocks.Add(block);
            }
        


        if (freeNodes.Count() == 2)
        {
            //Game Over;
            return;
        }
    }

}