using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingModule : MonoBehaviour, IModule
{
    Building _building;
    List<string> _recipyGroup;

    RecipyData _craftingRecipyData;
    public RecipyData CraftingRecipyData
    {
        get { return _craftingRecipyData; }
        private set
        { 
            _craftingRecipyData = value;
            OnRecipyChanged?.Invoke(_craftingRecipyData);
        }
    }    

    public Action<RecipyData> OnRecipyChanged;

    public void GetData(out Inventory inputInventory, out Inventory outputInventory)
    {
        inputInventory = InputInventory;
        outputInventory = OutputInventory;
    }

    float _craftingTime = 1;
    [SerializeField] float CraftingTimeGain = 1;
    float _craftingTimeValue;
    bool _isCrafting = true;
    public bool IsCrafting
    {
        get { return _isCrafting; }
        set
        {
            _isCrafting = value;
        }
    }

    Inventory _inputInventory;
    Inventory _outputInventory;

    public Inventory InputInventory
    {
        get { return _inputInventory; }
        private set
        {
            _inputInventory = value;
        }
    }

    public Inventory OutputInventory
    {
        get { return _outputInventory; }
        private set
        {
            _outputInventory = value;
        }
    }

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
            UpdateCraftingTime();

            for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
            {
                InputInventory.CellDataList[i].SetItem(CraftingRecipyData.InputItemGroup[i].Id);
            }
            for (int i = 0; i < CraftingRecipyData.OutputItemGroup.Count; i++)
            {
                OutputInventory.CellDataList[i].SetItem(CraftingRecipyData.OutputItemGroup[i].Id);
            }
        }
        else
        {
            _craftingTime = 1;
        }
    }

    void CraftItem()
    {
        if (CraftingRecipyData == null)
        {
            Debug.Log("제작할 레시피가 없습니다.");
            return;
        }

        for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
        {
            InputInventory.UseItem_FixedInventory(CraftingRecipyData.InputItemGroup[i].Id, CraftingRecipyData.InputItemGroup[i].Count);
        }
        for (int i = 0; i < CraftingRecipyData.OutputItemGroup.Count; i++)
        {
            OutputInventory.AddItem(CraftingRecipyData.OutputItemGroup[i].Id, CraftingRecipyData.OutputItemGroup[i].Count, out int remaining);
        }

        Debug.Log("제작 성공");
    }

    public bool CanCraftItem_InputItemCheck()
    {
        bool canCraftItem = true;
        for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
        {
            if(InputInventory.CanUseItem(CraftingRecipyData.InputItemGroup[i].Id, CraftingRecipyData.InputItemGroup[i].Count) == false)
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

        for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
        {
            if (OutputInventory.CellDataList[i].CanItemAdd() == false)
            {
                canCraftItem = false;
                Debug.Log("아웃풋이 가득 찼습니다.");
                break;
            }
        }

        return canCraftItem;
    }

    private void Awake()
    {
        if (_building == null)
        {
            _building = GetComponent<Building>();
        }

        _recipyGroup = JsonDataManager.GetRecipyGroupData(this._building.BuildingData.RecipyGroup);

        InputInventory = new Inventory(4, true);
        OutputInventory = new Inventory(4, true);

        foreach (var item in _recipyGroup)
        {
            Debug.Log(item);
        }

        //테스트 호출
        SetCraftingRecipyData("Recipy_IronPlate");
        InputInventory.AddItem(InputInventory.CellDataList[0].Id, 100, out int remaining);
    }

    private void Update()
    {
        if (IsCrafting)
        {
            _craftingTimeValue += Time.deltaTime;
        }

        if (_craftingTimeValue > _craftingTime)
        {
            _craftingTimeValue = 0;
            CraftItem();
        }

        IsCrafting = (CanCraftItem_InputItemCheck() && CanCraftItem_OutputItemCheck()) == true;
    }
}
