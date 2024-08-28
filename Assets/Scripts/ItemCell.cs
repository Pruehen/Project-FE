using System.ComponentModel;
using TMPro;
using UI.Extension;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemCell : MonoBehaviour
{
    static ItemCell _selectedCell;
    static ItemCell _onMouseCell;

    static ItemCell SelectedCell
    {
        get { return _selectedCell; }
        set
        {
            _selectedCell = value;
        }
    }
    static ItemCell OnMouseCell
    {
        get { return _onMouseCell; }
        set
        {
            _onMouseCell = value;
        }
    }

    [SerializeField] TextMeshProUGUI TMP_ItemCount;
    [SerializeField] Image Image_ItemIcon;
    [SerializeField] Image Image_FixedItemIcon;

    CellData _cellData;
    CellData CellData
    {
        get { return _cellData; }
        set
        {
            if(_cellData != null)
            {
                _cellData.PropertyChanged -= OnPropertyChanged;
            }

            if(_cellData != value)
            {
                _cellData = value;
                _cellData.PropertyChanged += OnPropertyChanged;
            }

            _cellData.RefreshVM();
        }
    }

    public void Init(CellData cellData)
    {
        CellData = cellData;
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CellData.Id):
                if (CellData.Id == null)
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
                if (CellData.Id != null)
                {
                    TMP_ItemCount.text = CellData.Count.ToString();
                }
                else
                {
                    TMP_ItemCount.text = string.Empty;
                }
                break;
            case nameof(CellData.MaxCount):
                break;
            case nameof(CellData.FixedCell):
                Image_FixedItemIcon.gameObject.SetActive(CellData.FixedCell);
                if(CellData.FixedCell)
                {
                    ItemData item = JsonDataManager.GetItem(CellData.Id);
                    Image_FixedItemIcon.SetLoadSprite(item.Icon_Path);
                }
                break;
        }
    }

    public void Active_BtnMouseOverInfo_OnPointerEnter()
    {
        if (CellData != null && CellData.Id != null)
        {
            UIManager.Instance.SetCellData_MouseTrackUI_OnCellPointerEnter(CellData);
        }
    }
    public void DeActive_BtnMouseOverInfo_OnPointerExit()
    {
        UIManager.Instance.SetCellData_MouseTrackUI_OnCellPointerEnter(null);
    }

    public void Set_OnMouseCell_OnPointerEnter()
    {
        OnMouseCell = this;
    }
    public void Remove_OnMouseCell_OnPointerExit()
    {
        OnMouseCell = null;
    }

    public void ItemGrab_OnPointerDown()
    {
        SelectedCell = this;
        UIManager.Instance.SetIcon_MouseTrackUI_OnGrab(CellData);
        Debug.Log("그랩");
    }
    public void ItemDrop_OnPointerUp()
    {
        Debug.Log("드랍");
        UIManager.Instance.RemoveIcon_MouseTrackUI_OnDrop();

        if (OnMouseCell != null && SelectedCell != null)
        {
            Debug.Log($"아이템 이동  {SelectedCell} -> {OnMouseCell.name}");
        }

        SelectedCell = null;
    }
}
