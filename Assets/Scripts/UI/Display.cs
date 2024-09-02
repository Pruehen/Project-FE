using TMPro;
using UnityEngine;

public class Display : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textField;

    public void SetText(string msg)
    {
        textField.text = $"현재 상태 : {msg}";
    }
}
