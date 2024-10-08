using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Linq;

public class BeltNode : Node
{
    Node _previousNode;
    Node _nextNode;

    public BeltModule beltPart;

    public BeltNode(Vector3Int gridPos)
    {
        this.gridPos = gridPos;
        nodeType = NodeType.BeltNode;        
    }

    public override Node PreviousNode 
    { 
        get 
        { 
            return _previousNode; 
        } 
        set 
        { 
            _previousNode = value;
            if(_previousNode != null)
            {
                RemoveRootNode(this);
            }
            else
            {
                SetRootNode(this);
            }
        }  
    }
    public override Node NextNode 
    { 
        get 
        { 
            return _nextNode; 
        } 
        set 
        { 
            _nextNode = value;
            if (_nextNode != null)
            {
                RemoveRootNode(this);
            }
            else
            {
                SetRootNode(this);
            }
        } 
    }

    public override void Init()
    {
        Quaternion dir = Quaternion.identity;
        BeltType type;

        if (PreviousNode == null && NextNode == null)
        {            
            type = BeltType.Mid;
        }
        else if (PreviousNode == null)
        {
            dir = Quaternion.LookRotation(NextNode.gridPos - gridPos);            
            type = BeltType.Mid;
        }
        else if (NextNode == null)
        {
            dir = Quaternion.LookRotation(gridPos - PreviousNode.gridPos);
            type = BeltType.Mid;
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
                type = BeltType.Right;
            }
            else if (angle < 0)
            {
                // 왼쪽으로 꺾임                
                type = BeltType.Left;
            }
            else
            {
                // 직선 (변화 없음)                
                type = BeltType.Mid;
            }
        }

        if (beltPart == null)
        {
            beltPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.beltPart, gridPos).GetComponent<BeltModule>();
        }

        beltPart.transform.rotation = dir;
        beltPart.SetBeltPart(type, this);

        Building building = beltPart.GetComponent<Building>();
        building.Init();
        building.Register_OnDismantle(Remove);

        transporter = beltPart;
    }

    public override void Remove() 
    {
        if (PreviousNode != null)
        {
            if (PreviousNode.nodeType == NodeType.BeltNode)
            {
                PreviousNode.NextNode = null;
            }
            else if (PreviousNode.nodeType == NodeType.SorterNode)
            {
                SorterNode sorterNode = PreviousNode as SorterNode;
                sorterNode.RemoveNode_InputOrOutput(this);
            }
        }
        if (NextNode != null)
        {
            if (NextNode.nodeType == NodeType.BeltNode)
            {
                NextNode.PreviousNode = null;
            }
            else if (NextNode.nodeType == NodeType.SorterNode)
            {
                SorterNode sorterNode = NextNode as SorterNode;
                sorterNode.RemoveNode_InputOrOutput(this);
            }
        }

        if (beltPart != null)
        {
            ObjectPoolManager.Instance.EnqueueObject(beltPart.gameObject);
        }
        beltPart = null;
        transporter = null;
        RemoveRootNode(this);

        GridMap.Remove_Dic_BeltDepth(this.gridPos);
    }
    //void Remove_OnDismantle()//우클릭 상호작용을 통해 해체
    //{
    //    if (PreviousNode != null)
    //    {
    //        if(PreviousNode.nodeType == NodeType.BeltNode)
    //        {
    //            PreviousNode.NextNode = null;
    //            SetRootNode(PreviousNode);
    //        }            
    //        else if(PreviousNode.nodeType == NodeType.SorterNode)
    //        {
    //            SorterNode sorterNode = PreviousNode as SorterNode;
    //            sorterNode.RemoveNode_InputOrOutput(this);
    //        }
    //    }
    //    if (NextNode != null)
    //    {
    //        if(NextNode.nodeType == NodeType.BeltNode)
    //        {
    //            NextNode.PreviousNode = null;
    //            SetRootNode(NextNode);
    //        }
    //        else if (NextNode.nodeType == NodeType.SorterNode)
    //        {
    //            SorterNode sorterNode = NextNode as SorterNode;
    //            sorterNode.RemoveNode_InputOrOutput(this);
    //        }
    //    }

    //    beltPart.RemoveBeltPart();
    //    Remove();
    //    GridMap.Remove_Dic_BeltDepth(this.gridPos);
    //}

    public static void SetRootNode_OnBeltCreate(Node tailNode, Node headNode)//벨트 로직 실행 순서를 설정하기 위한 메서드
    {
        Node currentNode = headNode;
        RemoveRootNode(headNode);

        if (headNode.nodeType == NodeType.SorterNode)//헤드가 소터일 경우, 해당 소터 노드를 초기 실행 멤버에 등록
        {
            SetSorterNode(headNode);
        }
        else
        {
            while (currentNode != null && currentNode.nodeType == NodeType.BeltNode)//아닐 경우, 맨 앞쪽의 소터가 아닌 노드를 등록
            {
                if (IsRootNode(currentNode) == false && currentNode.NextNode == null)
                {
                    SetRootNode(currentNode);
                    break;
                }
                else
                {
                    currentNode = currentNode.NextNode;
                    if (currentNode == headNode)
                    {
                        SetRootNode(currentNode);
                        break;
                    }
                }
            }
        }

        currentNode = tailNode;
        RemoveRootNode(tailNode);

        if (tailNode.nodeType == NodeType.SorterNode)//테일이 소터일 경우, 해당 소터 노드를 등록
        {
            SetSorterNode(tailNode);
        }
        else
        {
            while (currentNode != null && currentNode.nodeType == NodeType.BeltNode)
            {
                if (IsRootNode(currentNode) == false && currentNode.PreviousNode == null)
                {
                    SetRootNode(currentNode);
                    break;
                }
                else
                {
                    currentNode = currentNode.PreviousNode;
                    if (currentNode == tailNode)
                    {
                        break;
                    }
                }
            }
        }

        //Debug.Log(GameLogicManager.Instance.RootBeltNodeSet.Count);
    }

    public static bool IsRootNode(Node node)
    {
        return GameLogicManager.Instance.RootBeltNodeSet.Contains(node);
    }
    public static bool IsSorterNode(Node node)
    {
        return GameLogicManager.Instance.SorterNodeSet.Contains(node);
    }
    public static void SetRootNode(Node node)
    {
        GameLogicManager.Instance.RootBeltNodeSet.Add(node);
    }
    public static void SetSorterNode(Node node)
    {
        GameLogicManager.Instance.SorterNodeSet.Add(node);
    }
    public static void RemoveRootNode(Node node)
    {
        GameLogicManager.Instance.RootBeltNodeSet.Remove(node);        
    }
    public static void RemoveSorterNode(Node node)
    {
        GameLogicManager.Instance.SorterNodeSet.Remove(node);
    }
}

