using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolModule : MonoBehaviour
{
    [SerializeField] List<SelectableItemCell> selectableItemCellList;
    [SerializeField] List<string> tool_buildingIdList;

    Action<BuildingData> ActiveTool_OnToolSelect;
    public void Register_ActiveTool_OnToolSelect(Action<BuildingData> callBack) { ActiveTool_OnToolSelect = callBack; }

    public bool IsBuildMode {  get; private set; }

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
        ActiveTool_OnToolSelect?.Invoke(JsonDataManager.GetBuilding(buildingId));
    }
    
    public void SetBuildMode(bool value)
    {
        IsBuildMode = value;
    }
}
