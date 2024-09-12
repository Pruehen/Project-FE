public class CharactorInventoryModule : InventoryModule
{
    public override void Active_Wdw()
    {
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CharactorInventoryUIWdw, this);
        }
    }
}