using System.ComponentModel;
using UnityEngine;

public class PlayerViewModel
{
    string _onMouseObjectName = "";
    Vector3 _mousePosition;

    public string OnMouseObjectName
    {
        get { return _onMouseObjectName; }
        set
        {
            if (_onMouseObjectName == value) return;
            _onMouseObjectName = value;
            OnPropertyChanged(nameof(OnMouseObjectName));
        }
    }
    public Vector3 MousePosition
    {
        get { return _mousePosition; }
        set
        {
            if (_mousePosition == value) return;
            _mousePosition = value;
            OnPropertyChanged(nameof(MousePosition));
        }
    }

    #region PropChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)//값이 변경되었을 때 이벤트를 발생시키기 위한 용도 (데이터 바인딩)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
