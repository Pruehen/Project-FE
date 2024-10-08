using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static Extension;

public class Wdw_MinerModuleView : MonoBehaviour, IWindow
{
    [SerializeField] Display display;
    [SerializeField] ProgressBar progressBar_Crafting;
    [SerializeField] ProgressBar progressBar_Fuel;
    
    [SerializeField] List<ItemCell> outputCellList;

    MinerModuleViewModel _vm;
    MinerModule module;

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);

        module = craftingModule as MinerModule;        

        if (_vm == null)
        {
            _vm = new MinerModuleViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.Register(module.model);
            _vm.Command_RefreshVM(module.model);
        }
    }
    public void Close()
    {
        if (_vm != null)
        {
            _vm.UnRegister(module.model);
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }

        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);

        UIManager.Instance.OnDeActive_ModuleWdw(module);
        module = null;
    }
    public void Command_Close()
    {
        module.Close_Wdw();
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.OutputInventory):
                for (int i = 0; i < _vm.OutputInventory.CellDataList.Count; i++)
                {
                    outputCellList[i].RegisterCellData(_vm.OutputInventory.CellDataList[i]);
                }
                break;
            case nameof(_vm.CraftValueRatio):
                progressBar_Crafting.SetBarRatio(_vm.CraftValueRatio);
                break;
            case nameof(_vm.ExtractItem):
                SetWdw_OnItemDataChange(_vm.ExtractItem);
                break;
        }
    }

    void SetWdw_OnItemDataChange(ItemData data)
    {
        if (data == null)
        {
            display.SetText("À¯ÈÞ");
            for (int i = 0; i < outputCellList.Count; i++)
            {
                outputCellList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            display.SetText(data.Id);
            for (int i = 0; i < outputCellList.Count; i++)
            {
                outputCellList[i].gameObject.SetActive(1 > i);
            }
        }
    }
}

public class MinerModuleViewModel : VM
{
    Inventory _onputInventory;

    ItemData _extractItem;
    float _craftValueRatio;

    public Inventory OutputInventory
    {
        get { return _onputInventory; }
        private set
        {
            _onputInventory = value;
            OnPropertyChanged(nameof(OutputInventory));
        }
    }
    public ItemData ExtractItem
    {
        get { return _extractItem; }
        private set
        {
            _extractItem = value;
            OnPropertyChanged(nameof(ExtractItem));
        }
    }
    public float CraftValueRatio
    {
        get { return _craftValueRatio; }
        private set
        {
            if (_craftValueRatio != value)
            {
                _craftValueRatio = value;
                OnPropertyChanged(nameof(CraftValueRatio));
            }
        }
    }


    public void Register(MinerModuleModel model)
    {
        model.Register_OnExecuteLogic(OnExecuteLogic);
    }
    public void Command_RefreshVM(MinerModuleModel model)
    {        
        model.RefreshVM_OnWdwActive(RefreshVM);
    }
    public void UnRegister(MinerModuleModel model)
    {                
        model.UnRegister_OnExecuteLogic(OnExecuteLogic);
    }

    void OnSetExtractItemData(ItemData itemData)
    {
        this.ExtractItem = itemData;
    }
    void OnExecuteLogic(float craftTimeValue, float craftTime)
    {
        CraftValueRatio = craftTimeValue / craftTime;
    }
    void RefreshVM(Inventory outputIv, ItemData itemData)
    {
        OutputInventory = outputIv;
        this.ExtractItem = itemData;
    }
}