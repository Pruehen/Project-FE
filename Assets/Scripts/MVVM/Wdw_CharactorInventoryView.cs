public class Wdw_CharactorInventoryView : Wdw_InventoryView
{    
    public override void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);        

        UIManager.Instance.OnDeActive_ModuleWdw(Inventory);
        UIManager.Instance.AllModuleWdwDeActive();
    }
}