using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class BeltNode : Node
{
    public Belt beltPart;
    public bool isSplitter = false;

    public BeltNode(Vector3Int gridPos)
    {
        nodeType = NodeType.BeltNode;
        this.gridPos = gridPos;
        isSplitter = false;

        GridMap.NodeDic_NormalDepth.Add(gridPos, this);
        BeltManager.Instance.AllNodeList.Add(this);
    }
    public void Init()
    {        
        if(isSplitter == false)
        {
            Init_Belt();
        }
        else
        {
            Init_Splitter();
        }
    }

    void Init_Belt()
    {
        Quaternion dir = Quaternion.identity;

        if (PreviousNode == null && NextNode == null)
        {
            SetBeltType(BeltType.Mid, dir);
        }
        else if (PreviousNode == null)
        {
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);
            SetBeltType(BeltType.Start, dir);
        }
        else if (NextNode == null)
        {
            dir = Quaternion.LookRotation(gridPos - PreviousNode.gridPos);
            SetBeltType(BeltType.End, dir);
        }
        else
        {
            // 이전 노드에서 현재 노드로 가는 벡터
            Vector3 previousToCurrent = gridPos - PreviousNode.gridPos;

            // 현재 노드에서 다음 노드로 가는 벡터
            Vector3 currentToNext = NextNode.gridPos - gridPos;

            // 외적을 계산하여 Y축 값을 확인
            float angle = Vector3.SignedAngle(previousToCurrent, currentToNext, Vector3.up);
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);

            if (angle > 0)
            {
                // 오른쪽으로 꺾임
                SetBeltType(BeltType.Right, dir);
            }
            else if (angle < 0)
            {
                // 왼쪽으로 꺾임                
                SetBeltType(BeltType.Left, dir);
            }
            else
            {
                // 직선 (변화 없음)                
                SetBeltType(BeltType.Mid, dir);
            }
        }
    }
    void Init_Splitter()
    {
        SetBeltType(BeltType.Splitter, Quaternion.identity);
    }


    void SetBeltType(BeltType type, Quaternion dir)
    {
        if (beltPart == null)
        {
            beltPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.beltPart, gridPos).GetComponent<Belt>();
        }

        beltPart.transform.rotation = dir;
        beltPart.SetBeltPart(type, this);

        transporter = beltPart;
    }
}

public class BeltCreator
{
    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();
    List<BeltNode> buildBeltNodeList = new List<BeltNode>();

    public void BuildBelt(Vector3Int lastNode)
    {
        CheckBuildBelt(lastNode);
        BuildLineRenderer.Instance.HideAllGridLinesAndNodes();

        for (int i = 0; i < path.Count; i++)
        {
            if (GridMap.NodeDic_NormalDepth.ContainsKey(path[i]))//경로상에 이미 벨트가 있음
            {
                buildBeltNodeList.Add(GridMap.NodeDic_NormalDepth[path[i]] as BeltNode);

                if (i == 0)//시작점
                {
                    if (buildBeltNodeList[i].NextNode != null)//중간 노드에서 시작하는 경우 : 병합기 생성
                    {
                        buildBeltNodeList[i].isSplitter = true;
                    }
                    BeltManager.Instance.RootNodeDic.Remove(path[i]);
                }
                else if(i == path.Count - 1)//연결점 (마지막 노드)
                {
                    buildBeltNodeList[i - 1].NextNode = buildBeltNodeList[i];//이전 노드와 연결 작업

                    if (buildBeltNodeList[i].PreviousNode == null)//맨 끝단 노드에 연결하는 경우 : 경로를 자연스럽게 이어줌
                    {
                        buildBeltNodeList[i].PreviousNode = buildBeltNodeList[i - 1];
                    }
                    else//중간 노드에 연결하는 경우 : 병합기 생성
                    {
                        buildBeltNodeList[i].isSplitter = true;
                    }   
                    
                    BeltManager.Instance.RootNodeDic.Add(path[i], buildBeltNodeList[i - 1]);
                }
            }
            else
            {
                buildBeltNodeList.Add(new BeltNode(path[i]));

                if (i > 0)//시작점을 제외한 모든 노드
                {
                    buildBeltNodeList[i - 1].NextNode = buildBeltNodeList[i];
                    buildBeltNodeList[i].PreviousNode = buildBeltNodeList[i - 1];

                    if(i == path.Count - 1)//마지막 노드 : 루트 노드로 등록
                    {
                        BeltManager.Instance.RootNodeDic.Add(path[i], buildBeltNodeList[i]);
                    }
                }                
            }
        }

        foreach (var node in buildBeltNodeList)
        {
            node.Init();
        }

        buildBeltNodeList.Clear();
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

        if (GridMap.NodeDic_NormalDepth.ContainsKey(start) && GridMap.NodeDic_NormalDepth[start].nodeType == NodeType.BuildingNode)
        {
            BuildLineRenderer.Instance.DrawBeltLine(path);
            return;
        }

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

            if (GridMap.NodeDic_NormalDepth.ContainsKey(posTemp))
            {
                if (GridMap.NodeDic_NormalDepth[posTemp].nodeType == NodeType.BeltNode) { path.Add(posTemp); }
                break;
            }
            else
            {
                path.Add(posTemp);
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

    bool isBuildMode = false;
    public Dictionary<Vector3Int, BeltNode> RootNodeDic = new Dictionary<Vector3Int, BeltNode>();
    public List<BeltNode> AllNodeList = new List<BeltNode>();

    void Update()
    {
        foreach (var item in AllNodeList)
        {
            item.beltPart.LogicInit();
        }
        
        foreach (var item in RootNodeDic)
        {
            item.Value.beltPart.ExcuteLogic_OnUpdate(Time.deltaTime);
        }
    }

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
}