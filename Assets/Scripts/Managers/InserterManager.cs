using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class InserterNode : Node
{
    Inserter inserterPart;
    Vector3Int lastPos;

    Node _previousNode;
    Node _nextNode;

    public override Node PreviousNode { get { return _previousNode; } set { _previousNode = value; } }
    public override Node NextNode { get { return _nextNode; } set { _nextNode = value; } }
    public InserterNode(Vector3Int firstPos, Vector3Int lastPos)
    {
        nodeType = NodeType.InserterNode;
        this.gridPos = firstPos;
        this.lastPos = lastPos;
    }

    public override void Init()
    {
        if (GridMap.NodeDic_NormalDepth.ContainsKey(gridPos))
        {
            this.PreviousNode = GridMap.NodeDic_NormalDepth[gridPos];
            Debug.Log("이전 노드 연결");
        }
        if (GridMap.NodeDic_NormalDepth.ContainsKey(lastPos))
        {
            this.NextNode = GridMap.NodeDic_NormalDepth[lastPos];
            Debug.Log("다음 노드 연결");
        }

        if (inserterPart == null)
        {
            inserterPart = ObjectPoolManager.Instance.DequeueObject(InserterManager.Instance.Prefab_inserterPart).GetComponent<Inserter>();
        }

        inserterPart.Init(gridPos, lastPos, this);
        transporter = inserterPart;

        GameLogicManager.Instance.InserterNodeSet.Add(this);
    }
    public override void Remove() { }
}

public class InserterCrafter
{
    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();    

    public void BuildInserter(Vector3Int firstNode, GridDir gridDir)
    {
        CheckBuildInserter(firstNode, gridDir);
        BuildLineRenderer.Instance.HideAllGridLinesAndNodes();

        InserterNode createNode = GridMap.CreateInserter(_firstNode, _lastNode);
        createNode.Init();        
    }

    public void CheckBuildInserter(Vector3Int firstNode, GridDir gridDir)
    {
        _firstNode = firstNode;
        _lastNode = firstNode;

        switch (gridDir)
        {
            case GridDir.Top:
                _lastNode.z++;
                break;
            case GridDir.Right:
                _lastNode.x++;
                break;
            case GridDir.Bottom:
                _lastNode.z--;
                break;
            case GridDir.Left:
                _lastNode.x--;
                break;
            default:
                break;
        }

        CalculatePath(_firstNode, _lastNode);
    }
    public void DeActive()
    {
        path.Clear();
        BuildLineRenderer.Instance.DrawBeltLine(path);
    }

    void CalculatePath(Vector3Int start, Vector3Int end)
    {
        path.Clear();
        
        path.Add(start);
        path.Add(end);

        BuildLineRenderer.Instance.DrawBeltLine(path);
    }
}

public class InserterManager : SceneSingleton<InserterManager>, IBuildTool
{
    public GameObject Prefab_inserterPart;

    InserterCrafter inserterCrafter = new InserterCrafter();
    Vector3Int posTemp;
    GridDir buildDir;

    public void OnClick(Vector3Int pos)
    {
        BuildInserter(pos);
    }
    public void OnMove(Vector3Int pos)
    {
        if (posTemp != pos)
        {
            posTemp = pos;
            CheckBuildInserter(pos);
        }
    }
    public void OnKeyDown(KeyCode key)//건설 방향을 바꿈
    {
        if(key == KeyCode.R)
        {
            buildDir++;
            if ((int)buildDir >= 4)
            {
                buildDir = 0;
            }
            CheckBuildInserter(posTemp);
        }
    }
    public void SetBuildingData(BuildingData buildingData)
    {
        CheckBuildInserter(posTemp);
    }
    public void DeActive()
    {
        inserterCrafter.DeActive();
    }

    void CheckBuildInserter(Vector3Int mouseNode)
    {
        inserterCrafter.CheckBuildInserter(mouseNode, buildDir);
    }
    void BuildInserter(Vector3Int lastNode)
    {
        inserterCrafter.BuildInserter(lastNode, buildDir);
    }
}
