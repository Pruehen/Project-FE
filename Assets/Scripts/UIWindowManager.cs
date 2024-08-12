using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : SceneSingleton<UIWindowManager>
{
    [SerializeField] GameObject Prefab_InventoryUIWdw;

    Dictionary<Inventory, Wdw_InventoryView> useInventoryUI = new Dictionary<Inventory, Wdw_InventoryView>();
    public Wdw_InventoryView TryActive_InventoryUIWdw(Inventory inventory)
    {
        if(useInventoryUI.ContainsKey(inventory))
        {
            useInventoryUI[inventory].Active();
            return useInventoryUI[inventory];
        }
        else
        {
            GameObject obj = ObjectPoolManager.Instance.DequeueObject(Prefab_InventoryUIWdw);
            obj.transform.SetParent(this.transform);

            Wdw_InventoryView newUI = obj.GetComponent<Wdw_InventoryView>();
            newUI.Init(inventory);
            useInventoryUI.Add(inventory, newUI);
            return newUI;
        }
    }
    public void TryUnActive_InventoryUIWdw(Inventory inventory)
    {
        if (useInventoryUI.ContainsKey(inventory))
        {
            useInventoryUI[inventory].Close();
        }
    }
}
