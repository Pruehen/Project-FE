using TMPro;
using UI.Extension;
using UnityEngine;
using UnityEngine.UI;

public class MouseTrackUI : MonoBehaviour
{
    [SerializeField] GameObject ItemInfo;
    [SerializeField] TextMeshProUGUI Text_Name;
    [SerializeField] TextMeshProUGUI Text_Count;
    [SerializeField] TextMeshProUGUI Text_Decs;

    [SerializeField] Image Icon_GrabItem;

    RectTransform _rectTransform;
    bool _isGrab = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        SetCellData_OnCellPointerEnter(null);
        Icon_GrabItem.gameObject.SetActive(false);
    }

    public void SetCellData_OnCellPointerEnter(CellData cellData)
    {
        if(cellData == null)
        {
            ItemInfo.SetActive(false);
        }
        else
        {
            ItemInfo.SetActive(true);

            ItemData itemData = JsonDataManager.GetItem(cellData.Id);
            Text_Name.text = itemData.Name.GetTextTable();
            Text_Count.text = cellData.Count.ToString();
            Text_Decs.text = itemData.Desc.GetTextTable();
        }
    }

    public void SetIcon_OnGrab(CellData cellData)
    {
        if (cellData != null && cellData.Id != null)
        {
            Icon_GrabItem.gameObject.SetActive(true);            
            Icon_GrabItem.SetLoadSprite(JsonDataManager.GetItem(cellData.Id).Icon_Path);
            _isGrab = true;
        }
    }
    public void RemoveIcon_OnDrop()
    {
        Icon_GrabItem.gameObject.SetActive(false);
        _isGrab = false;
    }

    private void Update()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 screenPoint = Input.mousePosition;
        Vector2 position = screenPoint - screenSize * 0.5f;
        _rectTransform.anchoredPosition = position;
    }
}
