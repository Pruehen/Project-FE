using UnityEngine;

public class Window : MonoBehaviour
{
    bool _isMoveMode = false;
    Vector3 mousePosTemp;

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
        }
    }
}
