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

    [SerializeField] GameObject subUnit_RecipySelectWdw;
    [SerializeField] List<SelectableItemCell> recipySelectCellList;
    bool _isSubUnitActive;

    CraftingModuleViewModel _vm;
    CraftingModule module;
    int instanceId;

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);
        _isSubUnitActive = false;
        subUnit_RecipySelectWdw.SetActive(false);

        module = craftingModule as CraftingModule;
        instanceId = module.gameObject.GetInstanceID();

        if (_vm == null)
        {
            _vm = new CraftingModuleViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.Register(instanceId);
            _vm.Command_RefreshVM(instanceId);
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
        module = null;
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
                SetWdw_OnRecipyDataChange(_vm.RecipyData);
                break;
            case nameof(_vm.CraftValueRatio):
                progressBar_Crafting.SetBarRatio(_vm.CraftValueRatio);
                break;
            case nameof(_vm.RecipyList):
                if(_vm.RecipyList == null)
                {

                }
                else
                {
                    for (int i = 0; i < recipySelectCellList.Count; i++)
                    {
                        recipySelectCellList[i].gameObject.SetActive(_vm.RecipyList.Count > i);
                    }
                    for (int i = 0; i < _vm.RecipyList.Count; i++)
                    {
                        recipySelectCellList[i].Register_CraftModule(this);
                        recipySelectCellList[i].SetData(_vm.RecipyList[i]);
                    }
                }
                break;
        }
    }

    void SetWdw_OnRecipyDataChange(RecipyData data)
    {
        if (data == null)
        {
            display.SetText("À¯ÈÞ");
            for (int i = 0; i < inputCellList.Count; i++)
            {
                inputCellList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < outputCellList.Count; i++)
            {
                outputCellList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            display.SetText(data.Id);
            for (int i = 0; i < inputCellList.Count; i++)
            {
                inputCellList[i].gameObject.SetActive(data.InputItemGroup.Count > i);
            }
            for (int i = 0; i < outputCellList.Count; i++)
            {
                outputCellList[i].gameObject.SetActive(data.OutputItemGroup.Count > i);
            }
        }
    }

    public void Toggle_SubUnitActive()
    {
        _isSubUnitActive = !_isSubUnitActive;
        subUnit_RecipySelectWdw.SetActive(_isSubUnitActive);
    }
    public void Command_SetCraftingRecipyData(string recipyId)
    {
        module.SetCraftingRecipyData(recipyId);
    }
}

public class CraftingModuleViewModel : VM
{
    Inventory _inputInventory;
    Inventory _onputInventory;

    List<string> _recipyList;

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
    public List<string> RecipyList
    {
        get { return _recipyList; }
        set
        {
            _recipyList = value;
            OnPropertyChanged(nameof(RecipyList));
        }
    }


    public void Register(int id)
    {
        CraftingModuleModel model = ModelManager._craftingModuleModelDic[id];

        model.Register_OnSetCraftingRecipyData(OnSetCraftingRecipyData);
        model.Register_OnExecuteLogic(OnExecuteLogic);      
    }
    public void Command_RefreshVM(int id)
    {
        CraftingModuleModel model = ModelManager._craftingModuleModelDic[id];
        model.RefreshVM_OnWdwActive(RefreshVM);
    }
    public void UnRegister(int id)
    {
        CraftingModuleModel model = ModelManager._craftingModuleModelDic[id];

        model.UnRegister_OnSetCraftingRecipyData(OnSetCraftingRecipyData);
        model.UnRegister_OnExecuteLogic(OnExecuteLogic);       
    }

    void OnSetCraftingRecipyData(RecipyData recipyData)
    {
        RecipyData = recipyData;
    }
    void OnExecuteLogic(float craftTimeValue, float craftTime)
    {
        CraftValueRatio = craftTimeValue / craftTime;
    }
    void RefreshVM(RecipyData recipy, Inventory inputIv, Inventory outputIv, List<string> recipyList)
    {
        RecipyData = recipy;
        RecipyList = recipyList;
        InputInventory = inputIv;
        OutputInventory = outputIv;
    }    
}