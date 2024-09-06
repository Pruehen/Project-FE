using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class InserterNode : Node
{
    Inserter inserterPart;
    public InserterNode(Vector3Int firstPos, Vector3Int lastPos)
    {
        nodeType = NodeType.InserterNode;
        this.gridPos = firstPos;
        //GridMap.NodeDic_InteractableDepth.Add(firstPos, this);
        //GridMap.NodeDic_InteractableDepth.Add(lastPos, this);
    }
    public void Init(Vector3Int firstNode, Vector3Int lastNode)
    {
        inserterPart = ObjectPoolManager.Instance.DequeueObject(InserterManager.Instance.Prefab_inserterPart).GetComponent<Inserter>();
        inserterPart.Init(firstNode, lastNode);

        if (GridMap.NodeDic_NormalDepth.ContainsKey(firstNode))
        {

        }
        if (GridMap.NodeDic_NormalDepth.ContainsKey(lastNode))
        {

        }
    }
}

public class InserterCrafter
{
    Vector3Int _firstNode;
    Vector3Int _lastNode;

    // 경로를 저장할 리스트
    List<Vector3Int> path = new List<Vector3Int>();

    public void BuildInserter(Vector3Int lastNode)
    {
        CheckBuildInserter(lastNode);
        BuildLineRenderer.Instance.HideAllGridLinesAndNodes();

        InserterNode inserterNode = new InserterNode(_firstNode, _lastNode);
        inserterNode.Init(_firstNode, _lastNode);
    }

    public void StartBuildInserter(Vector3Int firstNode)
    {
        _firstNode = firstNode;
    }
    public void CheckBuildInserter(Vector3Int lastNode)
    {
        _lastNode = lastNode;
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

    bool isBuildMode = false;

    public void OnClick(Vector3Int pos)
    {
        if (isBuildMode == false)
        {
            isBuildMode = true;
            StartBuildInserter(pos);
        }
        else
        {
            isBuildMode = false;
            BuildInserter(pos);
        }
    }
    public void OnMove(Vector3Int pos)
    {
        if (isBuildMode == true && posTemp != pos)
        {
            posTemp = pos;
            CheckBuildInserter(pos);
        }
    }
    public void DeActive()
    {
        inserterCrafter.DeActive();
        isBuildMode = false;
    }

    void StartBuildInserter(Vector3Int firstNode)
    {        
        inserterCrafter.StartBuildInserter(firstNode);
    }
    void CheckBuildInserter(Vector3Int lastNode)
    {
        inserterCrafter.CheckBuildInserter(lastNode);
    }
    void BuildInserter(Vector3Int lastNode)
    {
        inserterCrafter.BuildInserter(lastNode);
    }
}
