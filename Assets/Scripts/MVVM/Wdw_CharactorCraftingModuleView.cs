using UnityEngine;

public class Wdw_CharactorCraftingModuleView : MonoBehaviour, IWindow
{
    CharactorCraftingModule module;

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);

        module = craftingModule as CharactorCraftingModule;
    }
    public void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);

        UIManager.Instance.OnDeActive_ModuleWdw(module);
        module = null;
    }
    public void Command_Close()
    {
        module.Close_Wdw();
    }
}