using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class CharactorCraftingModule : MonoBehaviour, IModule
{
    Charactor _charactor;
    public CharactorCraftingModuleModel Model { get; private set; }

    IWindow window;

    bool isInit = false;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
        Model = new CharactorCraftingModuleModel(_charactor.BuiltIn_InventoryModule.Inventory);        

        isInit = true;
    }

    public void Active_Wdw()
    {
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CharactorCraftingModuleUIWdw, this);
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
    public void OnBuildingInit()
    {

    }
    public void OnBuildingDismantle()
    {
        foreach (CellData item in TryGetInputInventory().CellDataList)
        {
            Player.Instance.GetItem(item.Id, item.Count);
        }
        foreach (CellData item in TryGetOutputInventory().CellDataList)
        {
            Player.Instance.GetItem(item.Id, item.Count);
        }
    }
    public Inventory TryGetInputInventory()
    {
        return _charactor.BuiltIn_InventoryModule.Inventory;
    }
    public Inventory TryGetOutputInventory()
    {
        return _charactor.BuiltIn_InventoryModule.Inventory;
    }

    public void Command_TryAddCraftOrder(string recipyDataKey, int count)
    {
        Model.TryAddCraftOrder(JsonDataManager.GetRecipyData(recipyDataKey), count);
    }
    public void Command_OrderCancel(int index)
    {
        Model.OrderCancel(index);
    }

    public void Command_RefreshData()
    {
        Model.RefreshData();
    }

    void Update()
    {
        Model?.ExecuteLogic(Time.deltaTime);
    }
}
public class CraftOrder
{
    public RecipyData RecipyData { get; private set; }
    public int Count { get; private set; }

    public CraftOrder(RecipyData recipyData, int count)
    {
        this.RecipyData = recipyData;
        this.Count = count;
    }

    public void ExecuteOrder()
    {
        Count--;
    }
}

public class CharactorCraftingModuleModel : Extension.VM
{
    List<CraftOrder> _list_CraftOrder = new List<CraftOrder>();

    public Inventory TempInventory { get; private set; }//제작 명령을 넣을 때 아이템을 담아 놓을 임시 공간. 현재 아이템을 제작 중일 때는 반드시 여기에 아이템이 들어있어야 함.
    public Inventory CharactorInventory { get; private set; }

    float craftingTime = 1;
    float craftingTimeValue;
    bool _isCrafting = false;

    float _craftingTimeGain = 1;
    float _craftingSpeedGain = 3;

    float _craftingTimeRatio;
    public float CraftingTimeRatio
    {
        get { return _craftingTimeRatio; }
        set
        {
            if(_craftingTimeRatio != value)
            {
                _craftingTimeRatio = value;
                OnPropertyChanged(nameof(CraftingTimeRatio));
            }
        }
    }

    int _craftOrderCount;
    public int CraftOrderCount
    {
        get { return _craftOrderCount; }
        set
        {
            if( _craftOrderCount != value)
            {
                _craftOrderCount = value;
                OnPropertyChanged(nameof(CraftOrderCount));
            }
        }
    }

    int _remainingCount_FirstOrder;
    public int RemainingCount_FirstOrder
    {
        get { return _remainingCount_FirstOrder; }
        set
        {
            if(_remainingCount_FirstOrder != value)
            {
                _remainingCount_FirstOrder = value;
                OnPropertyChanged(nameof(RemainingCount_FirstOrder));
            }
        }
    }
    public List<CraftOrder> List_CraftOrder
    {
        get 
        {
            return _list_CraftOrder; 
        }
    }

    public void RefreshData()
    {
        _craftingTimeRatio = craftingTimeValue / craftingTime;
        _craftOrderCount = _list_CraftOrder.Count;
        if(_craftOrderCount > 0)
        {
            _remainingCount_FirstOrder = _list_CraftOrder[0].Count;
        }

        OnPropertyChanged(nameof(CraftingTimeRatio));
        OnPropertyChanged(nameof(CraftOrderCount));
        OnPropertyChanged(nameof(RemainingCount_FirstOrder));
        OnPropertyChanged(nameof(List_CraftOrder));
    }

    public CharactorCraftingModuleModel(Inventory charactorIv)
    {
        TempInventory = new Inventory(100, false, InventoryType.Temp);
        CharactorInventory = charactorIv;

        //TempInventory.OnInventoryChange += Set_IsCraftItem;
        CharactorInventory.OnInventoryChange += Set_IsCraftItem;
        Set_IsCraftItem();
    }

    void UpdateCraftingTime(RecipyData recipyData)//현재 레시피를 제작하는 데에 걸리는 시간 설정
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

