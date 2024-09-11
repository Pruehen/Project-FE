using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InventoryModule : MonoBehaviour, IModule
{
    [SerializeField] int inventoryMaxCount = 150;
    public int InventoryMaxCount() { return inventoryMaxCount; }

    Inventory _inventory;
    public Inventory Inventory
    {
        get { return _inventory; }
        private set
        {
            _inventory = value;
        }
    }
    public CellData CellItemData(int index) { return Inventory.CellDataList[index]; }    

    private void Awake()
    {
        Inventory = new Inventory(inventoryMaxCount, false);
    }

    IWindow window;
    public void Active_Wdw()
    {
        window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_InventoryUIWdw, this);
    }
    public void Close_Wdw()
    {
        window.Close();
    }
    public Inventory TryGetInputInventory()
    {
        return Inventory;
    }
    public Inventory TryGetOutputInventory()
    {
        return Inventory;
    }
}

public class Inventory
{
    public List<CellData> CellDataList { get; private set; }
    int CellCorsor { get; set; }
    bool FixedInventory { get; set; }
    public Action OnInventoryChange;

    public Inventory(int maxCount, bool fixedInventory)
    {
        CellDataList = new List<CellData>();
        for (int i = 0; i < maxCount; i++)
        {
            CellDataList.Add(new CellData(this, 0, 0, fixedInventory));
        }
        CellCorsor = 0;
        FixedInventory = fixedInventory;
    }

    public void Clear()//인벤토리의 내용물을 싹 지워버림
    {
        foreach (var item in CellDataList)
        {
            item.Clear();
        }
        OnInventoryChange?.Invoke();
    }
    public bool CanAddItem(int id, int count)//인벤토리에 아이템을 추가할 수 있는지 판별함
    {
        if (FixedInventory)
        {
            if(TryFindCell(id, out CellData cell))
            {
                return cell.CanItemAdd();
            }
            else
            {
                return false;
            }
        }
        else
        {
            int cellCorsorTemp = CellCorsor;
            bool canAddItem = false;

            while (count > 0)
            {
                if (CellCorsor >= CellDataList.Count)
                {
                    canAddItem = false;
                    break;
                }
                // 현재 셀의 ID가 새로 추가할 아이템의 ID와 다르거나 아이템을 추가할 수 없는 경우
                if (CellDataList[CellCorsor].Id != id || CellDataList[CellCorsor].CanItemAdd() == false)
                {
                    SetCorsor_FindValidCellIndex(id);
                }

                // 셀 데이터 리스트의 범위를 벗어나는 경우
                if (CellCorsor >= CellDataList.Count)
                {
                    canAddItem = false;
                    break;
                }

                int cellRemaining = count - CellDataList[CellCorsor].Count;

                // 남은 아이템 수가 있는 경우
                if (cellRemaining > 0)
                {
                    // 남은 아이템 수를 다음 반복으로 전달
                    count = cellRemaining;
                    // 다음 셀로 커서 이동
                    CellCorsor++;
                }
                else
                {
                    // 아이템이 모두 추가된 경우
                    canAddItem = true;
                    break;
                }
            }

            CellCorsor = cellCorsorTemp;        
            return canAddItem;
        }
    }
    public void AddItem(int id, int count, out int remaining)//인벤토리를 찾아서 아이템 추가를 시도함. 아이템이 다 안 들어가면 remaining으로 남은 수량이 반환됨.
    {
        if (FixedInventory)
        {
            AddItem_FixedInventory(id, count, out remaining);
        }
        else
        {
            AddItem_NotFixedInventory(id, count, out remaining);
        }
        OnInventoryChange?.Invoke();
    }
    public bool CanUseItem(int id, int count)//아이템 소모가 가능한지를 체크함
    {
        if (TryFindCell(id, out CellData targetCell))
        {
            return (targetCell.Count >= count);
        }
        else
        {
            return false;
        }
    }
    public void UseItem_FixedInventory(int id, int count)//아이템을 소모함. 이 메서드 호출 이전에 CanUseItem 메서드를 한번 호출하는걸 권장함. 내부적으로 검사를 하긴 하지만
    {
        if (TryFindCell(id, out CellData targetCell))
        {
            targetCell.UseItem(count);
            OnInventoryChange?.Invoke();
        }
    }
    public void OnUseItem_NonFixedInventory()//아이템이 소모되었을 때 호출됨. 외부에서 호출할 필요 없음.
    {
        CellDataList.InsertionCellSort();
    }
    public bool CanGrabItem()//아이템을 투입기 등으로 잡을 수 있는지 체크함. 아이템 종류를 가리지 않음.
    {
        foreach (CellData cell in CellDataList)
        {
            if(cell.Id != 0 && cell.Count > 0) return true;
        }

        return false;
    }
    public void GrabItem(out int id, out int count)//아이템을 투입기 등으로 잡아서 옮김.
    {
        id = 0;
        count = 0;

        foreach (CellData cell in CellDataList)
        {
            if (cell.Id != 0 && cell.Count > 0)
            {
                id = cell.Id;
                count = 1;
                cell.UseItem(count);
                return;
            }
        }

        Debug.LogError("수송 실패");
    }
    public int GetNextGrabItem()
    {
        foreach (CellData cell in CellDataList)
        {
            if (cell.Id != 0 && cell.Count > 0)
            {
                return cell.Id;
            }
        }
        return 0;
    }//다음에 투입기로 잡을 아이템이 뭔지 확인함

