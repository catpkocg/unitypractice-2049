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
    
    // 0 0 0 0 0 0 0  0
    // 0 0 0 0 9 9 0  0
    // 0 0 0 0 9 0 0  0
    // 0 0 0 0 0 0 0  0
    // 0 0 0 0 0 0 0  0
    
    //블락에 그리드추가. 이동할수있을때까지 이동. (바운드, 블락 그리드 만나면 스탑.) (all block 반복)
    
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
