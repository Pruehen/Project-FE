using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wdw_CraftingModuleView : MonoBehaviour, IWindow
{
    [SerializeField] Display display;
    [SerializeField] ProgressBar progressBar_Crafting;
    [SerializeField] ProgressBar progressBar_Fuel;

    [SerializeField] List<ItemCell> inputCellList;
    [SerializeField] List<ItemCell> outputCellList;

    CraftingModule _craftingModule;
    CraftingModule CraftingModule
    {
        get { return _craftingModule; }
        set
        {
            if (_craftingModule != value)
            {
                _craftingModule = value;
                Init();                
            }            
        }
    }

    void Update()
    {
        if(CraftingModule != null)
        {
            progressBar_Crafting.SetBarRatio(CraftingModule.GetCraftingTimeRatio());
        }
    }

    public void Active(IModule craftingModule)
    {
        this.gameObject.SetActive(true);
        CraftingModule = craftingModule as CraftingModule;
    }

    void Init()
    {
        display.SetText("테스트 문자 출력 중...");

        for (int i = 0; i < inputCellList.Count; i++)
        {
            inputCellList[i].gameObject.SetActive(CraftingModule.InputItemTypeNum > i);
        }
        for (int i = 0; i < outputCellList.Count; i++)
        {
            outputCellList[i].gameObject.SetActive(CraftingModule.OutputItemTypeNum > i);
        }

        CraftingModule.GetData(out List<CellData> inputCellData, out List<CellData> outputCellData);
        for (int i = 0; i < inputCellData.Count; i++)
        {
            inputCellList[i].RegisterCellData(inputCellData[i]);
        }
        for (int i = 0; i < outputCellData.Count; i++)
        {
            outputCellList[i].RegisterCellData(outputCellData[i]);
        }
    }

    public void Close()
    {
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);

        UIManager.Instance.OnDeActive_ModuleWdw(CraftingModule);
    }
}