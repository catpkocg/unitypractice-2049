using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int width = 8;
    [SerializeField]
    private int height = 8;
    [SerializeField]
    private Node nodePrefab;
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        Instantiate(nodePrefab, new Vector3(0, 0, 0), quaternion.identity);
    }
    
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y > height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), quaternion.identity);
            }
        }
    }
}
