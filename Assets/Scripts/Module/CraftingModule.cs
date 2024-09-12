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
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CraftingModuleUIWdw, this);
        }
    }
    public void Close_Wdw()
    {
        if(window != null)
        {
            window.Close();
            window = null;
        }        
    }
    public Inventory TryGetInputInventory()
    {
        return model.InputInventory;
    }
    public Inventory TryGetOutputInventory()
    {
        return model.OutputInventory;
    }

    public void SetCraftingRecipyData(string recipyKey)
    {
        model.SetCraftingRecipyData(recipyKey);
    }
    private void Awake()
    {
        model = ModelManager.NewModel<CraftingModuleModel>(this.gameObject.GetInstanceID());
        model.Init_RecipyGroupKey(GetComponent<Building>().BuildingData.RecipyGroup);       
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
            }
        }
    }

    List<string> recipyDataGroupList;

    public Inventory InputInventory { get; private set; }
    public Inventory OutputInventory { get; private set; }

    float craftingTime = 1;
    float craftingTimeValue;
    bool _isCrafting = true;

    float _craftingTimeGain = 1;
    float _craftingSpeedGain = 1;

    Action<RecipyData> OnSetCraftingRecipyData;
    public void Register_OnSetCraftingRecipyData(Action<RecipyData> callBack)
    {
        OnSetCraftingRecipyData += callBack;        
    }
    public void UnRegister_OnSetCraftingRecipyData(Action<RecipyData> callBack)
    {
        OnSetCraftingRecipyData -= callBack;
    }

    Action<float, float> OnExecuteLogic;
    public void Register_OnExecuteLogic(Action<float, float> callBack)
    {
        OnExecuteLogic += callBack;        
    }
    public void UnRegister_OnExecuteLogic(Action<float, float> callBack)
    {
        OnExecuteLogic -= callBack;
    }

    public void RefreshVM_OnWdwActive(Action<RecipyData, Inventory, Inventory, List<string>> callBack)
    {
        callBack.Invoke(CraftingRecipyData, InputInventory, OutputInventory, recipyDataGroupList);
    }

    public CraftingModuleModel()
    {
        InputInventory = new Inventory(4, true);
        OutputInventory = new Inventory(4, true);

        InputInventory.OnInventoryChange += SetIsCraftItem_OnInventoryChange;
        OutputInventory.OnInventoryChange += SetIsCraftItem_OnInventoryChange;

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

    public void SetCraftingRecipyData(string recipyKey)
    {
        if(recipyKey == null)
        {
            CraftingRecipyData = null;            
        }
        else
        {
            CraftingRecipyData = JsonDataManager.GetRecipyData(recipyKey);
            if (CraftingRecipyData == null)
            {
                Debug.LogError($"잘못된 키가 입력되었습니다 : {recipyKey}");                
            }            
        }
        SetCraftModule_OnRecipyChange(CraftingRecipyData);
        OnSetCraftingRecipyData?.Invoke(CraftingRecipyData);
    }

    void SetCraftModule_OnRecipyChange(RecipyData recipyData)
    {
        InputInventory.Clear();
        OutputInventory.Clear();

        if (recipyData != null)
        {
            for (int i = 0; i < recipyData.InputItemGroup.Count; i++)
            {
                InputInventory.CellDataList[i].SetItem(recipyData.InputItemGroup[i].data.Id_Short);
            }
            for (int i = 0; i < recipyData.OutputItemGroup.Count; i++)
            {
                OutputInventory.CellDataList[i].SetItem(recipyData.OutputItemGroup[i].data.Id_Short);
            }
        }

        UpdateCraftingTime(recipyData);
    }


    public void ExecuteLogic(float deltaTime)
    {
        if (_isCrafting == false)
        {
            craftingTimeValue = 0;
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
            InputInventory.UseItem_FixedInventory(_craftingRecipyData.InputItemGroup[i].data.Id_Short, _craftingRecipyData.InputItemGroup[i].Count);
        }
        for (int i = 0; i < _craftingRecipyData.OutputItemGroup.Count; i++)
        {
            OutputInventory.AddItem(_craftingRecipyData.OutputItemGroup[i].data.Id_Short, _craftingRecipyData.OutputItemGroup[i].Count, out int remaining);
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
                if (InputInventory.CanUseItem(CraftingRecipyData.InputItemGroup[i].data.Id_Short, CraftingRecipyData.InputItemGroup[i].Count) == false)
                {
                    _isCrafting = false;
                    Debug.Log("인풋 아이템이 부족합니다.");
                    return;
                }
            }

            for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
            {
                if (OutputInventory.CellDataList[i].CanItemAdd() == false)
                {
                    _isCrafting = false;
                    Debug.Log("아웃풋이 가득 찼습니다.");
                    return;
                }
            }
        }
    }
}