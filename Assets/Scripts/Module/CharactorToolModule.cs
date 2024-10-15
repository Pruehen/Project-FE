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

    //[SerializeField] List<SelectableItemCell> selectableItemCellList;
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
        //for (int i = 0; i < selectableItemCellList.Count; i++)
        //{
        //    selectableItemCellList[i].Register_OnClick_CallBackBuilding(ToolSelect_OnSelectableCellClick);
        //    selectableItemCellList[i].SetData_StaticCell(tool_buildingIdList[i], i + 1);
        //}
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
            Command_ToolDeActive("툴을 비활성화합니다");
        }
        else
        {
            BuildingData buildingData = JsonDataManager.GetBuilding(buildingId);
            ItemData itemData = JsonDataManager.GetItem(buildingId.Replace_ToItem());
            if(Inventory.CanUseItem(itemData.Id_UShort))
            {
                selectedToolTemp = buildingId;
                if (buildingId.Contains("Building_Belt"))
                {
                    SetBuildMode(BuildMode.Belt, buildingId);
                }
                else
                {
                    SetBuildMode(BuildMode.Building, buildingId);
                }
            }
            else
            {
                Command_ToolDeActive("인벤토리에 필요한 아이템이 없습니다");
            }
        }
    }
    
    public void SetBuildMode(BuildMode value, string buildingId)//툴 설정 메서드 : 건설 모드로 진입함
    {
        this.BuildMode = value;

        switch (BuildMode)
        {
            case BuildMode.None:
                SelectTool = null;
                break;
            case BuildMode.Belt:
                SelectTool = BeltManager.Instance;
                break;
            case BuildMode.Building:
                SelectTool = BuildingManager.Instance;
                break;
            default:
                SelectTool = null;
                break;
        }

        if(SelectTool != null)
        {
            SelectTool.SetBuildingData(JsonDataManager.GetBuilding(buildingId));
            testText_BuildMode.text = JsonDataManager.GetItem(buildingId.Replace_ToItem()).Name.GetTextTable();
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

    public void Command_ToolDeActive(string msg)
    {
        Debug.Log(msg);
        selectedToolTemp = null;
        SetBuildMode(BuildMode.None, null);
    }
}
