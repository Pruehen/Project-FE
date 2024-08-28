using System.Collections.Generic;
using UnityEngine;

public class Wdw_InventoryView : MonoBehaviour, IWindow
{
    [SerializeField] GameObject Prefab_Cell;
    [SerializeField] Transform Trf_Contant;
    List<ItemCell> cellList = new List<ItemCell>();

    public bool IsActive { get; private set; } = false;

    Inventory _Inventory;

    public void CellChange(int index, CellData cellData)
    {
        cellList[index].Init(cellData);
    }
    public void Active(IModule module)
    {
        this.gameObject.SetActive(true);
        IsActive = true;

        _Inventory = module as Inventory;
        for (int i = 0; i < _Inventory.InventoryMaxCount(); i++)
        {
            ItemCell cell = Instantiate(Prefab_Cell, Trf_Contant).GetComponent<ItemCell>();
            cellList.Add(cell);
            cell.Init(_Inventory.CellItemData(i));
        }
    }
    public void Close()
    {
        this.gameObject.SetActive(false);
        IsActive = false;

        UIManager.Instance.OnDeActive_ModuleWdw(_Inventory.gameObject.GetInstanceID());
    }
}