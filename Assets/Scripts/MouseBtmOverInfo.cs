using TMPro;
using UI.Extension;
using UnityEngine;

public class MouseBtmOverInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Name;
    [SerializeField] TextMeshProUGUI Text_Count;
    [SerializeField] TextMeshProUGUI Text_Decs;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        SetData(null);
    }

    public void SetData(CellData cellData)
    {
        if(cellData == null)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);

            ItemData itemData = JsonDataManager.GetItem(cellData.Id);
            Text_Name.text = itemData.Name.GetTextTable();
            Text_Count.text = cellData.Count.ToString();
            Text_Decs.text = itemData.Desc.GetTextTable();
        }
    }

    private void Update()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 screenPoint = Input.mousePosition;
        Vector2 position = screenPoint - screenSize * 0.5f;
        _rectTransform.anchoredPosition = position;
    }
}
