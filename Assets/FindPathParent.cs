using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BlockType
{
    DontMove = 0,  //(장애물)
    Walkable = 1,  //걸을 수 있음.
    Card1 = 2,
    Card2 = 3,
    Card3 = 4,
    Card4 = 5,
    Card5 = 6,
    Card6 = 7,
    Card7 = 8,
    Card8 = 9,
    Card9 = 10,
    Card10 = 11,
    Card11 = 12,
    Card12 = 13,
    Card13 = 14,
    Card14 = 15,
    Card15 = 16,
    Card16 = 17,
}
public class FindPathParent : MonoBehaviour
{
    static public FindPathParent Instance;


    bool ingSearchCo; //서치 함수가 실행중인 동안 true.

    LineRenderer line;

    [System.Serializable]
    public class CardList
    {
        public List<BlockType> items = new List<BlockType>();
        public List<BlockInfo> blockInfos = new List<BlockInfo>();
        public override string ToString()
        {
            return items.Select(x => x.ToString()).Aggregate((current, next) => current + ", " + next);
        }
    }

    public List<CardList> mapList;
    public Dictionary<Vector2Int, BlockInfo> map = new Dictionary<Vector2Int, BlockInfo>();
    void Awake()
    {
        Instance = this;
        GetMapInfo();
        line = GetComponent<LineRenderer>();
    }
    public TextMesh textMesh;
    virtual protected void GetMapInfo()
    {
        List<BlockInfo> allBlocks = new List<BlockInfo>(GetComponentsInChildren<BlockInfo>());

        GroundExtention.scaleX = allBlocks[0].transform.localScale.x;
        GroundExtention.scaleY = allBlocks[0].transform.localScale.y;

        int maxCountX = allBlocks.OrderBy(x => x.transform.localPosition.x).Last().transform.localPosition.ToVector2Int().x + 1;
        var yLast = allBlocks.OrderBy(x => x.transform.localPosition.y).Last();
        var yLastPos = yLast.transform.localPosition;
        var yLastPosVector = yLastPos.ToVector2Int();
        int maxCountY = allBlocks.OrderBy(x => x.transform.localPosition.y).Last().transform.localPosition.ToVector2Int().y + 1;
        mapList = new List<CardList>(maxCountX);
        for (int i = 0; i < maxCountX; i++)
        {
            mapList.Add(new CardList());
            for (int y = 0; y < maxCountY; y++)
            {
                mapList[i].items.Add(0);
                mapList[i].blockInfos.Add(null);
            }
        }
        foreach (var item in allBlocks)
        {
            item.name = item.transform.localPosition.ToVector2Int().ToString();
            var newTextMesh = Instantiate(textMesh, item.transform);
            newTextMesh.text = item.name;
            newTextMesh.transform.localPosition = new Vector3(0, 0, -0.1f);
            var pos = item.transform.localPosition.ToVector2Int();
            mapList[pos.x].items[pos.y] = item.blockType;
            mapList[pos.x].blockInfos[pos.y] = item;
            map[pos] = item;
        }
    }

    public HashSet<BlockInfo> selectedCard = new HashSet<BlockInfo>();

    public void FindPath(BlockInfo blockInfo)
    {
        if (ingSearchCo)
        {
            print("이전 계산 진행중");
            return;
        }

        if (selectedCard.Count > 0)
        {
            if (selectedCard.First().blockType != blockInfo.blockType)
                selectedCard.Clear();
        }


        selectedCard.Add(blockInfo);

        if (selectedCard.Count == 2)
        {
            Vector2Int find = selectedCard.Where(x => x != blockInfo)
                .Select(x => x.transform.localPosition.ToVector2Int())
                .First();
            StartCoroutine(FindPathCo(blockInfo, find));
            selectedCard.Clear();
        }
    }
    virtual protected IEnumerator SearchCo(Pos pos, Vector2Int find, List<CardList> map, Pos result)
    {
        throw new NotImplementedException();
    }


    IEnumerator FindPathCo(BlockInfo blockInfo, Vector2Int find)
    {
        var pos = blockInfo.transform.localPosition.ToVector2Int();
        Pos result = new Pos();

        ClearParent(mapList);
        ingSearchCo = true;
        yield return StartCoroutine(SearchCo(new Pos() { x = pos.x, y = pos.y }, find, mapList, result)); ;
        ingSearchCo = false;
        print(result);

        DrawPath(result);
    }


    private void ClearParent(List<CardList> map)
    {
        foreach (var line in map)
        {
            foreach (var item in line.blockInfos)
            {
                item.parent = null;
            }
        }
    }

    private void DrawPath(Pos result)
    {
        if (result.x == -1) // 그릴께 없음
        {
            line.positionCount = 0;
            return;
        }

        List<Vector3> posList = new List<Vector3>();
        BlockInfo block = mapList[result.x].blockInfos[result.y]; // 도착 하는 지점.
        while (block != null)
        {
            posList.Add(block.transform.position + new Vector3(0, 0, -1f));
            block = block.parent;
        }
        line.positionCount = posList.Count;
        line.SetPositions(posList.ToArray());
    }

    public class Pos
    {
        public int x; // 행
        public int y; // 열
        public Direction dir; // ⭐방향⭐ (현재 향하고 있는 방향을 알아서 다음 위치를 큐에 삽입할 때 꺾어야할지를 알 수 있다.) 
        public override string ToString()
        {
            return $"x:{x}, y:{y}, {dir}";
        }
    }

    public float simulateSpeed = 0.3f;
}

public enum Direction
{
    None = -1,
    Left, Right, Down, Up
        , Last
}
static public class GroundExtention
{
    static public float scaleX = 1;
    static public float scaleY = 1;
    static public Vector2Int ToVector2Int(this Vector3 v3)
    {
        return new Vector2Int(Mathf.RoundToInt(v3.x / scaleX)
            , Mathf.RoundToInt(v3.y / scaleY));
    }
}