    void AddItem_NotFixedInventory(int id, int count, out int remaining)
    {
        while (count > 0)
        {
            if (CellCorsor >= CellDataList.Count)
            {
                Debug.LogWarning("인벤토리가 가득 찼습니다.");
                break;
            }
            // 현재 셀의 ID가 새로 추가할 아이템의 ID와 다르거나 아이템을 추가할 수 없는 경우
            if (CellDataList[CellCorsor].Id != id || CellDataList[CellCorsor].CanItemAdd() == false)
            {
                SetCorsor_FindValidCellIndex(id);
            }

            // 셀 데이터 리스트의 범위를 벗어나는 경우
            if (CellCorsor >= CellDataList.Count)
            {
                Debug.LogWarning("인벤토리가 가득 찼습니다.");
                break;
            }

            // 현재 셀에 아이템 추가 시도
            CellDataList[CellCorsor].AddItem_NotFixedCell(id, count, out int cellRemaining);

            // 남은 아이템 수가 있는 경우
            if (cellRemaining > 0)
            {
                // 남은 아이템 수를 다음 반복으로 전달
                count = cellRemaining;
                // 다음 셀로 커서 이동
                CellCorsor++;
            }
            else
            {
                // 아이템이 모두 추가된 경우
                count = 0;
                break;
            }
        }

        remaining = count;
        // 정렬
        CellDataList.InsertionCellSort();
    }
    void AddItem_FixedInventory(int id, int count, out int remaining)
    {
        remaining = count;
        if (TryFindCell(id, out CellData targetCell))
        {
            targetCell.AddItem_FixedCell(targetCell.Id, count, out remaining);
        }        
        
    }
    bool TryFindCell(int id, out CellData cell)
    {
        cell = null;
        foreach (var item in CellDataList)
        {
            if (item.Id == id)
            {
                cell = item;
                return true;
            }
        }
        Debug.LogWarning($"해당하는 아이템 슬롯을 찾지 못했습니다. : {id}");
        return false;
    }
    void SetCorsor_FindValidCellIndex(int searchId)
    {
        for (CellCorsor = 0; CellCorsor < CellDataList.Count; CellCorsor++)
        {
            int indexSlotId = CellDataList[CellCorsor].Id;
            if (CellDataList[CellCorsor].CanItemAdd() && CellDataList[CellCorsor].Id == searchId)//목표 커서 아이템이 찾는 아이템과 같고, 아이템 추가가 가능할 경우
            {
                return;
            }

            if (indexSlotId == 0)//빈 슬롯일 경우
            {
                return;
            }
        }
    }
}

public class CellData : IComparable<CellData>
{
    int _id;
    int _count;
    int _maxCount;
    bool _fixedCell;
    Inventory _inventory;

