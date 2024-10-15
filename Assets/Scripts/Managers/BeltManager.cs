using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Linq;

public class BeltNode : INode
{
    NodeType _nodeType;
    ITransporter _transporter;
    Vector3Int _gridPos;
    INode _previousNode;
    INode _nextNode;

    public BeltModule beltPart;

    public BeltNode(Vector3Int gridPos)
    {
        GridPos = gridPos;
        NodeType = NodeType.BeltNode;        
    }

    public NodeType NodeType { get => _nodeType; set => _nodeType = value; }
    public ITransporter Transporter { get => _transporter; set => _transporter = value; }
    public Vector3Int GridPos { get => _gridPos; set => _gridPos = value; }
    public INode PreviousNode 
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
    public INode NextNode 
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

    public void Init()
    {
        Quaternion dir = Quaternion.identity;
        BeltType type;

        if (PreviousNode == null && NextNode == null)
        {            
            type = BeltType.Mid;
        }
        else if (PreviousNode == null)
        {
            dir = Quaternion.LookRotation(NextNode.GridPos - GridPos);            
            type = BeltType.Mid;
        }
        else if (NextNode == null)
        {
            dir = Quaternion.LookRotation(GridPos - PreviousNode.GridPos);
            type = BeltType.Mid;
        }
        else
        {
            // 이전 노드에서 현재 노드로 가는 벡터
            Vector3 previousToCurrent = GridPos - PreviousNode.GridPos;

            // 현재 노드에서 다음 노드로 가는 벡터
            Vector3 currentToNext = NextNode.GridPos - GridPos;

            // 외적을 계산하여 Y축 값을 확인
            float angle = Vector3.SignedAngle(previousToCurrent, currentToNext, Vector3.up);
            dir = Quaternion.LookRotation(NextNode.GridPos - GridPos);

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
            beltPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.beltPart, GridPos).GetComponent<BeltModule>();
        }

        beltPart.transform.rotation = dir;
        beltPart.SetBeltPart(type, this);

        Building building = beltPart.GetComponent<Building>();
        building.Init();
        building.Register_OnDismantle(Remove);

        Transporter = beltPart;
    }

    public void Remove() 
    {
        if (PreviousNode != null)
        {
            if (PreviousNode.NodeType == NodeType.BeltNode)
            {
                PreviousNode.NextNode = null;
            }
            else if (PreviousNode.NodeType == NodeType.SorterNode)
            {
                SorterNode sorterNode = PreviousNode as SorterNode;
                sorterNode.RemoveNode_InputOrOutput(this);
            }
        }
        if (NextNode != null)
        {
            if (NextNode.NodeType == NodeType.BeltNode)
            {
                NextNode.PreviousNode = null;
            }
            else if (NextNode.NodeType == NodeType.SorterNode)
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
        Transporter = null;
        RemoveRootNode(this);

        GridMap.Remove_Dic_BeltDepth(GridPos);
    }

    public static void SetRootNode_OnBeltCreate(INode tailNode, INode headNode)//벨트 로직 실행 순서를 설정하기 위한 메서드
    {
        INode currentNode = headNode;
        RemoveRootNode(headNode);

        if (headNode.NodeType == NodeType.SorterNode)//헤드가 소터일 경우, 해당 소터 노드를 초기 실행 멤버에 등록
        {
            SetSorterNode(headNode);
        }
        else
        {
            while (currentNode != null && currentNode.NodeType == NodeType.BeltNode)//아닐 경우, 맨 앞쪽의 소터가 아닌 노드를 등록
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

        if (tailNode.NodeType == NodeType.SorterNode)//테일이 소터일 경우, 해당 소터 노드를 등록
        {
            SetSorterNode(tailNode);
        }
        else
        {
            while (currentNode != null && currentNode.NodeType == NodeType.BeltNode)
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
    }

    public static bool IsRootNode(INode node)
    {
        return GameLogicManager.Instance.RootBeltNodeSet.Contains(node);
    }
    public static bool IsSorterNode(INode node)
    {
        return GameLogicManager.Instance.SorterNodeSet.Contains(node);
    }
    public static void SetRootNode(INode node)
    {
        GameLogicManager.Instance.RootBeltNodeSet.Add(node);
    }
    public static void SetSorterNode(INode node)
    {
        GameLogicManager.Instance.SorterNodeSet.Add(node);
    }
    public static void RemoveRootNode(INode node)
    {
        GameLogicManager.Instance.RootBeltNodeSet.Remove(node);        
    }
    public static void RemoveSorterNode(INode node)
    {
        GameLogicManager.Instance.SorterNodeSet.Remove(node);
    }
}

public class SorterNode : INode
{
    NodeType _nodeType;
    ITransporter _transporter;
    Vector3Int _gridPos;

    public SorterModule sorterPart;

    List<INode> inputNodeList = new List<INode>();//PreviousNode
    List<INode> outputNodeList = new List<INode>();//NextNode
    int _usePort = 0;

    public NodeType NodeType { get => _nodeType; set => _nodeType = value; }
    public ITransporter Transporter { get => _transporter; set => _transporter = value; }
    public Vector3Int GridPos { get => _gridPos; set => _gridPos = value; }
    public INode PreviousNode 
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
                Debug.Log($"소터에 인포트를 추가합니다. {value.GridPos}");
                _usePort++;
            }
            else
            {
                Debug.Log("소터에 더 이상 포트를 추가할 수 없습니다.");
            }
        }     
    }
    public INode NextNode 
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
                Debug.Log($"소터에 아웃포트를 추가합니다. {value.GridPos}");
                _usePort++;
            }
            else
            {
                Debug.Log("소터에 더 이상 포트를 추가할 수 없습니다.");
            }
        } 
    }
    public void RemoveNode_InputOrOutput(INode node)
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
        GridPos = gridPos;
        NodeType = NodeType.SorterNode;
    }
    public void Init()
    {
        if (sorterPart == null)
        {
            sorterPart = ObjectPoolManager.Instance.DequeueObject(BeltManager.Instance.sorterPart, GridPos).GetComponent<SorterModule>();
        }
        
        sorterPart.SetSorterPart(inputNodeList, outputNodeList);

        Building building = sorterPart.GetComponent<Building>();
        building.Init();
        building.Register_OnDismantle(Remove);

        Transporter = sorterPart;
    }
    public void Remove() 
    {
        foreach (var inputNode in inputNodeList)
        {
            if (inputNode.NodeType == NodeType.BeltNode)
            {
                inputNode.NextNode = null;
            }
            else if (inputNode.NodeType == NodeType.SorterNode)
            {
                SorterNode sorterNode = inputNode as SorterNode;
                sorterNode.RemoveNode_InputOrOutput(this);
            }
        }
        foreach (var outputNode in outputNodeList)
        {
            if (outputNode.NodeType == NodeType.BeltNode)
            {
                outputNode.PreviousNode = null;
            }
            else if (outputNode.NodeType == NodeType.SorterNode)
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
        Transporter = null;
        BeltNode.RemoveSorterNode(this);

        GridMap.Remove_Dic_BeltDepth(GridPos);
    }
}

