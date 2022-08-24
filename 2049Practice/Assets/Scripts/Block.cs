using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;

    public Vector3[] ShapePos;
    GameManager GM;
    public Vector3 Pos => transform.position;
    public Vector3 BlockPos;

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


/*    void MoveR()
    {
        for(int x = 7; x < 0; x--)
        {
            for(int y = 0; y < height; y++)
            {
                if()

                for()
            }
        }
    }

    void canMove()
    {

    }*/
}