using UnityEngine;

public class CharactorCraftingModule : CraftingModule
{
    Charactor _charactor;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
    }
    protected override void Awake()
    {
        model = ModelManager.NewModel<CraftingModuleModel>(this.gameObject.GetInstanceID());
    }

    public override void Active_Wdw()
    {
        if (window == null)
        {
            window = UIManager.Instance.Actvie_ModuleWdw(UIManager.Instance.Prefab_CraftingModuleUIWdw, this);
            model.InputInventory.OnOpen();
            model.OutputInventory.OnOpen();
        }
    }
    public override void Close_Wdw()
    {
        if(window != null)
        {
            window.Close();
            window = null;
            model.InputInventory.OnClose();
            model.OutputInventory.OnClose();
        }        
    }
    public override Inventory TryGetInputInventory()
    {
        return model.InputInventory;
    }
    public override Inventory TryGetOutputInventory()
    {
        return model.OutputInventory;
    }

    public override void SetCraftingRecipyData(string recipyKey)
    {
        model.SetCraftingRecipyData(recipyKey);
    }
}