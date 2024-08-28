using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SceneSingleton<UIManager>
{
    public GameObject Prefab_InventoryUIWdw;
    public GameObject Prefab_CraftingModuleUIWdw;

    [SerializeField] Transform Trf_WindowParent;

    [SerializeField] MouseTrackUI _MouseTrackUI;

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
    public void Actvie_ModuleWdw(GameObject windowPrefab, IModule module)
    {
        GameObject obj = ObjectPoolManager.Instance.DequeueObject(windowPrefab);
        obj.transform.SetParent(Trf_WindowParent);

        IWindow window = obj.GetComponent<IWindow>();
        window.Active();
        window.Init(module);
    }
    public void Active_BuildingMainModuleUIWdw(IModule module)
    {
        if(module != null)
        {
            module.Active_Wdw();
        }        
    }

    public void SetCellData_MouseTrackUI_OnCellPointerEnter(CellData cellData)
    {
        _MouseTrackUI.SetCellData_OnCellPointerEnter(cellData);
    }
    public void SetIcon_MouseTrackUI_OnGrab(CellData cellData)
    {
        _MouseTrackUI.SetIcon_OnGrab(cellData);        
    }
    public void RemoveIcon_MouseTrackUI_OnDrop()
    {
        _MouseTrackUI.RemoveIcon_OnDrop();
    }
}
