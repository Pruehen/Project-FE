using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using static Extension;

public class Wdw_CharactorCraftingModuleView : MonoBehaviour, IWindow
{
    [SerializeField] List<ItemCell> List_DummyItemCell;
    [SerializeField] GameObject Object_CraftOrderCountOver7;

    Inventory _dummyInventory;
    [SerializeField] Image Image_CraftProgressSquare;

    CharactorCraftingModule module;
    CharactorCraftingModuleModel _vm;

    private void Awake()
    {
        _dummyInventory = new Inventory(6, true, EnumTypes.InventoryType.Temp);

        for (int i = 0; i < List_DummyItemCell.Count; i++)
        {
            List_DummyItemCell[i].RegisterCellData(_dummyInventory.CellDataList[i]);
        }
    }

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);

        module = craftingModule as CharactorCraftingModule;
        _vm = module.Model;

        _vm.PropertyChanged += OnPropertyChanged;
        module.Command_RefreshData();
    }
    public void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);

        UIManager.Instance.OnDeActive_ModuleWdw(module);
        _vm.PropertyChanged -= OnPropertyChanged;

        _vm = null;
        module = null;
    }
    public void Command_Close()
    {
        module.Close_Wdw();
    }
    public void Command_OrderCancel(int index)
    {
        module.Command_OrderCancel(index);
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.CraftingTimeRatio):
                Image_CraftProgressSquare.fillAmount = _vm.CraftingTimeRatio;
                break;
            case nameof(_vm.CraftOrderCount):
                Object_CraftOrderCountOver7.SetActive(_vm.CraftOrderCount >= 7);

                for (int i = 0; i < List_DummyItemCell.Count; i++)
                {
                    bool isActiveViewer = i < _vm.CraftOrderCount;
                    List_DummyItemCell[i].gameObject.SetActive(isActiveViewer);
                }
                break;
            case nameof(_vm.RemainingCount_FirstOrder):
                _dummyInventory.CellDataList[0].SetData(JsonDataManager.GetItem(_vm.List_CraftOrder[0].RecipyData.OutputItem_1).Id_UShort, _vm.RemainingCount_FirstOrder);
                break;
            case nameof(_vm.List_CraftOrder):
                for (int i = 0; i < 6; i++)
                {
                    if (_vm.List_CraftOrder.Count <= i) break;

                    _dummyInventory.CellDataList[i].SetData(JsonDataManager.GetItem(_vm.List_CraftOrder[i].RecipyData.OutputItem_1).Id_UShort, _vm.List_CraftOrder[i].Count);
                }
                break;
        }
    }
}