using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockManager : FindPathParent
{
    //FBS
    override protected IEnumerator SearchCo(Pos start, Vector2Int find, List<CardList> board, Pos result)
    {
        Dictionary<Direction, Vector2Int> directions = new Dictionary<Direction, Vector2Int>();
        directions[Direction.Left] = new Vector2Int(-1, 0);
        directions[Direction.Right] = new Vector2Int( 1, 0);
        directions[Direction.Down] = new Vector2Int( 0,-1);
        directions[Direction.Up] = new Vector2Int( 0, 1);

        int width = board.Count;
        int height = board[0].items.Count;

        // start 출발 위치, start_alpha 출발지 알파벳
        Queue<Pos> q = new Queue<Pos>();
        List<List<int>> cornerCountMap = new List<List<int>>(); // 꺾은 횟수 저장

        //꺾은 횟수 임의의 아주 큰수로 설정
        for (int i = 0; i < width; i++)
        {
            cornerCountMap.Add(new List<int>());
            for (int y = 0; y < height; y++)
            {
                cornerCountMap[i].Add(int.MaxValue);
            }
        }


        // 출발 지점 예약
        start.dir = Direction.None;
        q.Enqueue(start);
        cornerCountMap[start.x][start.y] = 0;

        bool first = true; // 출발지의 알파벳과 동일한 위치에서 종료할건데 바로 출발지의 알파벳과 동일하다고 판정되어 종료되면 안되기 때문에 사용할 플래그

        while (q.Count > 0)
        {
            // 방문
            Pos now = q.Dequeue();

            board[now.x].blockInfos[now.y].SetActiveState();

            if (simulateSpeed > 0)
                yield return new WaitForSeconds(simulateSpeed);

            // 짝꿍을 찾았다면! (출발지가 아니고!)
            if (first == false && now.x == find.x && now.y == find.y)
            {
                result.x = now.x;
                result.y = now.y;
                yield break;
            }

            first = false; // 출발지 방문시에만 false 상태고 나머지 위치 방문시엔 모두 true 인 상태

            for (Direction i = 0; i < Direction.Last; ++i)
            {
                var dir = directions[i];
                int nextX = now.x + dir.x;
                int nextY = now.y + dir.y;
                Direction nextDir = i;
                int cornerCount = cornerCountMap[now.x][now.y]; // 현재 방문 위치까지 꺾은 횟수가 초기값
                if (now.dir != Direction.None && now.dir != nextDir) // 출발지가 아니고(출발지의 방향은 -1로 하였다. 출발지에서 예약되는 위치들은 꺾였다고 판단되지 않기 위해) 방향이 일치하지 않으면 꺾어야 한다. 꺾는 횟수를 1 증가시켜야 한다.
                    cornerCount++;

                // 다음 방문 후보 검사
                if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height) // 1. 범위 내에 있어야 함
                    continue;

                if (cornerCount >= 3) // 꺾은 횟수가 3 이상이 되면 그 위치는 탐색하지 않는다.⭐
                    continue;

                if (board[nextX].items[nextY] != BlockType.Walkable && false == (nextX == find.x && nextY == find.y))
                    //if (board[nextX].items[nextY] != BlockType.Walkable && board[nextX].items[nextY] != find) // 다른 숫자나 장애물(0) 이라면 갈 수 없음,(1은 갈 수 있음)
                    continue;

                if (cornerCountMap[nextX][nextY] >= cornerCount)
                { 
                // 4. 기존에 찾은 꺾은 횟수 그 이하로 꺾을 수 있다면 더 적은 횟수로 꺾을 수 있는 가능성이 있는 탐색 경로가 되므로 또 삽입
                    q.Enqueue(new Pos() { x = nextX, y = nextY, dir = nextDir });
                    cornerCountMap[nextX][nextY] = cornerCount; // 위치별 현재까지 꺾은 횟수 업데이트
                    board[nextX].blockInfos[nextY].parent = board[now.x].blockInfos[now.y];
                }
            }
        }
        result.x = -1;
        result.y = -1;
        //return new Pos() { x = -1, y = -1 }; // while문을 빠져나왔다면 짝꿍알파벳을 찾지 못한 것이다. 즉, 제거 불가능! 제거 불가능시에는 {-1, -1}를 리턴하기로 했다.
    }
}
