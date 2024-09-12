using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    static ItemCell _selectedCell;
    static ItemCell _onMouseCell;

    public static ItemCell SelectedCell
    {
        get { return _selectedCell; }
        set
        {
            _selectedCell = value;
        }
    }
    public static ItemCell OnMouseCell
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
    public CellData CellData
    {
        get { return _cellData; }
        private set
        {
            if(_cellData != null)
            {
                _cellData.PropertyChanged -= OnPropertyChanged;
            }

            _cellData = value;
            _cellData.PropertyChanged += OnPropertyChanged;
            _cellData.RefreshVM();
        }
    }

    public void RegisterCellData(CellData cellData)
    {
        CellData = cellData;
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CellData.Id):                
                if (CellData.Id != 0)
                {                                        
                    ItemData item = JsonDataManager.GetItem(CellData.Id);
                    Image_ItemIcon.SetLoadSprite(item.Icon_Path);
                    Image_FixedItemIcon.SetLoadSprite(item.Icon_Path);
                }
                else
                {
                    Image_ItemIcon.gameObject.SetActive(false);
                }
                break;
            case nameof(CellData.Count):
                if (CellData.Id != 0 && CellData.Count > 0)
                {
                    Image_ItemIcon.gameObject.SetActive(true);
                    TMP_ItemCount.text = CellData.Count.ToString();                    
                }
                else
                {
                    Image_ItemIcon.gameObject.SetActive(false);
                    if (CellData.FixedCell == false)
                    {
                        TMP_ItemCount.text = string.Empty;
                    }
                    else
                    {
                        TMP_ItemCount.text = CellData.Count.ToString();
                    }
                }
                break;
            case nameof(CellData.FixedCell):
                Image_FixedItemIcon.gameObject.SetActive(CellData.FixedCell);
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

    public void Set_OnMouseCell_OnPointerEnter()
    {
        if (this.CellData != null)
        {
            OnMouseCell = this;
        }
    }
        public void Remove_OnMouseCell_OnPointerExit()
    {
        OnMouseCell = null;
    }

    void ItemGrab_OnPointerDown()
    {
        if (CellData == null || (CellData.FixedCell && CellData.Count == 0) || CellData.Id == 0)
            return;
        else
        {
            SelectedCell = this;
            UIManager.Instance.SetIcon_MouseTrackUI_OnGrab(CellData);
            Debug.Log("그랩");
        }
    }
    void ItemDrop_OnPointerUp()
    {
        Debug.Log("드랍");
        UIManager.Instance.RemoveIcon_MouseTrackUI_OnDrop();

        if (OnMouseCell != null && SelectedCell != null && OnMouseCell.CellData.Inventory != SelectedCell.CellData.Inventory)
        {
            Debug.Log($"아이템 이동  {SelectedCell.name} -> {OnMouseCell.name}");
            OnMouseCell.CellData.Inventory.AddItem(SelectedCell.CellData.Id, SelectedCell.CellData.Count, out int remaining);
            SelectedCell.CellData.UseItem(SelectedCell.CellData.Count - remaining);
        }

        SelectedCell = null;
    }
    void ItemTransport_OnPointerDown()
    {
        if (CellData == null || (CellData.FixedCell && CellData.Count == 0) || CellData.Id == 0)
            return;
        else
        {
            Inventory targetInventory = CellData.Inventory.Find_TransportTargetInventory();
            if (targetInventory != null)
            {
                targetInventory.AddItem(CellData.Id, CellData.Count, out int remaining);
                CellData.UseItem(CellData.Count - remaining);
            }
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemGrab_OnPointerDown();
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            ItemTransport_OnPointerDown();
        }            
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemDrop_OnPointerUp();
        }
        //else if (eventData.button == PointerEventData.InputButton.Right)
        //{
            
        //}
    }
}
