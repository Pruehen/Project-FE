using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCrafter
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

public class BuildingManager : SceneSingleton<BuildingManager>, IBuildTool
{
    public GameObject Prefab_Building { get; private set; }

    BuildingCrafter buildingCrafter = new BuildingCrafter();
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
        if (key == KeyCode.R)
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
        Prefab_Building = buildingData.GetBuildingPrefab();
    }
    public void DeActive()
    {
        buildingCrafter.DeActive();
    }

    void CheckBuildInserter(Vector3Int mouseNode)
    {
        buildingCrafter.CheckBuildInserter(mouseNode, buildDir);
    }
    void BuildInserter(Vector3Int lastNode)
    {
        buildingCrafter.BuildInserter(lastNode, buildDir);
    }
}

