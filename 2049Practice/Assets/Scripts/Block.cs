using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;

    public List<Vector3> ShapePos;
    GameManager GM;
    public Vector3 Pos => transform.position;
    public Vector3 BlockPos;

    // Start is called before the first frame update
    void Start()
    {
        BlockPos = transform.position;
        GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        ShapePos = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++)
        {
            ShapePos.Add(transform.GetChild(i).localPosition);
        }


    }

}