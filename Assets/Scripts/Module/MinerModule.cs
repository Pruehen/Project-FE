using System;
using UnityEngine;

public class MinerModule : MonoBehaviour, IModule, ITransporter
{
    MinerModuleModel model;

    [SerializeField] float MiningTimeGain = 1;
    [SerializeField] float MiningSpeedGain = 1;

    IWindow window;

    #region IModule 인터페이스 구현부
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
    #endregion

    #region ITransporter 인터페이스 구현부
    public bool CanItemOut(ITransporter nextNode)
    {
        Inventory outputinventory = TryGetOutputInventory();
        if (outputinventory != null)
        {
            return outputinventory.CanGrabItem();
        }
        else
        {
            return false;
        }
    }
    public void ItemOut(ITransporter nextNode)
    {
        Inventory outputinventory = TryGetOutputInventory();
        if (outputinventory != null)
        {
            outputinventory.GrabItem(out ushort itemId, out int itemCount);
            nextNode.ItemIn(itemId, Vector3.zero);
        }
    }

    public bool CanItemIn(ushort itemId)
    {
        Inventory inputInventory = TryGetInputInventory();
        if (inputInventory != null)
        {
            return inputInventory.CanAddItem(itemId, 1);
        }
        else
        {
            return false;
        }
    }

    public void ItemIn(ushort itemId, Vector3 inPos)
    {
        Inventory inputInventory = TryGetInputInventory();
        if (inputInventory != null)
        {
            inputInventory.AddItem(itemId, 1, out int r);
        }
    }
    public ushort GetItem()
    {
        Inventory outputinventory = TryGetOutputInventory();
        if (outputinventory != null)
        {
            return outputinventory.GetNextGrabItem();
        }
        else
        {
            return 0;
        }
    }

    public void LogicInit() { Debug.Log("구현되지 않은 메서드를 호출했습니다."); }
    public void ExcuteLogic_OnUpdate(float deltaTime) { Debug.Log("구현되지 않은 메서드를 호출했습니다."); }
    #endregion

    private void Awake()
    {
        model = ModelManager.NewModel<MinerModuleModel>(this.gameObject.GetInstanceID());
        model.Init_ExtractItem("Item_Iron");
        model.Register_OnExtract(OnExtract);
    }

    private void Update()
    {
        model.ExecuteLogic(Time.deltaTime);
    }

    void OnExtract()
    {

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
    Action OnExtract;
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
    public void Register_OnExtract(Action callBack)
    {
        OnExtract += callBack;
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
            ExtractItem();
        }

        OnExecuteLogic?.Invoke(craftingTimeValue, craftingTime);
    }

    void ExtractItem()
    {
        if (extractItem == null)
        {
            Debug.Log("채굴할 아이템이 없습니다.");
            return;
        }

        OutputInventory.AddItem(extractItem.Id_UShort, 1, out int remaining);

        //Debug.Log("채굴 성공");
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
                Debug.Log("아웃풋이 가득 찼습니다.");
                return;
            }
        }
    }
}