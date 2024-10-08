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

    public void BuildBuilding(Vector3Int centerNode, GridDir gridDir, BuildingData data)
    {
        ushort buildingItemId = JsonDataManager.GetItem(data.Id.Replace_ToItem()).Id_UShort;
        if (Player.Instance.CanUseItem(buildingItemId) == false)//인벤토리에 아이템이 없거나 키가 잘못되었을 경우
        {
            Command_ToolDeActive("아이템이 부족합니다");
            return;
        }

        CheckBuildPosition(centerNode, gridDir, data);        

        BuildingNode createNode = GridMap.CreateBuildingNode(data.GetBuildingPrefab(), center, dir);

        if (createNode != null)//빌딩 노드 생성에 성공했을 경우
        {
            createNode.Init();
            Player.Instance.UseItem(buildingItemId);

            if (Player.Instance.CanUseItem(buildingItemId) == false)//인벤토리에 아이템이 없거나 키가 잘못되었을 경우
            {
                Command_ToolDeActive("");
            }
        }
    }

    public void CheckBuildPosition(Vector3Int centerNode, GridDir gridDir, BuildingData data)
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

        BuildMeshRenderer.Instance.DrawMesh(center, dir, data.GetBuildingPrefab());
    }
    public void DeActive()
    {
        BuildMeshRenderer.Instance.RemoveMesh();
    }

    void Command_ToolDeActive(string msg)
    {
        Player.Instance.ControlledCharactor.builtIn_ToolModule.Command_ToolDeActive(msg);
    }
}

public class BuildingManager : SceneSingleton<BuildingManager>, IBuildTool
{
    BuildingData selectedBuildingData;

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
        selectedBuildingData = buildingData;
        CheckBuillBuilding(posTemp);
    }
    public void DeActive()
    {
        buildingCrafter.DeActive();
    }

    void CheckBuillBuilding(Vector3Int mouseNode)
    {
        buildingCrafter.CheckBuildPosition(mouseNode, buildDir, selectedBuildingData);
    }
    void BuildBuilding(Vector3Int lastNode)
    {
        buildingCrafter.BuildBuilding(lastNode, buildDir, selectedBuildingData);
    }
}

