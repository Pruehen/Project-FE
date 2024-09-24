using UnityEngine;

public class CharactorCraftingModule : MonoBehaviour, IModule
{
    Charactor _charactor;
    IWindow window;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
    }

    public void Active_Wdw()
    {
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CharactorCraftingModuleUIWdw, this);
        }
    }
    public void Close_Wdw()
    {
        if(window != null)
        {
            window.Close();
            window = null;
        }        
    }
    public Inventory TryGetInputInventory()
    {
        return _charactor.builtIn_InventoryModule.TryGetInputInventory();
    }
    public Inventory TryGetOutputInventory()
    {
        return _charactor.builtIn_InventoryModule.TryGetOutputInventory();
    }
}