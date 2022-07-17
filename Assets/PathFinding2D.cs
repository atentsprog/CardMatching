using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PathFinding2D : FindPathParent
{
    /**
     * find a path in grid tilemaps
     */
    override protected IEnumerator SearchCo(Pos start, Vector2Int endPos, List<CardList> board, Pos result)
    {
        Vector2Int startPos = new Vector2Int(start.x, start.y);
        List<Vector2Int> resultPos = new List<Vector2Int>();
        yield return StartCoroutine(Astar(startPos, endPos, map, resultPos));
        if (resultPos.Count == 0) 
        { 
            print($"{startPos}->{endPos} 갈 수 없는 길입니다");
        }
    }
    public float dirScoreWeight = 0.9f;
    float CalculateHScore(Vector2Int a, Vector2Int b, Direction previousDir, Direction currentDir)
    {
        float dirScore = previousDir == currentDir ? 0: dirScoreWeight;
        float distanceScore = GetDistanceScore(a, b);
        float resultScore = distanceScore + dirScore;
        return resultScore;
    }
    static float GetDistanceScore(Vector2Int a, Vector2Int b)
    {
        float xDistance = Mathf.Abs(a.x - b.x);
        float yDistance = Mathf.Abs(a.y - b.y);
        //return xDistance * xDistance + yDistance * yDistance;
        return xDistance + yDistance;
    }

    struct Vector2IntDir
    {
        public Vector2Int pos;
        public Direction dir;

        public Vector2IntDir(int x, int y, Direction dir)
        {
            pos = new Vector2Int(x, y);
            this.dir = dir;

        }
    }
    static List<Vector2IntDir> GetNeighbors(Vector2Int pos)
    {
        var neighbors = new List<Vector2IntDir>();
        neighbors.Add(new Vector2IntDir(pos.x, pos.y + 1, Direction.Up));
        neighbors.Add(new Vector2IntDir(pos.x, pos.y - 1, Direction.Down));
        neighbors.Add(new Vector2IntDir(pos.x + 1, pos.y, Direction.Right));
        neighbors.Add(new Vector2IntDir(pos.x - 1, pos.y, Direction.Left));
        return neighbors;
    }


    public Node finalNode;
    public bool resultFindDest = false;
    IEnumerator Astar(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, BlockInfo> map, List<Vector2Int> result)
    {
        if (from == to)
        {
            result.Add(from);
            yield break;
        }
        List<Node> open = new List<Node>();

        yield return StartCoroutine(FindDest(Direction.None, new Node(Direction.None, map[from], null, from, CalculateHScore(from, to, Direction.None, Direction.None), 0), open, map, to));
       
        if (resultFindDest)
        {
            while (finalNode != null)
            {
                if(finalNode.preNode != null)
                    map[finalNode.preNode.pos].parent = map[finalNode.pos];
                result.Add(finalNode.pos);
                finalNode = finalNode.preNode;
            } 
        }
        result.Reverse();
        yield break;
    }

    IEnumerator FindDest(Direction dir, Node currentNode, List<Node> openList,
                         Dictionary<Vector2Int, BlockInfo> map, Vector2Int to)
    {
        if (currentNode == null) {
            finalNode = null;
            resultFindDest = false;
            yield break;
        }
        else if (currentNode.pos == to)
        {
            finalNode = currentNode;
            resultFindDest = true;
            yield break;
        }
        currentNode.tr.GetComponent<BlockInfo>().SetActiveState();

        if (simulateSpeed > 0)
            yield return new WaitForSeconds(simulateSpeed);

        currentNode.open = false;
        openList.Add(currentNode);

        BlockType desType = map[to].blockType;
        foreach (var item in GetNeighbors(currentNode.pos))
        {
            if (map.ContainsKey(item.pos) && (desType == map[item.pos].blockType || map[item.pos].blockType == BlockType.Walkable))
            {
                FindTemp(map, dir, openList, currentNode, item, to);
            }
        }
        var next = openList.FindAll(obj => obj.open).Min();
        yield return StartCoroutine(FindDest(dir, next, openList, map, to));
    }

    void FindTemp(Dictionary<Vector2Int, BlockInfo> map, Direction dir, List<Node> openList, Node currentNode, Vector2IntDir nextNode, Vector2Int to)
    {
        Vector2Int from = nextNode.pos;
        Node temp = openList.Find(obj => obj.pos == (from));
        if (temp == null)
        {
            Vector2Int previousNodePos = currentNode.pos;
            Vector2Int currentNodePos = from;
            Direction previousDir = currentNode.dir;
            Direction currentDir = nextNode.dir;
           temp = new Node(currentDir, map[from], currentNode, from, CalculateHScore(from, to, previousDir, currentDir), currentNode.gScore + 1);
            openList.Add(temp);
        }
        else if (temp.open && temp.gScore > currentNode.gScore + 1)
        {
            temp.gScore = currentNode.gScore + 1;
            temp.preNode = currentNode;
        }
    }


    //https://ko.wikipedia.org/wiki/A*_%EC%95%8C%EA%B3%A0%EB%A6%AC%EC%A6%98
    public class Node :IComparable
    {
        public Direction dir;
        public BlockInfo tr;
        public Node preNode;
        public Vector2Int pos;
        public float fScore => hScore + gScore;
        public float hScore;    //꼭짓점 n으로부터 목표 꼭짓점까지의 추정 경로 가중치
        public float gScore;    //출발 꼭짓점으로부터 꼭짓점 n까지의 경로 가중치
        public bool open = true;

        public Node(Direction dir, BlockInfo transform, Node prePos, Vector2Int pos, float hScore, float gScore)
        {
            this.dir = dir;
            this.tr = transform;
            this.preNode = prePos;
            this.pos = pos;
            this.hScore = hScore;
            this.gScore = gScore;
        }

        public int CompareTo(object obj)
        {
            Node temp = obj as Node;

            if (temp == null) return 1;

            if (Mathf.Abs(this.fScore - temp.fScore) > 0.01f)
            {
                return this.fScore > temp.fScore ? 1 : -1;
            }

            if (Mathf.Abs(this.hScore - temp.hScore) > 0.01f)
            {
                return this.hScore > temp.hScore ? 1 : -1;
            }
            return 0;
        }
    }
}