    public void TryAddCraftOrder(RecipyData recipyData, int count)//제작 명령 추가. **해당 메서드 호출 전에, 미리 인벤토리를 체크할 것
    {
        if (recipyData == null)
        {
            Debug.LogError($"잘못된 키가 입력되었습니다 : {recipyData}");
        }

        for (int i = 0; i < recipyData.InputItemGroup.Count; i++)
        {
            if(CharactorInventory.CanUseItem(recipyData.InputItemGroup[i].data.Id_UShort, recipyData.InputItemGroup[i].Count * count) == false)
            {
                Debug.Log("아이템이 부족합니다");
                return;
            }
        }

        for (int i = 0; i < recipyData.InputItemGroup.Count; i++)
        {
            CharactorInventory.UseItem(recipyData.InputItemGroup[i].data.Id_UShort, recipyData.InputItemGroup[i].Count * count);
            TempInventory.AddItem(recipyData.InputItemGroup[i].data.Id_UShort, recipyData.InputItemGroup[i].Count * count, out int r);
        }

        AddOrder(recipyData, count);

        Set_IsCraftItem();
    }
    void AddOrder(RecipyData craftingRecipyData, int count)
    {
        _list_CraftOrder.Add(new CraftOrder(craftingRecipyData, count));
        OnPropertyChanged(nameof(List_CraftOrder));

        CraftOrderCount = _list_CraftOrder.Count;
    }
    public void OrderCancel(int index)//해당 인덱스의 제작 명령을 취소
    {
        if(_list_CraftOrder.Count > index)
        {
            RecipyData recipyData = _list_CraftOrder[index].RecipyData;
            int count = _list_CraftOrder[index].Count;

            for (int i = 0; i < recipyData.InputItemGroup.Count; i++)
            {
                TempInventory.UseItem(recipyData.InputItemGroup[i].data.Id_UShort, recipyData.InputItemGroup[i].Count * count);
                CharactorInventory.AddItem(recipyData.InputItemGroup[i].data.Id_UShort, recipyData.InputItemGroup[i].Count * count, out int r);
            }

            RemoveOrder(index);

            Set_IsCraftItem();
        }
    }
    void RemoveOrder(int index)
    {
        _list_CraftOrder.RemoveAt(index);
        OnPropertyChanged(nameof(List_CraftOrder));

        CraftOrderCount = _list_CraftOrder.Count;
    }

    public void ExecuteLogic(float deltaTime)//제작 로직 수행
    {
        if (_isCrafting == false)
        {
            craftingTimeValue = 0;
            CraftingTimeRatio = 0;
            return;
        }

        craftingTimeValue += deltaTime;

        if (craftingTimeValue > craftingTime)
        {
            craftingTimeValue -= craftingTime;
            CraftItem();

            Set_IsCraftItem();
        }

        CraftingTimeRatio = craftingTimeValue / craftingTime;
    }

    void CraftItem()
    {
        if (_list_CraftOrder.Count == 0)
        {
            Debug.LogWarning("제작할 레시피가 없습니다.");
            return;
        }

        CraftOrder peekedOrder = _list_CraftOrder[0];

        for (int i = 0; i < peekedOrder.RecipyData.InputItemGroup.Count; i++)
        {
            TempInventory.UseItem(peekedOrder.RecipyData.InputItemGroup[i].data.Id_UShort, peekedOrder.RecipyData.InputItemGroup[i].Count);
        }
        for (int i = 0; i < peekedOrder.RecipyData.OutputItemGroup.Count; i++)
        {
            CharactorInventory.AddItem(peekedOrder.RecipyData.OutputItemGroup[i].data.Id_UShort, peekedOrder.RecipyData.OutputItemGroup[i].Count, out int remaining);
        }

        OrderExecute();

        Debug.Log("제작 성공");
    }

    void OrderExecute()
    {
        CraftOrder peekedOrder = _list_CraftOrder[0];
        peekedOrder.ExecuteOrder();

        if(peekedOrder.Count <= 0)
        {
            _list_CraftOrder.RemoveAt(0);
            OnPropertyChanged(nameof(List_CraftOrder));
            
            CraftOrderCount = _list_CraftOrder.Count;
        }
        else
        {
            RemainingCount_FirstOrder = peekedOrder.Count;
        }
    }    

    void Set_IsCraftItem()//아이템 제작 가능 상태인지 설정함
    {
        if (_list_CraftOrder.Count == 0)
        {
            _isCrafting = false;
        }
        else
        {
            _isCrafting = true;

            CraftOrder peekedOrder = _list_CraftOrder[0];

            for (int i = 0; i < peekedOrder.RecipyData.OutputItemGroup.Count; i++)//다음에 제작할 물품의 완성 아이템 목록 순회
            {                
                if (CharactorInventory.CanAddItem(peekedOrder.RecipyData.OutputItemGroup[i].data.Id_UShort, peekedOrder.RecipyData.OutputItemGroup[i].Count) == false)//완성품을 하나라도 인벤토리에 투입하지 못할 경우, 비제작 상태로 변경
                {
                    _isCrafting = false;
                    Debug.Log("아웃풋이 가득 찼습니다.");
                    return;
                }
            }

            UpdateCraftingTime(peekedOrder.RecipyData);
        }
    }
}