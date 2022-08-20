using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public Vector3[] ShapePos;
    GameManager GM;
    private Vector3 BlockPos;
    // Start is called before the first frame update
    void Start()
    {
        BlockPos = transform.position;
        GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        ShapePos = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            ShapePos[i] = transform.GetChild(i).localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveR()
    {
        
    }

    void MoveL()
    {
        
    }

    void MoveU()
    {
        
    }

    void MoveD()
    {
        
    }
}
