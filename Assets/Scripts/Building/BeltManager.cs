using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class BeltNode
{
    public Vector3Int gridPos { get; private set; }
    public BeltNode PreviousNode { get; set; }
    public BeltNode NextNode { get; set; }
    Belt beltPart;

    public BeltNode(Vector3Int gridPos)
    {
        this.gridPos = gridPos;
        GridMap.beltDic.Add(gridPos, this);
    }
    public void Init()
    {
        BeltType beltType;
        Quaternion dir = Quaternion.identity;

        if (PreviousNode == null && NextNode == null)
        {
            beltType = BeltType.Mid;
        }
        else if (PreviousNode == null)
        {
            beltType = BeltType.Start;
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);
        }
        else if (NextNode == null)
        {
            beltType = BeltType.End;
            dir = Quaternion.LookRotation(gridPos - PreviousNode.gridPos);
        }
        else if(NextNode.PreviousNode != this)
        {
            beltType = BeltType.Merge;
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);
        }
        else
        {
            // 이전 노드에서 현재 노드로 가는 벡터
            Vector3 previousToCurrent = gridPos - PreviousNode.gridPos;

            // 현재 노드에서 다음 노드로 가는 벡터
            Vector3 currentToNext = NextNode.gridPos - gridPos;

            // 외적을 계산하여 Y축 값을 확인
            float angle = Vector3.SignedAngle(previousToCurrent, currentToNext, Vector3.up);
            Debug.Log(angle);
            if (angle > 0)
            {
                // 오른쪽으로 꺾임
                beltType = BeltType.Right;
            }
            else if (angle < 0)
            {
                // 왼쪽으로 꺾임
                beltType = BeltType.Left;
            }
            else
            {
                // 직선 (변화 없음)
                beltType = BeltType.Mid;
            }
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);
        }

        if (beltPart == null)
        {
            beltPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.beltPart, gridPos).GetComponent<Belt>();
        }

        beltPart.transform.rotation = dir;
        beltPart.SetBeltPart(beltType);
    }
}

public class BeltCreator
{
    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();    

    public void BuildBelt(Vector3Int lastNode)
    {
        CheckBuildBelt(lastNode);
        BuildLineRenderer.Instance.HideAllGridLinesAndNodes();

        List<BeltNode> beltNodes = BeltManager.Instance.NewBeltNodeList();

        for (int i = 0; i < path.Count; i++)
        {
            if (GridMap.beltDic.ContainsKey(path[i]))
            {
                if(i == 0)//시작점
                {
                    beltNodes.Add(GridMap.beltDic[path[i]]);
                }
                else if(i == path.Count - 1)//마지막점
                {
                    beltNodes[i - 1].NextNode = GridMap.beltDic[path[i]];
                }
            }
            else
            {
                beltNodes.Add(new BeltNode(path[i]));

                if (i > 0)
                {
                    beltNodes[i - 1].NextNode = beltNodes[i];
                    beltNodes[i].PreviousNode = beltNodes[i - 1];
                }
            }
        }

        foreach (var node in beltNodes)
        {
            node.Init();
        }
    }

    public void StartBuildBelt(Vector3Int firstNode)
    {
        _firstNode = firstNode;
    }
    public void CheckBuildBelt(Vector3Int lastNode)
    {
        _lastNode = lastNode;
        
        CalculatePath(_firstNode, _lastNode);
    }
    public void DeActive()
    {
        path.Clear();
        BuildLineRenderer.Instance.DrawBeltLine(path);
    }

    private void CalculatePath(Vector3Int start, Vector3Int end)
    {
        path.Clear();

        Vector3Int posTemp = start;
        path.Add(posTemp);

        while (posTemp != end && path.Count < 100)
        {
            if (posTemp.x != end.x)
            {
                if (posTemp.x > end.x)
                {
                    posTemp.x--;
                }
                else
                {
                    posTemp.x++;
                }
            }
            else if (posTemp.z != end.z)
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
            if (GridMap.beltDic.ContainsKey(posTemp))
            {
                break;
            }
        }

        BuildLineRenderer.Instance.DrawBeltLine(path);
    }
}

public class BeltManager : SceneSingleton<BeltManager>, IBuildTool
{
    public GameObject beltPart;

    BeltCreator buildingBeltTemp = new BeltCreator();
    Vector3Int posTemp;

    List<List<BeltNode>> BeltNodeTemp = new List<List<BeltNode>>();
    bool isBuildMode = false;

    public void OnClick(Vector3Int pos)
    {
        if(isBuildMode == false)
        {
            isBuildMode = true;
            StartBuildBelt(pos);
        }
        else
        {
            isBuildMode = false;
            BuildBelt(pos);
        }
    }
    public void OnMove(Vector3Int pos)
    {
        if (isBuildMode == true && posTemp != pos)
        {
            posTemp = pos;
            CheckBuildBelt(pos);
        }
    }
    public void DeActive()
    {
        buildingBeltTemp.DeActive();
        isBuildMode = false;
    }

    void StartBuildBelt(Vector3Int firstNode)
    {
        buildingBeltTemp.StartBuildBelt(firstNode);
    }
    void CheckBuildBelt(Vector3Int lastNode)
    {
        buildingBeltTemp.CheckBuildBelt(lastNode);
    }
    void BuildBelt(Vector3Int lastNode)
    {
        buildingBeltTemp.BuildBelt(lastNode);
    }

    public List<BeltNode> NewBeltNodeList()
    {
        BeltNodeTemp.Add(new List<BeltNode>());
        return BeltNodeTemp[BeltNodeTemp.Count - 1];
    }
}