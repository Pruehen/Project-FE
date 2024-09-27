using System.Collections.Generic;
using UnityEngine;

public class Wdw_InventoryView : MonoBehaviour, IWindow
{    
    [SerializeField] List<ItemCell> cellList;

    InventoryModule _module;
    protected InventoryModule Inventory
    {
        get { return _module; }
        set
        {
            if (_module != value)
            {
                _module = value;
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
    public virtual void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);        

        UIManager.Instance.OnDeActive_ModuleWdw(_module);
    }
    public void Command_Close()
    {
        _module.Close_Wdw();
    }
}