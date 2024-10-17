using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] List<SelectableItemCell> SelectableCell_QuickSlotList;

    public void Command_GetCellData_BuildingId(int index)
    {
        CellData cellData = SelectableCell_QuickSlotList[index].CellData;
        if (cellData == null)
        {
            Player.Instance.Command_ToolSelect_SetBuildingId(null);
            return;
        }

        ushort itemId = cellData.Id;
        ItemData itemData = JsonDataManager.GetItem(itemId);

        string buildingId = itemData.Id.Replace_ToBuilding();        
        if(JsonDataManager.GetBuilding(buildingId) != null)
        { 
            Player.Instance.Command_ToolSelect_SetBuildingId(buildingId);
        }
    }
    public void Command_RemoveCellData_BuildingId(int index)
    {
        SelectableCell_QuickSlotList[index].SetData_Item(0);
    }
}
