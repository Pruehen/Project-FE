using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class BuildingNode : Node
{
    public override Node PreviousNode { get { return null; } set { } }
    public override Node NextNode { get { return null; } set { } }

    public BuildingNode(Vector3Int gridPos, Building building)
    {
        this.nodeType = NodeType.BuildingNode;
        this.gridPos = gridPos;
        this.transporter = building;
    }

    public override void Init()
    {

    }

    public override void Remove()
    {

    }
}

public class BuildingCrafter
{
    Vector3Int center;
    Quaternion dir;

    public void BuildInserter(Vector3Int centerNode, GridDir gridDir)
    {
        CheckBuildInserter(centerNode, gridDir);        

        BuildingNode createNode = GridMap.CreateBuildingNode(centerNode);
        createNode.Init();
    }

    public void CheckBuildInserter(Vector3Int centerNode, GridDir gridDir)
    {
        center = centerNode;

        switch (gridDir)
        {
            case GridDir.Top:
                dir = Quaternion.Euler(0, 0, 0);
                break;
            case GridDir.Right:
                dir = Quaternion.Euler(0, 90, 0);
                break;
            case GridDir.Bottom:
                dir = Quaternion.Euler(0, 180, 0);
                break;
            case GridDir.Left:
                dir = Quaternion.Euler(0, 270, 0);
                break;
            default:
                break;
        }

        BuildMeshRenderer.Instance.DrawMesh(center, dir, BuildingManager.Instance.Prefab_Building);
    }
    public void DeActive()
    {
        BuildMeshRenderer.Instance.RemoveMesh();
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
        BuildBuilding(pos);
    }
    public void OnMove(Vector3Int pos)
    {
        if (posTemp != pos)
        {
            posTemp = pos;
            CheckBuillBuilding(pos);
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
            CheckBuillBuilding(posTemp);
        }
    }
    public void SetBuildingData(BuildingData buildingData)
    {
        Prefab_Building = buildingData.GetBuildingPrefab();
        CheckBuillBuilding(posTemp);
    }
    public void DeActive()
    {
        buildingCrafter.DeActive();
    }

    void CheckBuillBuilding(Vector3Int mouseNode)
    {
        buildingCrafter.CheckBuildInserter(mouseNode, buildDir);
    }
    void BuildBuilding(Vector3Int lastNode)
    {
        buildingCrafter.BuildInserter(lastNode, buildDir);
    }
}

