using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
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
        Inventory = new Inventory(inventoryMaxCount);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            Inventory.AddItem("Item_Copper", 50);
            Inventory.AddItem("Item_Iron", 50);
            Inventory.AddItem("Item_IronPlate", 50);
        }
    }


    IWindow window;
    public void Active_Wdw()
    {
        window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_InventoryUIWdw, this);
    }
    public void Close_Wdw()
    {
        if(window != null)
        {
            window.Close();
        }
    }
}

public class Inventory
{
    public List<CellData> CellDataList { get; private set; }
    int CellCorsor { get; set; }

    public Inventory(int maxCount)
    {
        CellDataList = new List<CellData>();
        for (int i = 0; i < maxCount; i++)
        {
            CellDataList.Add(new CellData());
        }
        CellCorsor = 0;
    }

    public void Clear()
    {
        foreach (var item in CellDataList)
        {
            item.Clear();
        }
    }

    public void AddItem(string id, int count)
    {
        while (count > 0)
        {
            // 현재 셀의 ID가 새로 추가할 아이템의 ID와 다르거나 아이템을 추가할 수 없는 경우
            if (CellDataList[CellCorsor].Id != id || !CellDataList[CellCorsor].CanItemAdd())
            {
                SetCorsor(id);
            }

            // 셀 데이터 리스트의 범위를 벗어나는 경우
            if (CellCorsor >= CellDataList.Count)
            {
                Debug.LogWarning("인벤토리가 가득 찼습니다.");
                break;
            }

            // 현재 셀에 아이템 추가 시도
            int remaining = 0;
            CellDataList[CellCorsor].AddItem(id, count, out remaining);

            // 남은 아이템 수가 있는 경우
            if (remaining > 0)
            {
                // 남은 아이템 수를 다음 반복으로 전달
                count = remaining;
                // 다음 셀로 커서 이동
                CellCorsor++;
            }
            else
            {
                // 아이템이 모두 추가된 경우
                break;
            }
        }

        // 정렬
        CellDataList.InsertionCellSort();
    }

    void SetCorsor(string searchId)//모든 인벤토리가 가득 찼을 경우, cellCorsor가 -1이 됨.
    {
        for (CellCorsor = 0; CellCorsor < CellDataList.Count; CellCorsor++)
        {
            string indexSlotId = CellDataList[CellCorsor].Id;
            if (CellDataList[CellCorsor].CanItemAdd() && CellDataList[CellCorsor].Id == searchId)//목표 커서 아이템이 찾는 아이템과 같고, 아이템 추가가 가능할 경우
            {
                return;
            }

            if (indexSlotId == null)//빈 슬롯일 경우
            {
                return;
            }
        }
    }
}

public class CellData : IComparable<CellData>
{
    string _id;
    int _count;
    int _maxCount;
    bool fixedCell;

    public int CompareTo(CellData other)
    {
        if (other == null)
            return int.MaxValue; // Null은 비교할 수 없는 것으로 간주

        // _id의 해시값을 기준으로 비교
        int thisHashCode = (_id != null) ? _id.GetHashCode() - Count : int.MaxValue;
        int otherHashCode = (other._id != null) ? other._id.GetHashCode() - other.Count : int.MaxValue;

        return thisHashCode.CompareTo(otherHashCode);
    }


    public string Id 
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
        get { return fixedCell; }
        private set
        {
            if (fixedCell != value)
            {
                fixedCell = value;
                OnPropertyChanged(nameof(FixedCell));
            }
        }
    }    //아이템 고정 변수. true일 시 아이템이 모두 제거되어도 Id가 null이 되지 않음.
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
    public void Swap(CellData target)//비고정 셀만 정렬 작업을 수행함
    {
        if(FixedCell)
        {
            return;
        }

        string idTemp = Id;
        int countTemp = Count;
        int maxCountTemp = MaxCount;

        Id = target.Id;
        Count = target.Count;
        MaxCount = target.MaxCount;

        target.Swap_SetData(idTemp, countTemp, maxCountTemp);
    }
    void Swap_SetData(string id, int count, int maxCount)
    {
        Id = id;
        Count = count;
        MaxCount = maxCount;
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
            Debug.LogWarning("셀의 보유량을 초과하는 요청입니다. 운송 수량을 강제로 감소시킵니다.");
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)//값이 변경되었을 때 이벤트를 발생시키기 위한 용도 (데이터 바인딩)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}