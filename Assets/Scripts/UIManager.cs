using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SceneSingleton<UIManager>
{
    [SerializeField] GameObject Prefab_InventoryUIWdw;
    [SerializeField] Transform Trf_WindowParent;

    [SerializeField] MouseBtmOverInfo _BtnMouseOverInfo;

    Dictionary<Inventory, Wdw_InventoryView> useInventoryUI = new Dictionary<Inventory, Wdw_InventoryView>();
    public Wdw_InventoryView Toggle_InventoryUIWdw(Inventory inventory)
    {
        if(useInventoryUI.ContainsKey(inventory))
        {
            if (useInventoryUI[inventory].IsActive == false)
            {
                useInventoryUI[inventory].Active();
                return useInventoryUI[inventory];
            }
            else
            {
                useInventoryUI[inventory].Close();
                return useInventoryUI[inventory];
            }
        }
        else
        {
            GameObject obj = ObjectPoolManager.Instance.DequeueObject(Prefab_InventoryUIWdw);
            obj.transform.SetParent(Trf_WindowParent);

            Wdw_InventoryView newUI = obj.GetComponent<Wdw_InventoryView>();
            newUI.Init(inventory);
            newUI.Active();
            useInventoryUI.Add(inventory, newUI);
            return newUI;
        }
    }

    public void Set_BtnMouseOverInfo(CellData cellData)
    {
        _BtnMouseOverInfo.SetData(cellData);
    }
}
