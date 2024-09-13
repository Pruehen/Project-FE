using EnumTypes;

public class CharactorInventoryModule : InventoryModule
{
    Charactor _charactor;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
    }

    public override void Active_Wdw()
    {
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CharactorInventoryUIWdw, this);
            Inventory.OnOpen();
        }
    }
}