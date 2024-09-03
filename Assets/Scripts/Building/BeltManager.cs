using System.Collections.Generic;
using UnityEngine;

public class BeltNode
{
    public Vector3Int gridPos { get; private set; }
    BeltNode previousNode;
    BeltNode nextNode;
    GameObject beltPart;

    public BeltNode(Vector3Int gridPos)
    {
        this.gridPos = gridPos;
        //GridMap.beltDic.Add(gridPos, this);
    }
    public void Init(BeltNode previous, BeltNode next)
    {
        previousNode = previous;
        nextNode = next;

        GameObject beltPrefab;
        Quaternion dir = Quaternion.identity;

        if (previousNode == null && nextNode == null)
        {
            beltPrefab = BeltManager.Instance.beltPart_Mid;
        }
        else if (previousNode == null)
        {
            beltPrefab = BeltManager.Instance.beltPart_Start;
            dir = Quaternion.LookRotation(next.gridPos - gridPos);
        }
        else if (nextNode == null)
        {
            beltPrefab = BeltManager.Instance.beltPart_End;
            dir = Quaternion.LookRotation(gridPos - previous.gridPos);
        }
        else
        {
            // 이전 노드에서 현재 노드로 가는 벡터
            Vector3 previousToCurrent = gridPos - previousNode.gridPos;

            // 현재 노드에서 다음 노드로 가는 벡터
            Vector3 currentToNext = nextNode.gridPos - gridPos;

            // 외적을 계산하여 Y축 값을 확인
            float angle = Vector3.SignedAngle(previousToCurrent, currentToNext, Vector3.up);
            Debug.Log(angle);
            if (angle > 0)
            {
                // 오른쪽으로 꺾임
                beltPrefab = BeltManager.Instance.beltPart_Right;
            }
            else if (angle < 0)
            {
                // 왼쪽으로 꺾임
                beltPrefab = BeltManager.Instance.beltPart_Left;
            }
            else
            {
                // 직선 (변화 없음)
                beltPrefab = BeltManager.Instance.beltPart_Mid;
            }
            dir = Quaternion.LookRotation(next.gridPos - gridPos);
        }

        if (beltPart != null)
        {
            ObjectPoolManager.Instance.EnqueueObject(beltPart);
        }
        beltPart = ObjectPoolManager.Instance.DequeueObject(beltPrefab, gridPos);
        beltPart.transform.rotation = dir;
    }
}

public class Belt
{
    LinkedList<BeltNode> beltNodes;

    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();    

    public void BuildBelt(Vector3Int lastNode)
    {
        CheckBuildBelt(lastNode);
        beltNodes = new LinkedList<BeltNode>();

        foreach (Vector3Int pos in path)
        {
            beltNodes.AddLast(new BeltNode(pos));
        }

        LinkedListNode<BeltNode> tempBeltNode = beltNodes.First;
        while(tempBeltNode != null)
        {
            BeltNode previousNode = (tempBeltNode.Previous != null) ? tempBeltNode.Previous.Value : null;
            BeltNode nextNode = (tempBeltNode.Next != null) ? tempBeltNode.Next.Value : null;            

            tempBeltNode.Value.Init(previousNode, nextNode);

            tempBeltNode = tempBeltNode.Next;
        }
    }

    public void StartBuildBelt(Vector3Int firstNode)
    {
        _firstNode = firstNode;
    }
    public void CheckBuildBelt(Vector3Int lastNode)
    {
        _lastNode = lastNode;
        path.Clear(); // 이전 경로를 지우고 새로 계산
        CalculatePath(_firstNode, _lastNode);
    }

    private void CalculatePath(Vector3Int start, Vector3Int end)
    {
        path.Clear();
        path.Add(start);

        Vector3Int posTemp = start;

        while(posTemp != end)
        {
            if(posTemp.x != end.x)
            {
                if(posTemp.x > end.x)
                {
                    posTemp.x--;                    
                }
                else
                {
                    posTemp.x++;                    
                }
            }
            else if(posTemp.z != end.z)
            {
                if (posTemp.z > end.z)
                {
                    posTemp.z--;
                }
                else
                {
                    posTemp.z++;
                }
            }

            path.Add(posTemp);
        }
    }
}

public class BeltManager : SceneSingleton<BeltManager>
{
    public GameObject beltPart_Start;
    public GameObject beltPart_End;
    public GameObject beltPart_Mid;
    public GameObject beltPart_Left;
    public GameObject beltPart_Right;

    Belt buildingBeltTemp;
    Vector3Int posTemp;

    public void OnClick(Vector3Int pos)
    {
        if(buildingBeltTemp == null)
        {
            StartBuildBelt(pos);
        }
        else
        {
            BuildBelt(pos);
        }
    }
    public void OnMove(Vector3Int pos)
    {
        if (buildingBeltTemp != null && posTemp != pos)
        {
            posTemp = pos;
            CheckBuildBelt(pos);
        }
    }

    void StartBuildBelt(Vector3Int firstNode)
    {
        buildingBeltTemp = new Belt();
        buildingBeltTemp.StartBuildBelt(firstNode);
    }
    void CheckBuildBelt(Vector3Int lastNode)
    {
        if(buildingBeltTemp != null)
        {
            buildingBeltTemp.CheckBuildBelt(lastNode);
        }
    }
    void BuildBelt(Vector3Int lastNode)
    {
        if (buildingBeltTemp != null)
        {
            buildingBeltTemp.BuildBelt(lastNode);
            buildingBeltTemp = null;
        }
    }
}