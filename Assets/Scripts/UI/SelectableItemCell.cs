using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableItemCell : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TMP_ItemCount;
    [SerializeField] Image Image_ItemIcon;

    string recipyId;
    int itemId;
    string buildingId;

    Action<string> OnClick_CallBackRecipy;
    public void Register_OnClick_CallBackRecipy(Action<string> callBack) { OnClick_CallBackRecipy = callBack; }        

    Action<int> OnClick_CallBackItem;
    public void Register_OnClick_CallBackItem(Action<int> callBack) { OnClick_CallBackItem = callBack; }

    Action<string> OnClick_CallBackBuilding;
    public void Register_OnClick_CallBackBuilding(Action<string> callBack) { OnClick_CallBackBuilding = callBack; }

    CellData _cellData;
    public CellData CellData
    {
        get { return _cellData; }
        private set
        {
            if (_cellData != null)
            {
                _cellData.PropertyChanged -= OnPropertyChanged;
            }

            if (_cellData != value)
            {
                _cellData = value;
                _cellData.PropertyChanged += OnPropertyChanged;
            }

            _cellData.RefreshVM();
        }
    }
    public void SetData_Recipy(string recipyId)//추후 모델 단에서 호출하도록 처리
    {
        this.recipyId = recipyId;
        RecipyData data = JsonDataManager.GetRecipyData(recipyId);
        
        CellData = new CellData(null, (data.OutputItem_1 == null) ? 0 : data.OutputItem_1.GetHashCode(), data.OutputItemCount_1, true);
    }
    public void SetData_Item(int itemId)
    {
        this.itemId = itemId;
        CellData = new CellData(null, itemId, 0, true);        
    }
    public void SetData_Building(string buildingId)
    {
        this.buildingId = buildingId;
        string itemId = buildingId.Replace("Building_", "Item_");            

        CellData = new CellData(null, itemId.GetHashCode(), 0, true);
    }
    public void SetData_StaticCell(string buildingId, int count)
    {
        this.buildingId = buildingId;
        string itemId = buildingId.Replace("Building_", "Item_");

        CellData = new CellData(null, itemId.GetHashCode(), count, true);
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CellData.Id):
                if (CellData.Id == 0)
                {
                    TMP_ItemCount.text = string.Empty;
                    Image_ItemIcon.gameObject.SetActive(false);
                }
                else
                {
                    ItemData item = JsonDataManager.GetItem(CellData.Id);
                    Image_ItemIcon.gameObject.SetActive(true);
                    Image_ItemIcon.SetLoadSprite(item.Icon_Path);
                }
                break;
            case nameof(CellData.Count):
                if (CellData.Id != 0 && CellData.Count > 0)
                {
                    TMP_ItemCount.text = CellData.Count.ToString();
                }
                else
                {
                    TMP_ItemCount.text = string.Empty;
                }
                break;
        }
    }

    public void Active_BtnMouseOverInfo_OnPointerEnter()
    {
        if (CellData != null && CellData.Id != 0)
        {
            UIManager.Instance.SetCellData_MouseTrackUI_OnCellPointerEnter(CellData);
        }
    }
    public void DeActive_BtnMouseOverInfo_OnPointerExit()
    {
        UIManager.Instance.SetCellData_MouseTrackUI_OnCellPointerEnter(null);
    }

    public void SelectCell_OnClick()
    {
        if (this.CellData != null)
        {            
            OnClick_CallBackRecipy?.Invoke(recipyId);
            OnClick_CallBackItem?.Invoke(itemId);
            OnClick_CallBackBuilding?.Invoke(buildingId);
        }
        else
        {
            Debug.LogWarning("빈 셀 데이터를 선택했습니다.");
        }
    }
}
