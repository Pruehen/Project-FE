using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class InserterNode
{
    public Vector3Int gridPos { get; private set; }
    public BeltNode PreviousNode { get; set; }
    public BeltNode NextNode { get; set; }    

    public InserterNode(Vector3Int gridPos)
    {
        this.gridPos = gridPos;
        GridMap.inserterDic.Add(gridPos, this);
    }
    public void Init()
    {

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

        InserterNode inserterNode = new InserterNode(lastNode);
        inserterNode.Init();
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
