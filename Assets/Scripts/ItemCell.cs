using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
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

    public void Set_BtnMouseOverInfo(bool OnEnter)
    {
        if (OnEnter && _cellData.Id != null)
        {
            UIManager.Instance.Set_BtnMouseOverInfo(_cellData);
        }
        else
        {
            UIManager.Instance.Set_BtnMouseOverInfo(null);
        }
    }

    Sprite LoadSprite(string path)
    {
        // Resources.Load를 사용하여 스프라이트를 로드합니다.
        return Resources.Load<Sprite>(path);
    }
}
