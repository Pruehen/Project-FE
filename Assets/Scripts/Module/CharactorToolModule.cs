using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using TMPro;

public class CharactorToolModule : MonoBehaviour
{
    Charactor _charactor;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
    }

    [SerializeField] List<SelectableItemCell> selectableItemCellList;
    [SerializeField] List<string> tool_buildingIdList;

    [SerializeField] TextMeshProUGUI testText_BuildMode;

    public BuildMode BuildMode {  get; private set; }

    IBuildTool _selectTool;
    public IBuildTool SelectTool
    {
        get { return _selectTool; }
        private set
        {
            if (_selectTool != null)
            {
                GridRenderer.Instance.Command_HideAllGridLines();
                _selectTool.DeActive();
                testText_BuildMode.text = "";
            }
            _selectTool = value;            
        }
    }
    string selectedToolTemp = null;

    Inventory _inventory;
    public Inventory Inventory
    {
        get 
        { 
            if(_inventory == null)
            {
                _inventory = GetComponent<InventoryModule>().Inventory; 
            }
            return _inventory; 
        }
    }    

    private void Awake()
    {
        for (int i = 0; i < selectableItemCellList.Count; i++)
        {
            selectableItemCellList[i].Register_OnClick_CallBackBuilding(ToolSelect_OnSelectableCellClick);
            selectableItemCellList[i].SetData_StaticCell(tool_buildingIdList[i], i + 1);
        }
    }

    public void ToolSelect_OnSelectableCellClick(string buildingId)
    {
        ToolSelect(buildingId);
    }
    public void ToolSelect_OnNumKeyClick(int index)
    {
        ToolSelect(tool_buildingIdList[index]);
    }

    void ToolSelect(string buildingId)
    {
        if (selectedToolTemp == buildingId)
        {
            selectedToolTemp = null;
            SetBuildMode(BuildMode.None, null);
        }
        else
        {
            selectedToolTemp = buildingId;

            BuildingData buildingData = JsonDataManager.GetBuilding(buildingId);
            if (buildingData.BuildingType == BuildingType.Conveying)
            {
                SetBuildMode(BuildMode.Belt, buildingId);
            }
            else if (buildingData.BuildingType == BuildingType.Inserter)
            {
                SetBuildMode(BuildMode.Inserter, buildingId);
            }
            else if (buildingData.BuildingType == BuildingType.Mining || buildingData.BuildingType == BuildingType.Crafting || buildingData.BuildingType == BuildingType.Refinery
                || buildingData.BuildingType == BuildingType.Generator || buildingData.BuildingType == BuildingType.Storage)
            {
                SetBuildMode(BuildMode.Inserter, buildingId);
            }
            else
            {
                Debug.LogError("해당 빌딩 타입은 지원되는 빌드 모드가 없습니다.");
                SetBuildMode(BuildMode.None, null);
            }
        }
    }
    
    public void SetBuildMode(BuildMode value, string buildingId)
    {
        this.BuildMode = value;

        switch (BuildMode)
        {
            case BuildMode.None:
                SelectTool = null;
                break;
            case BuildMode.Belt:
                SelectTool = BeltManager.Instance;
                testText_BuildMode.text = "벨트";
                break;
            case BuildMode.Inserter:
                SelectTool = InserterManager.Instance;
                testText_BuildMode.text = "투입기";
                break;
            case BuildMode.Building:
                SelectTool = BuildingManager.Instance;
                testText_BuildMode.text = "건물";
                break;
            default:
                SelectTool = null;
                break;
        }

        if(SelectTool != null)
        {
            SelectTool.SetBuildingId(buildingId);
        }
    }

    public void ToolOnClick(Vector3Int gridPos)
    {
        SelectTool.OnClick(gridPos);
    }
    public void ToolOnMove(Vector3Int gridPos)
    {
        GridRenderer.Instance.DrawGrid(gridPos);        
        SelectTool.OnMove(gridPos);
    }
    public void ToolOnKeyDown(KeyCode key)
    {
        SelectTool.OnKeyDown(key);
    }
}
