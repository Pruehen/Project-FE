using EnumTypes;
using UnityEngine;

public class BuildingNode : Node
{
    public override Node PreviousNode { get { return null; } set { } }
    public override Node NextNode { get { return null; } set { } }

    Building building;
    IModule module;
    public BuildingNode(Building building, Vector3Int gridPos)
    {
        this.nodeType = NodeType.BuildingNode;
        this.building = building;
        this.gridPos = gridPos;
    }

    public override void Init()
    {
        building.Init();
        building.Register_OnDismantle(Remove);

        transporter = building.Transporter;
        module = building.MainModule;

        if(module is InserterModule)
        {
            InserterModule inserterModule = module as InserterModule;
            inserterModule.SetInserterPart(1);

            GameLogicManager.Instance.InserterNodeSet.Add(this);
        }
    }

    public override void Remove()
    {
        if (building != null)
        {
            ObjectPoolManager.Instance.EnqueueObject(building.gameObject);
        }
        building = null;
        transporter = null;

        if (module is InserterModule)
        {
            InserterModule inserterModule = module as InserterModule;            
            GameLogicManager.Instance.InserterNodeSet.Remove(this);
        }
        module = null;        

        GridMap.Remove_Dic_BuildingDepth(this.gridPos);
    }
}

public class BuildingCrafter
{
    Vector3Int center;
    Quaternion dir;

    public void BuildBuilding(Vector3Int centerNode, GridDir gridDir)
    {
        CheckBuildPosition(centerNode, gridDir);        

        BuildingNode createNode = GridMap.CreateBuildingNode(BuildingManager.Instance.Prefab_Building, center, dir);
        createNode?.Init();
    }

    public void CheckBuildPosition(Vector3Int centerNode, GridDir gridDir)
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
        buildingCrafter.CheckBuildPosition(mouseNode, buildDir);
    }
    void BuildBuilding(Vector3Int lastNode)
    {
        buildingCrafter.BuildBuilding(lastNode, buildDir);
    }
}