    public int CompareTo(CellData other)
    {
        if (other == null)
            return int.MaxValue; // Null은 비교할 수 없는 것으로 간주

        // _id의 해시값을 기준으로 비교
        int thisHashCode = (_id != 0) ? _id - Count : int.MaxValue;
        int otherHashCode = (other._id != 0) ? other._id - other.Count : int.MaxValue;

        return thisHashCode.CompareTo(otherHashCode);
    }


    public int Id 
    { 
        get { return _id; } 
        private set
        {            
            if(_id != value)
            {
                _id = value;                
                OnPropertyChanged(nameof(Id));
            }
        }
    }    
    public int Count
    {
        get { return _count; }
        private set
        {
            if (_count != value)
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
    }
    public int MaxCount
    {
        get { return _maxCount; }
        private set
        {
            if (_maxCount != value)
            {
                _maxCount = value;
                OnPropertyChanged(nameof(MaxCount));
            }
        }
    }
    public bool FixedCell
    {
        get { return _fixedCell; }
        private set
        {
            if (_fixedCell != value)
            {
                _fixedCell = value;
                OnPropertyChanged(nameof(FixedCell));
            }
        }
    }    //아이템 고정 변수. true일 시 아이템이 모두 제거되어도 Id가 null이 되지 않음.
    public Inventory Inventory
    {
        get { return _inventory; }
        private set
        {
            if (_inventory != value)
            {
                _inventory = value;                
            }
        }
    }
    public bool CanItemAdd()
    {
        return MaxCount > Count;
    }    
    public void RefreshVM()
    {
        OnPropertyChanged(nameof(Id));
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(MaxCount));
        OnPropertyChanged(nameof(FixedCell));
    }

    public CellData(Inventory inventory, int id = 0, int count = 0, bool fixedCell = false)
    {
        this.Inventory = inventory;
        FixedCell = fixedCell;
        Count = count;
        
        SetItem(id);
    }
    public void Clear()//Remove
    {
        Id = 0;
        Count = 0;
        MaxCount = 0;             
    }
    public void SetItem(int itemId)
    {
        Id = itemId;        
        if(Id != 0)
        {
            MaxCount = JsonDataManager.GetItem(itemId).MaxStack;
        }        
        else
        {
            MaxCount = 0;
        }
    }

    public void AddItem_NotFixedCell(int id, int count, out int remaining)
    {
        remaining = 0;

        if (Id == 0)//빈 칸일 경우
        {
            SetItem(id);
            Debug.LogWarning("수신 셀이 비어있습니다. 아이템을 할당합니다.");
        }
        else if (Id != id)
        {
            Debug.LogError("다른 아이템이 추가되었습니다.");
            return;
        }

        Count += count;

        if (Count > MaxCount && FixedCell == false)
        {
            remaining = Count - MaxCount;
            Count = MaxCount;
        }
    }
    public void AddItem_FixedCell(int id, int count, out int remaining)
    {
        remaining = count;
        if (FixedCell == false)
        {
            Debug.LogWarning("고정 아이템 셀이 아닙니다.");
            return;
        }
        if (Id == 0)//빈 칸일 경우
        {
            Debug.LogWarning("수신 셀이 비어있습니다.");
            return;
        }

        Count += count;
        remaining = 0;
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
            if (FixedCell == false)
            {
                Clear();
            }
        }        
        if(Inventory != null)
        {
            Inventory.OnInventoryChange?.Invoke();
            Inventory.OnUseItem_NonFixedInventory();
        }
    }
    public void Swap_OnSort(CellData target)//비고정 셀만 정렬 작업을 수행함
    {
        if(FixedCell)
        {
            return;
        }

        int idTemp = Id;
        int countTemp = Count;
        int maxCountTemp = MaxCount;

        Id = target.Id;
        Count = target.Count;
        MaxCount = target.MaxCount;

        target.OnSwap_SetData(idTemp, countTemp, maxCountTemp);
    }
    void OnSwap_SetData(int id, int count, int maxCount)
    {
        Id = id;
        Count = count;
        MaxCount = maxCount;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)//값이 변경되었을 때 이벤트를 발생시키기 위한 용도 (데이터 바인딩)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}