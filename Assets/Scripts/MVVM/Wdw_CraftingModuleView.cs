using System.ComponentModel;
using TMPro;
using UI.Extension;
using UnityEngine;

public class Wdw_CraftingModuleView : MonoBehaviour, IWindow
{
    [SerializeField] TextMeshProUGUI Text_RecipyName;

    Wdw_CraftingModuleViewModel _vm;

    CraftingModule _craftingModule;

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.CraftingModule):
                _craftingModule = _vm.CraftingModule;
                break;
            case nameof(_vm.RecipyData):
                Text_RecipyName.text = _vm.RecipyData.Name.GetTextTable();
                break;
        }
    }

    void Update()
    {
        
    }

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);
        _craftingModule = craftingModule as CraftingModule;

        if (_vm == null)
        {
            _vm = new Wdw_CraftingModuleViewModel();
            _vm.PropertyChanged += OnPropertyChanged;

            //_vm.OnCraftingModuleChanged(craftingModule);
        }
    }


    public void Close()
    {
        this.gameObject.SetActive(false);

        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }

        UIManager.Instance.OnDeActive_ModuleWdw(_craftingModule.gameObject.GetInstanceID());
    }
}