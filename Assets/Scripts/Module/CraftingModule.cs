using System.Collections.Generic;
using UnityEngine;

public class CraftingModule : MonoBehaviour
{
    Building _building;
    RecipyData _craftingRecipyData;

    int _inputItemTypeNum;
    int _outputItemTypeNum;
    float _craftingTime = 1;
    [SerializeField] float CraftingTimeGain = 1;
    public void UpdateCraftingTime()
    {
        _craftingTime = _craftingRecipyData.CraftingTime / (_building.BuildingData.SpeedEfficiency * CraftingTimeGain);
    }
    float _craftingTimeValue;

    List<CellData> _inputItemCellList = new List<CellData>();
    List<int> _inputItemRequiredList = new List<int>();
    List<CellData> _outputItemCellList = new List<CellData>();
    List<int> _outputItemRequiredList = new List<int>();

    public void SetCraftingRecipyData(string key)
    {
        RemoveCraftingRecipyData();

        RecipyData data = JsonDataManager.GetRecipyData(key);
        if (data == null)
        {
            Debug.LogError($"잘못된 키가 입력되었습니다 : {key}");
            return;
        }

        _craftingRecipyData = data;       

        if (_building == null)
        {
            _building = GetComponent<Building>();
        }

        SetCraftModule();
    }

    public void RemoveCraftingRecipyData()
    {
        _craftingRecipyData = null;
        SetCraftModule();
    }

    void SetCraftModule()
    {
        if (_craftingRecipyData != null)
        {
            _inputItemTypeNum = _craftingRecipyData.InputItemGroup.Count;
            _outputItemTypeNum = _craftingRecipyData.OutputItemGroup.Count;
            UpdateCraftingTime();

            for (int i = 0; i < _inputItemTypeNum; i++)
            {
                _inputItemCellList.Add(new CellData(_craftingRecipyData.InputItemGroup[i].Id, true));
                _inputItemRequiredList.Add(_craftingRecipyData.InputItemGroup[i].Count);
            }
            for (int i = 0; i < _outputItemTypeNum; i++)
            {
                _outputItemCellList.Add(new CellData(_craftingRecipyData.OutputItemGroup[i].Id, true));
                _outputItemRequiredList.Add(_craftingRecipyData.OutputItemGroup[i].Count);
            }
        }
        else
        {
            _inputItemTypeNum = 0;
            _outputItemTypeNum = 0;
            _craftingTime = 1;

            _inputItemCellList.Clear();
            _inputItemRequiredList.Clear();
            _outputItemCellList.Clear();
            _outputItemRequiredList.Clear();
        }
    }

    void CraftItem()
    {
        if (_craftingRecipyData == null)
        {
            Debug.Log("제작할 레시피가 없습니다.");
            return;
        }

        if(CanCraftItem_InputItemCheck() && CanCraftItem_OutputItemCheck())
        {
            for (int i = 0; i < _inputItemTypeNum; i++)
            {
                _inputItemCellList[i].UseItem(_inputItemRequiredList[i]);
            }
            for (int i = 0; i < _outputItemTypeNum; i++)
            {
                _outputItemCellList[i].AddItem(_outputItemCellList[i].Id, _outputItemRequiredList[i], out int remaining);
            }

            Debug.Log("제작 성공");
        }
        else
        {
            Debug.Log("제작 실패");
        }
    }

    public bool CanCraftItem_InputItemCheck()
    {
        bool canCraftItem = true;
        for (int i = 0; i < _inputItemTypeNum; i++)
        {
            if (_inputItemCellList[i].Count < _inputItemRequiredList[i])
            {
                canCraftItem = false;
                Debug.Log("인풋 아이템이 부족합니다.");
                break;
            }
        }        
        return canCraftItem;
    }
    public bool CanCraftItem_OutputItemCheck()
    {
        bool canCraftItem = true;
        for (int i = 0; i < _outputItemTypeNum; i++)
        {
            if (_outputItemCellList[i].CanItemAdd() == false)
            {
                canCraftItem = false;
                Debug.Log("아웃풋이 가득 찼습니다.");
                break;
            }
        }

        return canCraftItem;
    }

    private void Start()
    {
        SetCraftingRecipyData("Recipy_IronPlate");

        foreach (var item in _inputItemCellList)
        {
            item.AddItem(100);
        }
    }
    private void Update()
    {
        _craftingTimeValue += Time.deltaTime;
        if (_craftingTimeValue > _craftingTime)
        {
            _craftingTimeValue = 0;
            CraftItem();
        }
    }
}
