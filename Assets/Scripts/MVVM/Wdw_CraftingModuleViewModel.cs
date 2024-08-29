using System.ComponentModel;
using UnityEngine;

public class Wdw_CraftingModuleViewModel
{
    CraftingModule _craftingModule;
    RecipyData _recipyData;

    public CraftingModule CraftingModule
    {
        get { return _craftingModule; }
        set
        {
            if (_craftingModule == value) return;
            _craftingModule = value;
            OnPropertyChanged(nameof(CraftingModule));
        }
    }
    public RecipyData RecipyData
    {
        get { return _recipyData; }
        set
        {
            if (_recipyData == value) return;
            _recipyData = value;
            OnPropertyChanged(nameof(RecipyData));
        }
    }

    #region PropChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)//값이 변경되었을 때 이벤트를 발생시키기 위한 용도 (데이터 바인딩)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
