using System;
using UnityEngine;

public class MinerModule : MonoBehaviour, IModule
{
    MinerModuleModel model;

    [SerializeField] float MiningTimeGain = 1;
    [SerializeField] float MiningSpeedGain = 1;

    IWindow window;
    public void Active_Wdw()
    {
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_MinerModuleUIWdw, this);
            model.OutputInventory.OnOpen();
        }
    }
    public void Close_Wdw()
    {
        if (window != null)
        {
            window.Close();
            window = null;
            model.OutputInventory.OnClose();
        }
    }
    public Inventory TryGetInputInventory()
    {
        return null;
    }
    public Inventory TryGetOutputInventory()
    {
        return model.OutputInventory;
    }

    private void Awake()
    {
        model = ModelManager.NewModel<MinerModuleModel>(this.gameObject.GetInstanceID());
        model.Init_ExtractItem("Item_Iron");
    }

    private void Update()
    {
        model.ExecuteLogic(Time.deltaTime);
    }
}

public class MinerModuleModel
{    
    public Inventory OutputInventory { get; private set; }
    ItemData extractItem;

    float craftingTime = 1;
    float craftingTimeValue;
    bool _isCrafting = true;

    Action<float, float> OnExecuteLogic;
    public void Register_OnExecuteLogic(Action<float, float> callBack)
    {
        OnExecuteLogic += callBack;
    }
    public void UnRegister_OnExecuteLogic(Action<float, float> callBack)
    {
        OnExecuteLogic -= callBack;
    }

    public void RefreshVM_OnWdwActive(Action<Inventory, ItemData> callBack)
    {
        callBack.Invoke(OutputInventory, extractItem);
    }

    public MinerModuleModel()
    {        
        OutputInventory = new Inventory(1, true, EnumTypes.InventoryType.Output);
        
        OutputInventory.OnInventoryChange += SetIsCraftItem_OnInventoryChange;
    }
    public void Init_ExtractItem(string itemKey)
    {
        extractItem = JsonDataManager.GetItem(itemKey);
        OutputInventory.CellDataList[0].SetItem(extractItem.Id_UShort);

        SetIsCraftItem_OnInventoryChange();
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
        if (extractItem == null)
        {
            Debug.Log("√§±º«“ æ∆¿Ã≈€¿Ã æ¯Ω¿¥œ¥Ÿ.");
            return;
        }

        OutputInventory.AddItem(extractItem.Id_UShort, 1, out int remaining);

        Debug.Log("√§±º º∫∞¯");
    }

    void SetIsCraftItem_OnInventoryChange()
    {
        if (extractItem == null)
        {
            _isCrafting = false;
        }
        else
        {
            _isCrafting = true;

            if (OutputInventory.CellDataList[0].CanItemAdd() == false)
            {
                _isCrafting = false;
                Debug.Log("æ∆øÙ«≤¿Ã ∞°µÊ √°Ω¿¥œ¥Ÿ.");
                return;
            }
        }
    }
}