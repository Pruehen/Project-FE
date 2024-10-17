using EnumTypes;

public class CharactorInventoryModule : InventoryModule
{
    protected override void Awake()
    {
        Inventory = new Inventory(inventoryMaxCount, false, InventoryType.CharactorStorage);
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