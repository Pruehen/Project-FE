using TMPro;
using UI.Extension;
using UnityEngine;
using UnityEngine.UI;

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

    public void Init()
    {
        TMP_ItemCount.text = string.Empty;
        Image_ItemIcon.gameObject.SetActive(false);
    }
    public void Init(CellData cellData)
    {
        _cellData = cellData;
        if (cellData.Id == null)
        {
            TMP_ItemCount.text = string.Empty;
            Image_ItemIcon.gameObject.SetActive(false);
        }
        else
        {
            ItemData item = JsonDataManager.GetItem(cellData.Id);
            TMP_ItemCount.text = cellData.Count.ToString();
            Image_ItemIcon.gameObject.SetActive(true);
            Image_ItemIcon.SetLoadSprite(item.Icon_Path);

            Image_FixedItemIcon.gameObject.SetActive(cellData.FixedCell);
            if (cellData.FixedCell)
            {                
                Image_FixedItemIcon.SetLoadSprite(item.Icon_Path);
            }            
        }
    }

    public void Active_BtnMouseOverInfo_OnPointerEnter()
    {
        if (_cellData != null && _cellData.Id != null)
        {
            UIManager.Instance.SetCellData_MouseTrackUI_OnCellPointerEnter(_cellData);
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
        UIManager.Instance.SetIcon_MouseTrackUI_OnGrab(_cellData);
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
