using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : SceneSingleton<UIWindowManager>
{
    [SerializeField] GameObject Prefab_InventoryUIWdw;

    Dictionary<Inventory, GameObject> useInventoryUI = new Dictionary<Inventory, GameObject>();
    public void TryActive_InventoryUIWdw(Inventory inventory)
    {
        if(useInventoryUI.ContainsKey(inventory))
        {
            useInventoryUI[inventory].SetActive(true);
        }
        else
        {
            GameObject newUI = ObjectPoolManager.Instance.DequeueObject(Prefab_InventoryUIWdw);
            newUI.transform.SetParent(this.transform);
            useInventoryUI.Add(inventory, newUI);
        }
    }
    public void TryUnActive_InventoryUIWdw(Inventory inventory)
    {
        if (useInventoryUI.ContainsKey(inventory))
        {
            useInventoryUI[inventory].SetActive(false);
        }
    }
}
