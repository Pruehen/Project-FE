using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingModule : MonoBehaviour, IModule
{
    Building _building;
    RecipyData _craftingRecipyData;
    public RecipyData CraftingRecipyData
    {
        get { return _craftingRecipyData; }
        set
        { 
            _craftingRecipyData = value;
            OnRecipyChanged?.Invoke(_craftingRecipyData);
        }
    }    

    public Action<RecipyData> OnRecipyChanged;
    public Action<List<CellData>> OnInputCellChanged;
    public Action<List<CellData>> OnOutputCellChanged;

    public void GetData(out List<CellData> inputCellData, out List<CellData> outputCellData)
    {
        inputCellData = _inputItemCellList;
        outputCellData = _outputItemCellList;
    }

    public void RefreshView()
    {
        OnRecipyChanged?.Invoke(_craftingRecipyData);
        OnInputCellChanged?.Invoke(_inputItemCellList);
        OnOutputCellChanged?.Invoke(_outputItemCellList);
    }

    public int InputItemTypeNum { get; private set; }
    public int OutputItemTypeNum { get; private set; }

    float _craftingTime = 1;
    [SerializeField] float CraftingTimeGain = 1;
    float _craftingTimeValue;
    bool _isCrafting = true;

    List<CellData> _inputItemCellList = new List<CellData>();
    List<int> _inputItemRequiredList = new List<int>();
    List<CellData> _outputItemCellList = new List<CellData>();
    List<int> _outputItemRequiredList = new List<int>();

    IWindow window;
    public void Active_Wdw()
    {
        window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CraftingModuleUIWdw, this);
    }
    public void Close_Wdw()
    {
        if (window != null)
        {
            window.Close();
        }
    }

    public void SetIsCrafting_OnStageChange()
    {
        _isCrafting = true;
    }

    void UpdateCraftingTime()
    {
        _craftingTime = CraftingRecipyData.CraftingTime / (_building.BuildingData.SpeedEfficiency * CraftingTimeGain);
    }
    public float GetCraftingTimeRatio()
    {
        return _craftingTimeValue / _craftingTime;
    }

    public void SetCraftingRecipyData(string key)
    {
        RemoveCraftingRecipyData();

        RecipyData data = JsonDataManager.GetRecipyData(key);
        if (data == null)
        {
            Debug.LogError($"잘못된 키가 입력되었습니다 : {key}");
            return;
        }

        CraftingRecipyData = data;       

        if (_building == null)
        {
            _building = GetComponent<Building>();
        }

        SetCraftModule();
    }

    public void RemoveCraftingRecipyData()
    {
        CraftingRecipyData = null;
        SetCraftModule();
    }

    void SetCraftModule()
    {
        if (CraftingRecipyData != null)
        {
            InputItemTypeNum = CraftingRecipyData.InputItemGroup.Count;
            OutputItemTypeNum = CraftingRecipyData.OutputItemGroup.Count;
            UpdateCraftingTime();

            for (int i = 0; i < InputItemTypeNum; i++)
            {
                _inputItemCellList.Add(new CellData(CraftingRecipyData.InputItemGroup[i].Id, true));
                _inputItemRequiredList.Add(CraftingRecipyData.InputItemGroup[i].Count);
            }
            for (int i = 0; i < OutputItemTypeNum; i++)
            {
                _outputItemCellList.Add(new CellData(CraftingRecipyData.OutputItemGroup[i].Id, true));
                _outputItemRequiredList.Add(CraftingRecipyData.OutputItemGroup[i].Count);
            }
        }
        else
        {
            InputItemTypeNum = 0;
            OutputItemTypeNum = 0;
            _craftingTime = 1;

            _inputItemCellList.Clear();
            _inputItemRequiredList.Clear();
            _outputItemCellList.Clear();
            _outputItemRequiredList.Clear();
        }
    }

    void CraftItem()
    {
        if (CraftingRecipyData == null)
        {
            Debug.Log("제작할 레시피가 없습니다.");
            return;
        }

        if(CanCraftItem_InputItemCheck() && CanCraftItem_OutputItemCheck())
        {
            for (int i = 0; i < InputItemTypeNum; i++)
            {
                _inputItemCellList[i].UseItem(_inputItemRequiredList[i]);                
            }            
            for (int i = 0; i < OutputItemTypeNum; i++)
            {
                _outputItemCellList[i].AddItem(_outputItemCellList[i].Id, _outputItemRequiredList[i], out int remaining);                
            }

            OnInputCellChanged?.Invoke(_inputItemCellList);
            OnOutputCellChanged?.Invoke(_outputItemCellList);

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
        for (int i = 0; i < InputItemTypeNum; i++)
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
        for (int i = 0; i < OutputItemTypeNum; i++)
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
        OnInputCellChanged?.Invoke(_inputItemCellList);
    }
    private void Update()
    {
        if (_isCrafting)
        {
            _craftingTimeValue += Time.deltaTime;
        }

        if (_craftingTimeValue > _craftingTime)
        {
            _craftingTimeValue = 0;
            CraftItem();
        }
    }
}
