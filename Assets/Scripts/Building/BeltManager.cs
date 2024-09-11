using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class BeltNode : Node
{
    Node _previousNode;
    Node _nextNode;

    public Belt beltPart;

    public BeltNode(Vector3Int gridPos)
    {
        this.gridPos = gridPos;
        nodeType = NodeType.BeltNode;        
    }

    public override Node PreviousNode { get { return _previousNode; } set { _previousNode = value; }  }
    public override Node NextNode { get { return _nextNode; } set { _nextNode = value; } }

    public override void Init()
    {
        Quaternion dir = Quaternion.identity;

        if (PreviousNode == null && NextNode == null)
        {
            SetBeltType(BeltType.Mid, dir);
        }
        else if (PreviousNode == null)
        {
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);
            SetBeltType(BeltType.Mid, dir);
        }
        else if (NextNode == null)
        {
            dir = Quaternion.LookRotation(gridPos - PreviousNode.gridPos);
            SetBeltType(BeltType.Mid, dir);
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
    public override void Remove() 
    {
        if (beltPart != null)
        {
            ObjectPoolManager.Instance.EnqueueObject(beltPart.gameObject);
        }
        beltPart = null;
        transporter = null;
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

        if(NextNode == null || PreviousNode == null)
        {
            GameLogicManager.Instance.RootBeltNodeSet.Add(this);
        }
    }
}

public class SorterNode : Node
{
    public Sorter sorterPart;

    List<Node> inputNodeList = new List<Node>();
    List<Node> outputNodeList = new List<Node>();
    int _usePort = 0;
    public override Node PreviousNode 
    { 
        get 
        { 
            return (inputNodeList.Count > 0) ? inputNodeList[0] : null; 
        } 
        set 
        {
            if (_usePort <= 4 && value != null)
            {
                inputNodeList.Add(value);
                Debug.Log($"소터에 인포트를 추가합니다. {value.gridPos}");
                _usePort++;
            }
            else
            {
                Debug.Log("소터에 더 이상 포트를 추가할 수 없습니다.");
            }
        }     
    }
    public override Node NextNode 
    { 
        get 
        { 
            return (outputNodeList.Count > 0) ? outputNodeList[0] : null; 
        } 
        set 
        {
            if (_usePort <= 4 && value != null)
            {
                outputNodeList.Add(value);
                Debug.Log($"소터에 아웃포트를 추가합니다. {value.gridPos}");
                _usePort++;
            }
            else
            {
                Debug.Log("소터에 더 이상 포트를 추가할 수 없습니다.");
            }
        } 
    }

    public SorterNode(Vector3Int gridPos)
    {
        this.gridPos = gridPos;
        nodeType = NodeType.SorterNode;
    }
    public override void Init()
    {
        if (sorterPart == null)
        {
            sorterPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.sorterPart, gridPos).GetComponent<Sorter>();
        }
        
        sorterPart.SetSorterPart(inputNodeList, outputNodeList);
        transporter = sorterPart;
        GameLogicManager.Instance.SorterNodeSet.Add(this);
    }
    public override void Remove() 
    {
        if (sorterPart != null)
        {
            ObjectPoolManager.Instance.EnqueueObject(sorterPart.gameObject);
        }
        sorterPart = null;
        transporter = null;
    }
}

public class BeltCreator
{
    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();
    List<Node> buildBeltNodeList = new List<Node>();

    public void BuildBelt(Vector3Int lastNode)
    {
        CheckBuildBelt(lastNode);
        BuildLineRenderer.Instance.HideAllGridLinesAndNodes();

        for (int i = 0; i < path.Count; i++)
        {
            if (GridMap.NodeDic_NormalDepth.ContainsKey(path[i]))//경로상에 이미 노드가 있음
            {
                Node selectNode = GridMap.NodeDic_NormalDepth[path[i]];

                if (i == 0 && selectNode.NextNode == null)//전방 말단 노드와 연결하는 경우
                {
                    //buildBeltNodeList.Add(selectNode);
                }
                else if (i == path.Count - 1 && selectNode.PreviousNode == null)//후방 말단 노드와 연결하는 경우
                {
                    //buildBeltNodeList.Add(selectNode);
                }
                else//중단 노드에 연결하는 경우 : 병합기 생성
                {
                    if(selectNode.nodeType == NodeType.BeltNode)
                    {
                        selectNode = GridMap.CreateSorterNode(path[i]);
                        Debug.Log("신규 병합기 생성");
                    }
                    else if(selectNode.nodeType == NodeType.SorterNode)
                    {
                        Debug.Log("기존 병합기에 연결");
                    }
                    else
                    {
                        Debug.LogError("잘못된 노드 연결입니다.");
                    }
                }
                buildBeltNodeList.Add(selectNode);
            }
            else
            {
                buildBeltNodeList.Add(GridMap.CreateBeltNode(path[i]));
            }            

            if (i > 0)//시작점을 제외한 모든 노드
            {
                buildBeltNodeList[i - 1].NextNode = buildBeltNodeList[i];
                buildBeltNodeList[i].PreviousNode = buildBeltNodeList[i - 1];
            }
        }        

        foreach (Node node in buildBeltNodeList)
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
    public GameObject sorterPart;

    BeltCreator buildingBeltTemp = new BeltCreator();
    Vector3Int posTemp;
    

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
    public void OnKeyDown(KeyCode key)
    {

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