using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IModule
{
    [SerializeField] int inventoryMaxCount = 150;
    public int InventoryMaxCount() { return inventoryMaxCount; }

    List<CellData> tempItemList = new List<CellData>();
    int cellCorsor = 0;

    public List<CellData> TempItemList() { return tempItemList; }
    public CellData CellItemData(int index) { return tempItemList[index]; }

    Wdw_InventoryView wdw;
    public void ToggleActiveUI()
    {
        wdw = UIManager.Instance.Toggle_InventoryUIWdw(this);
    }

    private void Awake()
    {
        for (int i = 0; i < inventoryMaxCount; i++)
        {
            tempItemList.Add(new CellData());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            AddItem("Item_Iron", 1500);
        }
    }

    public void Clear()
    {
        foreach (var item in tempItemList)
        {
            item.Clear();
        }
    }

    public void AddItem(string id, int count)
    {
        if (cellCorsor == -1)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
            return;
        }
        if (tempItemList[cellCorsor].Id != id || tempItemList[cellCorsor].CanItemAdd() == false)
        {
            SetCorsor(id);
        }
        if (cellCorsor == -1)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
            return;
        }

        int remaining = 0;
        tempItemList[cellCorsor].AddItem(id, count, out remaining);
        if(wdw != null)
        {
            wdw.CellChange(cellCorsor, tempItemList[cellCorsor]);
        }


        if(remaining > 0)
        {
            AddItem(id, remaining);
        }
    }

    void SetCorsor(string id)//모든 인벤토리가 가득 찼을 경우, cellCorsor가 -1이 됨.
    {
        int firstEmptySlotIndex = -1;
        for (int index = 0; index < tempItemList.Count; index++)
        {
            string slotId = tempItemList[index].Id;
            if (tempItemList[index].Id == id && tempItemList[index].CanItemAdd())//목표 커서 아이템이 찾는 아이템과 같고, 아이템 추가가 가능할 경우
            {
                cellCorsor = index;
                return;
            }

            if(firstEmptySlotIndex == -1 && slotId == null)
            {
                firstEmptySlotIndex = index;
            }
        }

        cellCorsor = firstEmptySlotIndex;
    }

    public void Active_Wdw()
    {
        
    }
}

public class CellData
{
    public string Id { get; private set; }    
    public int Count { get; private set; }
    public int MaxCount { get; private set; }
    public bool FixedCell { get; private set; }//아이템 고정 변수. true일 시 아이템이 모두 제거되어도 Id가 null이 되지 않음.
    public bool CanItemAdd()
    {
        return MaxCount > Count;
    }

    public CellData(string id, int count, int maxCount, bool fixedCell)
    {
        Id = id;
        Count = count;
        MaxCount = maxCount;
        FixedCell = fixedCell;
    }
    public CellData(string id = null, bool fixedCell = false)
    {
        FixedCell = fixedCell;
        Count = 0;
        if (FixedCell && id != null)
        {
            SetItem(id);            
        }
        else
        {
            Id = null;
            MaxCount = 0;
        }
    }
    public void Clear()//Remove
    {
        if(FixedCell == false)
        {
            Id = null;
            MaxCount = 0;
        }
        Count = 0;        
    }
    public void SetItem(string id)
    {
        Id = id;
        MaxCount = JsonDataManager.GetItem(id).MaxStack;
    }
    public void AddItem(string id, int count, out int remaining)
    {
        remaining = 0;

        if (Id == null)//빈 칸일 경우
        {
            SetItem(id);
            Debug.LogWarning("수신 셀이 비어있습니다. 아이템을 할당합니다.");
        }
        else if(Id != id)
        {
            Debug.LogError("다른 아이템이 추가되었습니다.");
            return;
        }

        Count += count;

        if (Count > MaxCount)
        {
            remaining = Count - MaxCount;
            if(FixedCell == false)
            {
                Count = MaxCount;
            }
        }
    }

    public void AddItem(int count)//FixedCell 전용
    {        
        if(FixedCell == false)
        {
            Debug.LogWarning("고정 아이템 셀이 아닙니다.");
            return;
        }
        if (Id == null)//빈 칸일 경우
        {            
            Debug.LogWarning("수신 셀이 비어있습니다.");
            return;
        }

        Count += count;
    }
    public void UseItem(int count)
    {
        if(count > Count)
        {
            Debug.LogWarning("사용 가능한 수량 이상을 요청했습니다.");
            return;
        }

        Count -= count;
        if (Count == 0)
        {
            Debug.Log("셀 비워짐");
            Clear();
        }
    }
    public void TransportItem(CellData targetCell, int transportCount)
    {
        if (Id == null || Count == 0)
        {
            Debug.LogWarning("발신 셀이 비어있습니다.");
            return;
        }

        if(transportCount > Count)
        {
            transportCount = Count;
            Debug.LogWarning("셀의 보유량을 초과하는 요청입니다.");
        }
        
        targetCell.AddItem(Id, transportCount, out int remaining);
        Count -= transportCount;

        if (remaining > 0)
        {
            Debug.LogWarning($"셀 오버플로우 발생. {remaining} 개의 아이템 반환");
            Count += remaining;
        }
        if(Count == 0)
        {
            Debug.Log("셀 비워짐");
            Clear();
        }
    }
}