using System.Collections.Generic;
using UnityEngine;

public class Wdw_InventoryView : MonoBehaviour, IWindow
{    
    [SerializeField] List<ItemCell> cellList;

    InventoryModule _Inventory;
    InventoryModule Inventory
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

    public void Active(IModule module)
    {
        this.gameObject.SetActive(true);
        Inventory = module as InventoryModule;
    }
    void Init()
    {
        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].gameObject.SetActive(Inventory.InventoryMaxCount() > i);
        }

        for (int i = 0; i < Inventory.InventoryMaxCount(); i++)
        {            
            cellList[i].RegisterCellData(Inventory.CellItemData(i));
        }
    }
    public void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);        

        UIManager.Instance.OnDeActive_ModuleWdw(_Inventory);
    }
    public void Command_Close()
    {
        _Inventory.Close_Wdw();
    }
}