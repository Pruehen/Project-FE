using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class CharactorCraftingModule : MonoBehaviour, IModule
{
    Charactor _charactor;
    CharactorCraftingModuleModel _model;

    IWindow window;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
        _model = new CharactorCraftingModuleModel(_charactor.builtIn_InventoryModule.Inventory);        
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
    public Inventory TryGetInputInventory()
    {
        return _charactor.builtIn_InventoryModule.Inventory;
    }
    public Inventory TryGetOutputInventory()
    {
        return _charactor.builtIn_InventoryModule.Inventory;
    }

    void Update()
    {
        _model.ExecuteLogic(Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Q))
        {
            _model.AddCraftOrder("Recipy_IronPlate", 10);
        }
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

public class CharactorCraftingModuleModel
{
    Queue<CraftOrder> queue_CraftOrder = new Queue<CraftOrder>();
    
    public Inventory TempInventory { get; private set; }//제작 명령을 넣을 때 아이템을 담아 놓을 임시 공간. 현재 아이템을 제작 중일 때는 반드시 여기에 아이템이 들어있어야 함.
    public Inventory CharactorInventory { get; private set; }

    float craftingTime = 1;
    float craftingTimeValue;
    bool _isCrafting = false;

    float _craftingTimeGain = 1;
    float _craftingSpeedGain = 1;

    public CharactorCraftingModuleModel(Inventory charactorIv)
    {
        TempInventory = new Inventory(100, false, InventoryType.Temp);
        CharactorInventory = charactorIv;

        //TempInventory.OnInventoryChange += Set_IsCraftItem;
        CharactorInventory.OnInventoryChange += Set_IsCraftItem;
        Set_IsCraftItem();
    }
    //public void Init_RecipyGroupKey(string initKey)
    //{
    //    recipyDataGroupList = JsonDataManager.GetRecipyGroupData(initKey);
    //}

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

    public void AddCraftOrder(string recipyKey, int count)//제작 명령 추가. 외부 인터페이스를 통해 유일하게 접근 가능한 퍼블릭 메서드로 한정해야 함. **해당 메서드 호출 전에, 미리 인벤토리를 체크할 것
    {
        if (recipyKey == null)
        {
            return;
        }
        else
        {
            RecipyData craftingRecipyData = JsonDataManager.GetRecipyData(recipyKey);
            if (craftingRecipyData == null)
            {
                Debug.LogError($"잘못된 키가 입력되었습니다 : {recipyKey}");
            }

            for (int i = 0; i < craftingRecipyData.InputItemGroup.Count; i++)
            {
                CharactorInventory.UseItem(craftingRecipyData.InputItemGroup[i].data.Id_UShort, craftingRecipyData.InputItemGroup[i].Count * count);
                TempInventory.AddItem(craftingRecipyData.InputItemGroup[i].data.Id_UShort, craftingRecipyData.InputItemGroup[i].Count * count, out int r);
            }

            OrderAdd(craftingRecipyData, count);

            Set_IsCraftItem();
        }                
    }
    void OrderAdd(RecipyData craftingRecipyData, int count)
    {
        queue_CraftOrder.Enqueue(new CraftOrder(craftingRecipyData, count));
    }


    public void ExecuteLogic(float deltaTime)//제작 로직 수행
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

            Set_IsCraftItem();
        }
    }

    void CraftItem()
    {
        if (queue_CraftOrder.Count == 0)
        {
            Debug.LogWarning("제작할 레시피가 없습니다.");
            return;
        }

        CraftOrder peekedOrder = queue_CraftOrder.Peek();

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
        CraftOrder peekedOrder = queue_CraftOrder.Peek();
        peekedOrder.ExecuteOrder();

        if(peekedOrder.Count <= 0)
        {
            queue_CraftOrder.Dequeue();
        }
    }    

    void Set_IsCraftItem()//아이템 제작 가능 상태인지 설정함
    {
        if (queue_CraftOrder.Count == 0)
        {
            _isCrafting = false;
        }
        else
        {
            _isCrafting = true;

            //for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
            //{
            //    if (TempInventory.CanUseItem(CraftingRecipyData.InputItemGroup[i].data.Id_UShort, CraftingRecipyData.InputItemGroup[i].Count) == false)
            //    {
            //        _isCrafting = false;
            //        Debug.Log("인풋 아이템이 부족합니다.");
            //        return;
            //    }
            //}

            //for (int i = 0; i < CraftingRecipyData.InputItemGroup.Count; i++)
            //{
            //    if (CharactorInventory.CellDataList[i].CanItemAdd() == false)
            //    {
            //        _isCrafting = false;
            //        Debug.Log("아웃풋이 가득 찼습니다.");
            //        return;
            //    }
            //}

            CraftOrder peekedOrder = queue_CraftOrder.Peek();

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