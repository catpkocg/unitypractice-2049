using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Sequence = DG.Tweening.Sequence;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class GameManager : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private Node nodePrefab;

    public Block[] BlockPrefabs;
    public Block[] OneBlock;
    public List<Node> Nodes;
    public int[,] Array = new int [8, 8];
    public event System.Action EditorRepaint = () => { };

    public GameObject BASE;

    private Vector3 Pos;
    private Vector2[] BlockShape;
    public List<Block> MoveBlocks;
    public int LineNums = 0;
    public List<Vector3> Good = new List<Vector3>();
    
    public bool NeedMove = true;
    void Start()
    {
        GenerateGrid();
        StartCoroutine(SpawnBlock());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnBlock());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(Right());
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(Left());
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(Up());
        }else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(Down());
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

        Camera.main.transform.position = new Vector3(center.x, center.y, -20);
    }

    void GameOver()
    {
        foreach (Block block in MoveBlocks)
        {
            Destroy(block);
        }

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

    bool CanSpawn(List<Vector3> RandomBlock)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = 0;
                for (int k = 0; k < RandomBlock.Count; k++)
                {
                    Vector3 CurShapePos = RandomBlock[k] + new Vector3(i, j, 0);
                    if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                    ++count;
                }

                if (count == RandomBlock.Count)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    IEnumerator SpawnBlock()
    {
        var random = Random.Range(0, BlockPrefabs.Length);
        var randomX = Random.Range(0, 7);
        var randomY = Random.Range(0, 7);
        var randomPos = new Vector3(randomX, randomY, 0);

        var RandomBlock = BlockPrefabs[random].ShapePos;

        if (CanSpawn(RandomBlock) == false)
        {
            GameOver();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Array[x, y] = 1;
                }
            }
        }

        else
        {
            List<Vector3> validPlaces = new List<Vector3>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int count = 0;

                    for (int k = 0; k < RandomBlock.Count; k++)
                    {
                        Vector3 CurShapePos = RandomBlock[k] + new Vector3(i, j, 0);
                        if (!InRange((int)CurShapePos.x, (int)CurShapePos.y)) break;
                        if (!Possible((int)CurShapePos.x, (int)CurShapePos.y)) break;
                        ++count;

                    }

                    if (count == RandomBlock.Count)
                    {
                        validPlaces.Add(new Vector3(i, j, 0));
                    }

                }
            }

            int randomSpawn = Random.Range(0, validPlaces.Count);

            var SpawnPos = validPlaces[randomSpawn];
            
            var moveBlock = Instantiate(BlockPrefabs[random], SpawnPos, Quaternion.identity);
            MoveBlocks.Add(moveBlock);

            for (int k = 0; k < RandomBlock.Count; k++)
            {
                Vector3 getposition = RandomBlock[k] + SpawnPos;
                Array[(int)getposition.x, (int)getposition.y] = 1;
            }
        }

        yield return null;
    }
    
    IEnumerator MoveUp()
    {
        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.y).ToList();
        
        List<Vector3> goPos = new List<Vector3>();
        
        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].ShapePos;

            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.y;
            Vector3 goodPos = presentPos;
            for (int i = current; i < height; i++)
            {
                var position = new Vector3(presentPos.x, i, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.1f).SetEase(Ease.OutCubic);
            

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
        yield return null;
    }
    IEnumerator MoveDown()
    {
        
        var blockMove = MoveBlocks.OrderBy(b => b.transform.position.y).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var nn = blockMove[0].ShapePos;

            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.y;
            Vector3 goodPos = presentPos;
            for (int i = current; i >= 0; i--)
            {
                var position = new Vector3(presentPos.x, i, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.1f).SetEase(Ease.OutCubic);

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
        yield return null;
    }
    IEnumerator MoveLeft()
    {

        var blockMove = MoveBlocks.OrderBy(b => b.transform.position.x).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;

            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.x;
            Vector3 goodPos = presentPos;
            for (int i = current; i >= 0; i--)
            {
                var position = new Vector3(i, presentPos.y, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);
            
            blockMove[k].transform.DOMove(goPos[k], 0.1f).SetEase(Ease.OutCubic);
            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }
        yield return null;
    }
    
    IEnumerator MoveRight()
    {
        
        var blockMove = MoveBlocks.OrderByDescending(b => b.transform.position.x).ToList();

        List<Vector3> goPos = new List<Vector3>();

        for (int k = 0; k < blockMove.Count; k++)
        {
            var presentPos = blockMove[k].transform.position;
            
            var Arraychange = blockMove[k].ShapePos;

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)blockMove[k].transform.position.x + (int)Arraychange[n].x,
                    (int)blockMove[k].transform.position.y + (int)Arraychange[n].y] = 0;
            }

            int current = (int)presentPos.x;
            Vector3 goodPos = presentPos;
            for (int i = current; i < width; i++)
            {
                var position = new Vector3(i, presentPos.y, 0);
                if (IsValidPosition(blockMove[k], position))
                {
                    goodPos = position;
                }
                else
                {
                    break;    
                }
            
            } goPos.Add(goodPos);

            blockMove[k].transform.DOMove(goPos[k], 0.1f).SetEase(Ease.OutCubic);

            for (int n = 0; n < Arraychange.Count; n++)
            {
                Array[(int)goPos[k].x + (int)Arraychange[n].x,
                    (int)goPos[k].y + (int)Arraychange[n].y] = 1;
            }
        }

        yield return null;
    }

    IEnumerator CheckLineV()
    {
        int oneLine = 0;
        for (int i= 0; i < width; i++)
        {
            int verticalCount = 0;
            for (int j = 0; j < height; j++) 
            {
                if (Array[i, j] != 0)
                {
                    ++verticalCount;
                }
            }
            if (verticalCount == 8)
            {
                ++oneLine;
                for (int j = 0; j < height; j++)
                {
                    Array[i, j] = -1;
                }
            }
            LineNums = oneLine;
        }
        yield return null;
    }
    
    IEnumerator CheckLineH()
    {
        int oneLine = 0;
        for (int i= 0; i < height; i++)
        {
            int HorizonCount = 0;
            for (int j = 0; j < width; j++) 
            {
                if (Array[j, i] != 0)
                {
                    ++HorizonCount;
                }
            }
            if (HorizonCount == 8)
            {
                ++oneLine;
                for (int j = 0; j < width; j++)
                {
                    Array[j, i] = -1;
                }
            }
            LineNums = oneLine;
        }
        yield return null;
    }
    

    IEnumerator LineClear()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Array[i, j] == -1)
                {
                    Array[i, j] = 0;
                    
                    for (int k = 0; k < MoveBlocks.Count; k++)
                    {
                        for (int n = MoveBlocks[k].transform.childCount -1; n >= 0; n--)
                        {
                            
                            GameObject child = MoveBlocks[k].transform.GetChild(n).gameObject;
                            
                            if (child.transform.position == new Vector3(i, j, 0))
                            {
                                Destroy(child);
                                MoveBlocks[k].ShapePos.Remove(new Vector3(i, j, 0)-MoveBlocks[k].Pos);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }
    
    

    IEnumerator Organize()
    {
        for (int n = 0; n < MoveBlocks.Count; n++)
        {
            if (MoveBlocks[n].transform.childCount == 0)
            {
                Debug.Log("뭐지1");
                Destroy(MoveBlocks[n].gameObject);
                MoveBlocks.Remove(MoveBlocks[n]);
                
            }
            
            else if(MoveBlocks[n].transform.childCount == 2 &&
                    MoveBlocks[n].ShapePos[0] != new Vector3(0, 0, 0))
            {
                Debug.Log("뭐지2");
                var one = Instantiate(OneBlock[0], MoveBlocks[n].ShapePos[0]+MoveBlocks[n].Pos, Quaternion.identity);
                var two = Instantiate(OneBlock[0], MoveBlocks[n].ShapePos[1]+MoveBlocks[n].Pos, Quaternion.identity);
                
                Destroy(MoveBlocks[n].gameObject);
                
                MoveBlocks.Remove(MoveBlocks[n]);
                
                MoveBlocks.Add(one);
                MoveBlocks.Add(two);
            }
        }
        yield return null;
    }

    IEnumerator Right()
    {
        NeedMove = true;
        yield return StartCoroutine(MoveRight());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveRight());
        yield return new WaitForSeconds(0.2f);
        while(NeedMove)
        {
            yield return StartCoroutine(CheckLineV());
            yield return new WaitForSeconds(0.01f);
            if (LineNums != 0)
            {
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.01f);
                yield return StartCoroutine(Organize());
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MoveRight());
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(MoveRight());
                yield return null;
            }else if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return null;
        yield return StartCoroutine(CheckLineH());
        yield return null;
        yield return StartCoroutine(LineClear());
        yield return null;
        yield return StartCoroutine(Organize());
    }

    
    IEnumerator Left()
    {
        NeedMove = true;
        yield return StartCoroutine(MoveLeft());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveLeft());
        yield return new WaitForSeconds(0.2f);
        while(NeedMove)
        {
            yield return StartCoroutine(CheckLineV());
            yield return new WaitForSeconds(0.01f);
            if (LineNums != 0)
            {
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.01f);
                yield return StartCoroutine(Organize());
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MoveLeft());
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(MoveLeft());
                yield return null;
            }else if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return null;
        yield return StartCoroutine(CheckLineH());
        yield return null;
        yield return StartCoroutine(LineClear());
        yield return null;
        yield return StartCoroutine(Organize());

    }
    IEnumerator Down()
    {
        NeedMove = true;
        yield return StartCoroutine(MoveDown());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveDown());
        yield return new WaitForSeconds(0.2f);
        while(NeedMove)
        {
            yield return StartCoroutine(CheckLineH());
            yield return new WaitForSeconds(0.01f);
            if (LineNums != 0)
            {
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.01f);
                yield return StartCoroutine(Organize());
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MoveDown());
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(MoveDown());
                yield return null;
            }else if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return null;
        yield return StartCoroutine(CheckLineV());
        yield return null;
        yield return StartCoroutine(LineClear());
        yield return null;
        yield return StartCoroutine(Organize());

    }
    IEnumerator Up()
    {
        NeedMove = true;
        yield return StartCoroutine(MoveUp());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveUp());
        yield return new WaitForSeconds(0.2f);
        while(NeedMove)
        {
            yield return StartCoroutine(CheckLineH());
            yield return new WaitForSeconds(0.01f);
            if (LineNums != 0)
            {
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.01f);
                yield return StartCoroutine(Organize());
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MoveUp());        
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(MoveUp());
                yield return null;
            }else if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return null;
        yield return StartCoroutine(CheckLineV());
        yield return null;
        yield return StartCoroutine(LineClear());
        yield return null;
        yield return StartCoroutine(Organize());

    }
    
    
    
    public bool IsValidPosition(Block block, Vector3 position)
    {
        var Block = block.ShapePos;
        
        // The position is only valid if every cell is valid
        for (int i = 0; i < Block.Count; i++)
        {
            Vector3 nowPosition = Block[i] + position;

            // An out of bounds tile is invalid
            if (!InRange((int)nowPosition.x, (int)nowPosition.y))
            {
                return false;
            }

            if (!Possible((int)nowPosition.x, (int)nowPosition.y)) {
                return false;
            }
        }

        return true;
    }

    
}






