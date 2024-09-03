using UnityEngine;

public class WindowMove : MonoBehaviour
{
    RectTransform rectTransform;

    bool _isMoveMode = false;
    Vector3 mousePosTemp;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    public void SetMoveMode(bool value)
    {
        _isMoveMode = value;
        mousePosTemp = Input.mousePosition;
    }

    private void Update()
    {
        if(_isMoveMode)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mousePos_Delta = mousePos - mousePosTemp;

            this.transform.position += mousePos_Delta;

            mousePosTemp = mousePos;
            rectTransform.ClampToScreen();
        }
    }
}