public class BeltCreator
{
    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();
    List<INode> buildBeltNodeList = new List<INode>();

    public void BuildBelt(Vector3Int lastNode, BuildingData data)
    {
        CheckBuildBelt(lastNode);
        BuildLineRenderer.Instance.HideAllGridLinesAndNodes();

        ushort buildingItemId = JsonDataManager.GetItem(data.Id.Replace_ToItem()).Id_UShort;
        if (Player.Instance.CanUseItem(buildingItemId) == false)//인벤토리에 아이템이 없거나 키가 잘못되었을 경우
        {
            Command_ToolDeActive("아이템이 부족합니다");
            return;
        }

        for (int i = 0; i < path.Count; i++)
        {
            if (GridMap.Dic_BeltDepth.ContainsKey(path[i]))//경로상에 이미 노드가 있음
            {
                INode selectNode = GridMap.Dic_BeltDepth[path[i]];

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
                    if(selectNode.NodeType == NodeType.BeltNode)
                    {
                        selectNode = GridMap.CreateSorterNode(path[i]);
                        Debug.Log("신규 병합기 생성");
                    }
                    else if(selectNode.NodeType == NodeType.SorterNode)
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

            Player.Instance.UseItem(buildingItemId);
            if (Player.Instance.CanUseItem(buildingItemId) == false)//인벤토리에 아이템이 없거나 키가 잘못되었을 경우
            {
                break;
            }
        }        

        foreach (INode node in buildBeltNodeList)
        {
            node.Init();
        }

        BeltNode.SetRootNode_OnBeltCreate(buildBeltNodeList.First(), buildBeltNodeList.Last());
        
        buildBeltNodeList.Clear();

        if (Player.Instance.CanUseItem(buildingItemId) == false)//인벤토리에 아이템이 없거나 키가 잘못되었을 경우
        {
            Command_ToolDeActive("아이템이 부족합니다");            
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
    public void OnDeActive()
    {
        path.Clear();
        BuildLineRenderer.Instance.DrawBeltLine(path);
    }
    void Command_ToolDeActive(string msg)
    {
        Player.Instance.ControlledCharactor.builtIn_ToolModule.Command_ToolDeActive(msg);
    }
    private void CalculatePath(Vector3Int start, Vector3Int end)
    {
        path.Clear();

        if (GridMap.Dic_BeltDepth.ContainsKey(start) && GridMap.Dic_BeltDepth[start].NodeType == NodeType.BuildingNode)
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
                if (GridMap.Dic_BeltDepth[posTemp].NodeType == NodeType.BeltNode) { path.Add(posTemp); }
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

    BuildingData buildingData;

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
        this.buildingData = buildingData;
    }
    public void DeActive()
    {
        buildingBeltTemp.OnDeActive();
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
        buildingBeltTemp.BuildBelt(lastNode, buildingData);
    }
}