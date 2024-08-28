using System.Collections.Generic;
using UnityEngine;

public class Wdw_InventoryView : MonoBehaviour, IWindow
{
    [SerializeField] GameObject Prefab_Cell;
    [SerializeField] Transform Trf_Contant;
    List<ItemCell> cellList = new List<ItemCell>();    

    Inventory _Inventory;
    Inventory Inventory
    {
        get { return _Inventory; }
        set
        {
            if (_Inventory != value)
            {
                _Inventory = value;
                Init();
            }
        }
    }

    public void CellChange(int index, CellData cellData)
    {
        cellList[index].Init(cellData);
    }
    public void Active(IModule module)
    {
        this.gameObject.SetActive(true);
        Inventory = module as Inventory;
        Inventory.OnCellDataChanged = CellChange;
    }
    void Init()
    {
        for (int i = 0; i < Inventory.InventoryMaxCount(); i++)
        {
            ItemCell cell = Instantiate(Prefab_Cell, Trf_Contant).GetComponent<ItemCell>();
            cellList.Add(cell);
            cell.Init(Inventory.CellItemData(i));
        }
    }
    public void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);        

        UIManager.Instance.OnDeActive_ModuleWdw(_Inventory);
    }
}