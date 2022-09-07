using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;



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

    public bool NeedMove;
    public bool CanMove;

    public Text score;
    public int LineScore;

    private float currentTime;
    public int startSecond;
    public Text currentTimeText;

    public GameObject overPrefab;
    

    void Start()
    {
        GenerateGrid();
        StartCoroutine(SpawnBlock());
        NeedMove = false;
        currentTime = 10f;
    }
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime = currentTime - Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            String a = time.Seconds.ToString();
            String b = (time.Milliseconds/10).ToString();
            String Timetext = a + "\n" + b;
            currentTimeText.text = Timetext;

        }
        else if (currentTime <= 0)
        {
            GameOver();
            currentTimeText.text = "Game Over";
            return;

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnBlock());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!NeedMove)
            {
                StartCoroutine(Right());
            }
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!NeedMove)
            {
                StartCoroutine(Left());
            }
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!NeedMove)
            {
                StartCoroutine(Up());
            }
        }else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!NeedMove)
            {
                StartCoroutine(Down());
            }
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
            Destroy(block.gameObject);
            MoveBlocks.Remove(block);
        }
        Debug.Log("Game Over");
        
        StopAllCoroutines();
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        
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
        
        
            Sequence S = DOTween.Sequence();
        
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
            
                //S.Join(blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true));
            

                for (int n = 0; n < Arraychange.Count; n++)
                {
                    Array[(int)goPos[k].x + (int)Arraychange[n].x,
                        (int)goPos[k].y + (int)Arraychange[n].y] = 1;
                }
                IEnumerator Do(int k)
                {
                    blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true);
                    yield return null;
                }

                StartCoroutine(Do(k));
            }
            
        
        yield return null;
    }
    IEnumerator MoveDown()
    {
        
            Sequence S = DOTween.Sequence();
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
            
                //S.Join(blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true));

                for (int n = 0; n < Arraychange.Count; n++)
                {
                    Array[(int)goPos[k].x + (int)Arraychange[n].x,
                        (int)goPos[k].y + (int)Arraychange[n].y] = 1;
                }
                
                IEnumerator Do(int k)
                {
                    blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true);
                    yield return null;
                }

                StartCoroutine(Do(k));
            }

        
        yield return null;
    }
    IEnumerator MoveLeft()
    {
        
            Sequence S = DOTween.Sequence();
        
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
            
                //S.Join(blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true));
            
                for (int n = 0; n < Arraychange.Count; n++)
                {
                    Array[(int)goPos[k].x + (int)Arraychange[n].x,
                        (int)goPos[k].y + (int)Arraychange[n].y] = 1;
                }
                
                IEnumerator Do(int k)
                {
                    blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true);
                    yield return null;
                }

                StartCoroutine(Do(k));
            }

        
        yield return null;
    }
    
    IEnumerator MoveRight()
    {
        
            Sequence S = DOTween.Sequence();

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

                //S.Join(blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true));

                for (int n = 0; n < Arraychange.Count; n++)
                {
                    Array[(int)goPos[k].x + (int)Arraychange[n].x,
                        (int)goPos[k].y + (int)Arraychange[n].y] = 1;
                }
                IEnumerator Do(int k)
                {
                    blockMove[k].transform.DOMove(goPos[k], 0.1f).SetAutoKill(true);
                    yield return null;
                }

                StartCoroutine(Do(k));
            }

            yield return null;
    }
    
    IEnumerator Do(int i) {
        
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
        Debug.Log(oneLine);
        
        yield return null;
    }

    public void Updatescore(int LineScore)
    {
        score.text = "Score : " + LineScore.ToString();
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
                                LineScore += 1;
                                MoveBlocks[k].ShapePos.Remove(new Vector3(i, j, 0)-MoveBlocks[k].Pos);
                            }
                        }
                    }
                }
            }
        }
        
        Updatescore(LineScore);
        
        yield return null;
    }
    
    

    IEnumerator Organize()
    {
        for (int n = 0; n < MoveBlocks.Count; n++)
        {
            if (MoveBlocks[n].transform.childCount == 0)
            {
                
                Destroy(MoveBlocks[n].gameObject);
                MoveBlocks.Remove(MoveBlocks[n]);
                
            }

            else if (MoveBlocks[n].transform.childCount == 1)
            {
                var one = Instantiate(OneBlock[0], MoveBlocks[n].ShapePos[0]+MoveBlocks[n].Pos, Quaternion.identity);
                
                Destroy(MoveBlocks[n].gameObject);

                MoveBlocks.Remove((MoveBlocks[n]));
                
                MoveBlocks.Add(one);
            }
            
            else if(MoveBlocks[n].transform.childCount == 2 &&
                    MoveBlocks[n].ShapePos[0] != new Vector3(0, 0, 0))
            {
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
        yield return new WaitForEndOfFrame();
        
        yield return StartCoroutine(CheckLineV());
        yield return new WaitForEndOfFrame();

        while (NeedMove)
        {
            
            if(LineNums != 0)
            {
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.2f);
                Debug.Log("뭐지1");
                StartCoroutine(Organize());
                yield return StartCoroutine(Organize());
                yield return new WaitForEndOfFrame();
                Debug.Log("뭐지2");
                yield return StartCoroutine(MoveRight());
                yield return new WaitForEndOfFrame();
                Debug.Log("뭐지3");
                yield return StartCoroutine(CheckLineV());
                yield return new WaitForEndOfFrame();
                Debug.Log("뭐지4");
            }
            
            if (LineNums == 0)
            {
                NeedMove = false;
                yield return null;
                StopCoroutine(MoveRight());
            }
            
        }
        
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return new WaitForEndOfFrame();
        Debug.Log("뭐지5");
        yield return StartCoroutine(CheckLineH());
        yield return new WaitForEndOfFrame();
        Debug.Log("뭐지6");
        yield return StartCoroutine(LineClear());
        yield return new WaitForEndOfFrame();
        Debug.Log("뭐지7");
        yield return StartCoroutine(Organize());
        yield return new WaitForEndOfFrame();
        Debug.Log("뭐지8");
        NeedMove = false;


    }

    
    IEnumerator Left()
        {
        NeedMove = true;
        yield return StartCoroutine(MoveLeft());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CheckLineV());
        yield return new WaitForEndOfFrame();
        
        while(NeedMove)
        {
            if (LineNums != 0)
            {
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(Organize());
                yield return StartCoroutine(Organize());
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(MoveLeft());
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(CheckLineV());
                yield return new WaitForEndOfFrame();
                
            }
            if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
                StopCoroutine(MoveLeft());
            }
            
        }
        
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CheckLineH());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(LineClear());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Organize());
        NeedMove = false;

        }
    IEnumerator Down(){
        
        NeedMove = true;
        yield return StartCoroutine(MoveDown());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CheckLineH());
        yield return new WaitForEndOfFrame();
        while(NeedMove)
        {
            if (LineNums != 0)
            {
                
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(Organize());
                yield return StartCoroutine(Organize());
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(MoveDown());
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(CheckLineH());
                yield return new WaitForEndOfFrame();
                
            }else if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
                StopCoroutine(MoveDown());
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CheckLineV());
        yield return new WaitForEndOfFrame();
        
        yield return StartCoroutine(LineClear());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Organize());
        yield return new WaitForEndOfFrame();
        NeedMove = false;
        
    }
    IEnumerator Up()
    {
        NeedMove = true;
        yield return StartCoroutine(MoveUp());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CheckLineH());
        yield return new WaitForEndOfFrame();
        while(NeedMove)
        {
            if (LineNums != 0)
            {
                
                yield return StartCoroutine(LineClear());
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(Organize());
                yield return StartCoroutine(Organize());
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(MoveUp());        
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(CheckLineH());
                yield return new WaitForEndOfFrame();
                
            }else if (LineNums == 0)
            {
                yield return null;
                NeedMove = false;
                StopCoroutine(MoveUp());  
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnBlock());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(CheckLineV());
        yield return new WaitForEndOfFrame();
        
        yield return StartCoroutine(LineClear());
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Organize());
        yield return new WaitForEndOfFrame();
        NeedMove = false;

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






