using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingModule : MonoBehaviour, IModule
{
    CraftingModuleModel model;

    [SerializeField] float CraftingTimeGain = 1;
    [SerializeField] float CraftingSpeedGain = 1;
    
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

    private void Awake()
    {
        model = ModelManager.NewModel<CraftingModuleModel>(this.gameObject.GetInstanceID());
        model.Init_RecipyGroupKey(GetComponent<Building>().BuildingData.RecipyGroup);

        //model.SetCraftingRecipyData("Recipy_IronPlate");        
    }

    private void Update()
    {
        model.ExecuteLogic(Time.deltaTime);
    }
}

public class CraftingModuleModel
{
    RecipyData _craftingRecipyData;
    RecipyData CraftingRecipyData
    {
        get { return _craftingRecipyData; }
        set
        {
            if( _craftingRecipyData != value )
            {
                _craftingRecipyData = value;
                SetCraftModule(_craftingRecipyData);
            }
        }
    }

    List<string> recipyDataGroupList;

    Inventory inputInventory;
    Inventory outputInventory;

    float craftingTime = 1;
    float craftingTimeValue;
    bool _isCrafting = true;

    float _craftingTimeGain = 1;
    float _craftingSpeedGain = 1;

    Action<RecipyData, Inventory, Inventory> OnSetCraftingRecipyData;
    public void Register_OnSetCraftingRecipyData(Action<RecipyData, Inventory, Inventory> callBack)
    {
        OnSetCraftingRecipyData += callBack;
        OnSetCraftingRecipyData?.Invoke(CraftingRecipyData, inputInventory, outputInventory);
    }
    public void UnRegister_OnSetCraftingRecipyData(Action<RecipyData, Inventory, Inventory> callBack)
    {
        OnSetCraftingRecipyData -= callBack;
    }

    Action<float, float> OnExecuteLogic;
    public void Register_OnExecuteLogic(Action<float, float> callBack)
    {
        OnExecuteLogic += callBack;
        OnExecuteLogic?.Invoke(craftingTimeValue, craftingTime);
    }
    public void UnRegister_OnExecuteLogic(Action<float, float> callBack)
    {
        OnExecuteLogic -= callBack;
    }
    public CraftingModuleModel()
    {
        inputInventory = new Inventory(4, true);
        outputInventory = new Inventory(4, true);

        inputInventory.OnInventoryChange += SetIsCraftItem_OnInventoryChange;
        outputInventory.OnInventoryChange += SetIsCraftItem_OnInventoryChange;
        SetIsCraftItem_OnInventoryChange();
    }
    public void Init_RecipyGroupKey(string initKey)
    {
        recipyDataGroupList = JsonDataManager.GetRecipyGroupData(initKey);
    }

    void UpdateCraftingTime(RecipyData recipyData)
    {
        if (recipyData != null)
        {
            craftingTime = _craftingTimeGain * recipyData.CraftingTime / _craftingSpeedGain;
        }
        else
        {
            craftingTime = 1;
        }
    }

    public void SetCraftingRecipyData(string key)
    {
        if(key == null)
        {
            CraftingRecipyData = null;            
        }
        else
        {
            CraftingRecipyData = JsonDataManager.GetRecipyData(key);
            if (CraftingRecipyData == null)
            {
                Debug.LogError($"잘못된 키가 입력되었습니다 : {key}");                
            }            
        }
        OnSetCraftingRecipyData?.Invoke(CraftingRecipyData, inputInventory, outputInventory);
    }

    void SetCraftModule(RecipyData recipyData)
    {
        inputInventory.Clear();
        outputInventory.Clear();

        if (recipyData != null)
        {
            for (int i = 0; i < recipyData.InputItemGroup.Count; i++)
            {
                inputInventory.CellDataList[i].SetItem(recipyData.InputItemGroup[i].Id);
            }
            for (int i = 0; i < recipyData.OutputItemGroup.Count; i++)
            {
                outputInventory.CellDataList[i].SetItem(recipyData.OutputItemGroup[i].Id);
            }
        }

        UpdateCraftingTime(recipyData);
    }


    public void ExecuteLogic(float deltaTime)
    {
        if (_isCrafting == false)
        {
            return;            
        }

        craftingTimeValue += deltaTime;

        if (craftingTimeValue > craftingTime)
        {
            craftingTimeValue -= craftingTime;
            CraftItem();
        }

        OnExecuteLogic?.Invoke(craftingTimeValue, craftingTime);
    }

    void CraftItem()
    {
        if (_craftingRecipyData == null)
        {
            Debug.Log("제작할 레시피가 없습니다.");
            return;
        }

        for (int i = 0; i < _craftingRecipyData.InputItemGroup.Count; i++)
        {
            inputInventory.UseItem_FixedInventory(_craftingRecipyData.InputItemGroup[i].Id, _craftingRecipyData.InputItemGroup[i].Count);
        }
        for (int i = 0; i < _craftingRecipyData.OutputItemGroup.Count; i++)
        {
            outputInventory.AddItem(_craftingRecipyData.OutputItemGroup[i].Id, _craftingRecipyData.OutputItemGroup[i].Count, out int remaining);
        }

        Debug.Log("제작 성공");
    }

    void SetIsCraftItem_OnInventoryChange()
    {
        if (CraftingRecipyData == null)
        {
            _isCrafting = false;
        }
        else
        {
            _isCrafting = true;
            for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
            {
                if (inputInventory.CanUseItem(CraftingRecipyData.InputItemGroup[i].Id, CraftingRecipyData.InputItemGroup[i].Count) == false)
                {
                    _isCrafting = false;
                    Debug.Log("인풋 아이템이 부족합니다.");
                    return;
                }
            }

            for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
            {
                if (outputInventory.CellDataList[i].CanItemAdd() == false)
                {
                    _isCrafting = false;
                    Debug.Log("아웃풋이 가득 찼습니다.");
                    return;
                }
            }
        }
    }
}