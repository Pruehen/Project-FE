using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    static ItemCell SelectedCell;
    static ItemCell OnMouseCell;

    [SerializeField] TextMeshProUGUI TMP_ItemCount;
    [SerializeField] Image Image_ItemIcon;

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
            Image_ItemIcon.sprite = LoadSprite(item.Icon_Path);
        }
    }

    public void Active_BtnMouseOverInfo_OnPointerEnter()
    {
        if (_cellData.Id != null)
        {
            UIManager.Instance.Set_BtnMouseOverInfo(_cellData);
        }
    }
    public void DeActive_BtnMouseOverInfo_OnPointerExit()
    {
        UIManager.Instance.Set_BtnMouseOverInfo(null);
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
        Debug.Log("그랩");
    }
    public void ItemDrop_OnPointerUp()
    {
        Debug.Log("드랍");

        if(OnMouseCell != null && SelectedCell != null)
        {
            Debug.Log($"아이템 이동  {SelectedCell} -> {OnMouseCell.name}");
        }

        SelectedCell = null;
    }

    Sprite LoadSprite(string path)
    {
        // Resources.Load를 사용하여 스프라이트를 로드합니다.
        return Resources.Load<Sprite>(path);
    }
}
