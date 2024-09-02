using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static Extension;

public class Wdw_CraftingModuleView : MonoBehaviour, IWindow
{
    [SerializeField] Display display;
    [SerializeField] ProgressBar progressBar_Crafting;
    [SerializeField] ProgressBar progressBar_Fuel;

    [SerializeField] List<ItemCell> inputCellList;
    [SerializeField] List<ItemCell> outputCellList;

    CraftingModuleViewModel _vm;
    int instanceId;

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);
        instanceId = (craftingModule as CraftingModule).gameObject.GetInstanceID();

        if (_vm == null)
        {
            _vm = new CraftingModuleViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.Register(instanceId);
        }
    }
    public void Close()
    {
        if (_vm != null)
        {
            _vm.UnRegister(instanceId);
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }

        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);

        UIManager.Instance.OnDeActive_ModuleWdw(instanceId);
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.InputInventory):
                for (int i = 0; i < _vm.InputInventory.CellDataList.Count; i++)
                {
                    inputCellList[i].RegisterCellData(_vm.InputInventory.CellDataList[i]);
                }
                break;
            case nameof(_vm.OutputInventory):
                for (int i = 0; i < _vm.OutputInventory.CellDataList.Count; i++)
                {
                    outputCellList[i].RegisterCellData(_vm.OutputInventory.CellDataList[i]);
                }
                break;
            case nameof(_vm.RecipyData):
                display.SetText(_vm.RecipyData.Id);
                for (int i = 0; i < inputCellList.Count; i++)
                {
                    inputCellList[i].gameObject.SetActive(_vm.RecipyData.InputItemGroup.Count > i);
                }
                for (int i = 0; i < outputCellList.Count; i++)
                {
                    outputCellList[i].gameObject.SetActive(_vm.RecipyData.OutputItemGroup.Count > i);
                }
                break;
            case nameof(_vm.CraftValueRatio):
                progressBar_Crafting.SetBarRatio(_vm.CraftValueRatio);
                break;                
        }
    }
}

public class CraftingModuleViewModel : VM
{
    Inventory _inputInventory;
    Inventory _onputInventory;

    RecipyData _recipyData;
    float _craftValueRatio;

    public Inventory InputInventory
    {
        get { return _inputInventory; }
        set
        {
            _inputInventory = value;
            OnPropertyChanged(nameof(InputInventory));
        }
    }
    public Inventory OutputInventory
    {
        get { return _onputInventory; }
        set
        {
            _onputInventory = value;
            OnPropertyChanged(nameof(OutputInventory));
        }
    }
    public RecipyData RecipyData
    {
        get { return _recipyData; }
        set
        {
            _recipyData = value;
            OnPropertyChanged(nameof(RecipyData));
        }
    }
    public float CraftValueRatio
    {
        get { return _craftValueRatio; }
        set
        {
            if (_craftValueRatio != value)
            {
                _craftValueRatio = value;
                OnPropertyChanged(nameof(CraftValueRatio));
            }
        }
    }

    public void Register(int id)
    {
        CraftingModuleModel model = ModelManager._craftingModuleModelDic[id];

        model.Register_OnSetCraftingRecipyData(OnSetCraftingRecipyData);
        model.Register_OnExecuteLogic(OnExecuteLogic);
    }

    public void UnRegister(int id)
    {
        CraftingModuleModel model = ModelManager._craftingModuleModelDic[id];

        model.UnRegister_OnSetCraftingRecipyData(OnSetCraftingRecipyData);
        model.UnRegister_OnExecuteLogic(OnExecuteLogic);
    }

    void OnSetCraftingRecipyData(RecipyData recipyData, Inventory inputIv, Inventory outputIv)
    {
        RecipyData = recipyData;
        InputInventory = inputIv;
        OutputInventory = outputIv;
    }
    void OnExecuteLogic(float craftTimeValue, float craftTime)
    {
        CraftValueRatio = craftTimeValue / craftTime;
    }
}