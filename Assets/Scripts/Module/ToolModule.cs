using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using TMPro;

public class ToolModule : MonoBehaviour
{
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
            SetBuildMode(BuildMode.None);
        }
        else
        {
            selectedToolTemp = buildingId;

            BuildingData buildingData = JsonDataManager.GetBuilding(buildingId);
            if (buildingData.BuildingType == BuildingType.Conveying)
            {
                SetBuildMode(BuildMode.Belt);
            }
            else if (buildingData.BuildingType == BuildingType.Inserter)
            {
                SetBuildMode(BuildMode.Inserter);
            }
            else
            {
                SetBuildMode(BuildMode.None);
            }
        }
    }
    
    public void SetBuildMode(BuildMode value)
    {
        this.BuildMode = value;

        GridRenderer.Instance.Command_HideAllGridLines();
        BeltManager.Instance.DeActive();

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
                SelectTool = null;
                break;
            default:
                SelectTool = null;
                break;
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
}
