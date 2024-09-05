using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class ToolModule : MonoBehaviour
{
    [SerializeField] List<SelectableItemCell> selectableItemCellList;
    [SerializeField] List<string> tool_buildingIdList;

    public bool IsBuildMode {  get; private set; }
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
            SetBuildMode(false);
        }
        else
        {
            Debug.Log("Åø ¼¿·ºÆ®");
            selectedToolTemp = buildingId;

            BuildingData buildingData = JsonDataManager.GetBuilding(buildingId);
            if (buildingData.BuildingType == BuildingType.Conveying)
            {
                SetBuildMode(true);
            }
            else
            {
                SetBuildMode(false);
            }
        }
    }
    
    public void SetBuildMode(bool value)
    {
        IsBuildMode = value;
    }
}