public class SorterNode : Node
{
    public SorterModule sorterPart;

    List<Node> inputNodeList = new List<Node>();//PreviousNode
    List<Node> outputNodeList = new List<Node>();//NextNode
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
    public void RemoveNode_InputOrOutput(Node node)
    {
        if(inputNodeList.Contains(node))
        {
            inputNodeList.Remove(node);
            _usePort--;
        }
        else if(outputNodeList.Contains(node))
        {
            outputNodeList.Remove(node);
            _usePort--;
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
            sorterPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.sorterPart, gridPos).GetComponent<SorterModule>();
        }
        
        sorterPart.SetSorterPart(inputNodeList, outputNodeList);

        Building building = sorterPart.GetComponent<Building>();
        building.Init();
        building.Register_OnDismantle(Remove);

        transporter = sorterPart;
    }
    public override void Remove() 
    {
        foreach (var inputNode in inputNodeList)
        {
            if (inputNode.nodeType == NodeType.BeltNode)
            {
                inputNode.NextNode = null;
            }
            else if (inputNode.nodeType == NodeType.SorterNode)
            {
                SorterNode sorterNode = inputNode as SorterNode;
                sorterNode.RemoveNode_InputOrOutput(this);
            }
        }
        foreach (var outputNode in outputNodeList)
        {
            if (outputNode.nodeType == NodeType.BeltNode)
            {
                outputNode.PreviousNode = null;
            }
            else if (outputNode.nodeType == NodeType.SorterNode)
            {
                SorterNode sorterNode = outputNode as SorterNode;
                sorterNode.RemoveNode_InputOrOutput(this);
            }
        }

        if (sorterPart != null)
        {
            ObjectPoolManager.Instance.EnqueueObject(sorterPart.gameObject);
        }
        sorterPart = null;
        transporter = null;
        BeltNode.RemoveSorterNode(this);

        GridMap.Remove_Dic_BeltDepth(this.gridPos);
    }
    //void Remove_OnDismantle()
    //{
    //    foreach (var inputNode in inputNodeList)
    //    {
    //        if (inputNode.nodeType == NodeType.BeltNode)
    //        {
    //            inputNode.NextNode = null;
    //            BeltNode.SetRootNode(inputNode);
    //        }
    //        else if (inputNode.nodeType == NodeType.SorterNode)
    //        {
    //            SorterNode sorterNode = inputNode as SorterNode;
    //            sorterNode.RemoveNode_InputOrOutput(this);
    //        }
    //    }
    //    foreach (var outputNode in outputNodeList)
    //    {
    //        if (outputNode.nodeType == NodeType.BeltNode)
    //        {
    //            outputNode.PreviousNode = null;
    //            BeltNode.SetRootNode(outputNode);
    //        }
    //        else if (outputNode.nodeType == NodeType.SorterNode)
    //        {
    //            SorterNode sorterNode = outputNode as SorterNode;
    //            sorterNode.RemoveNode_InputOrOutput(this);
    //        }
    //    }

    //    sorterPart.RemoveSorterPart();
    //    Remove();
    //    GridMap.Remove_Dic_BeltDepth(this.gridPos);
    //}
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
            if (GridMap.Dic_BeltDepth.ContainsKey(path[i]))//경로상에 이미 노드가 있음
            {
                Node selectNode = GridMap.Dic_BeltDepth[path[i]];

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

        BeltNode.SetRootNode_OnBeltCreate(buildBeltNodeList.First(), buildBeltNodeList.Last());
        
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

        if (GridMap.Dic_BeltDepth.ContainsKey(start) && GridMap.Dic_BeltDepth[start].nodeType == NodeType.BuildingNode)
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

            if (GridMap.Dic_OccupiedDepth.ContainsKey(posTemp))
            {
                if (GridMap.Dic_BeltDepth[posTemp].nodeType == NodeType.BeltNode) { path.Add(posTemp); }
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
    public void SetBuildingData(BuildingData buildingData)
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