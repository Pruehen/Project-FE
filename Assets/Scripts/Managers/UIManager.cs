using System.Collections.Generic;
using UnityEngine;

public class UIManager : SceneSingleton<UIManager>
{
    [Header("프리팹")]
    public GameObject Prefab_CharactorInventoryUIWdw;
    public GameObject Prefab_CharactorCraftingModuleUIWdw;
    public GameObject Prefab_InventoryUIWdw;
    public GameObject Prefab_CraftingModuleUIWdw;
    public GameObject Prefab_MinerModuleUIWdw;

    [Header("하위 UI")]    
    [SerializeField] MouseTrackUI _MouseTrackUI;

    [Header("기타")]
    [SerializeField] Transform Trf_WindowParent;

    Dictionary<IModule, IWindow> ActiveWdwModuleDic = new Dictionary<IModule, IWindow>();
    //=============================================================================================================================
    //public void Active_BuildingMainModuleUIWdw(IModule module)
    //{
    //    if (module != null)
    //    {
    //        module.Active_Wdw();
    //    }
    //}
    public IWindow Actvie_ModuleWdw<T>(GameObject windowPrefab, T module) where T : MonoBehaviour, IModule
    {
        if (ActiveWdwModuleDic.ContainsKey(module) == false && ActiveWdwModuleDic.Count < 5)
        {
            GameObject obj = ObjectPoolManager.Instance.DequeueObject(windowPrefab);
            obj.transform.SetParent(Trf_WindowParent);

            IWindow window = obj.GetComponent<IWindow>();

            window.Active(module);

            ActiveWdwModuleDic.Add(module, window);
            return window;
        }
        else
        {
            return null;
        }
    }
    public void OnDeActive_ModuleWdw<T>(T module) where T : MonoBehaviour, IModule
    {
        ActiveWdwModuleDic.Remove(module);
    }
    public void AllModuleWdwDeActive()
    {
        List<IWindow> wdwTemp = new List<IWindow>();
        foreach (var item in ActiveWdwModuleDic)
        {
            wdwTemp.Add(item.Value);
        }
        foreach (var item in wdwTemp)
        {
            item.Command_Close();
        }
    }
    //=============================================================================================================================
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
    //=============================================================================================================================
    public void SetActive_Label_BuildMode(bool value)
    {
        //Label_BuildMode.SetActive(value);
    }
    private void Awake()
    {
        SetActive_Label_BuildMode(false);
    }
}
