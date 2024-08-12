using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    string spritePath = "Art/Sprite/";

    [SerializeField] TextMeshProUGUI TMP_ItemCount;
    [SerializeField] Image Image_ItemIcon;

    public void Init()
    {
        TMP_ItemCount.text = string.Empty;
        Image_ItemIcon.gameObject.SetActive(false);
    }
    public void Init(CellData cellData)
    {
        if (cellData.Id == -1)
        {
            TMP_ItemCount.text = string.Empty;
            Image_ItemIcon.gameObject.SetActive(false);
        }
        else
        {
            Item item = JsonDataManager.GetItem(cellData.Id);
            TMP_ItemCount.text = cellData.Count.ToString();
            Image_ItemIcon.sprite = LoadSprite(spritePath + item.Icon);
        }
    }

    Sprite LoadSprite(string path)
    {
        // Resources.Load를 사용하여 스프라이트를 로드합니다.
        return Resources.Load<Sprite>(path);
    }
}
