using System.ComponentModel;
using TMPro;
using UI.Extension;
using UnityEngine;
using ViewModel.Extensions;

public class PlayerView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TMP_OnMouseObjectName;

    PlayerViewModel _vm;
    Vector3 _originPos_TMP_OnMouseObjectName;

    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new PlayerViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.Register_OnMouseObjectNameChanged();
            _vm.Refresh_OnMouseObjectNameChanged();
        }
    }
    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.UnRegister_OnMouseObjectNameChanged();
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }
    }

    private void Update()
    {
        UIExtension.SetUIPos_WorldToScreenPos(TMP_OnMouseObjectName.rectTransform, _originPos_TMP_OnMouseObjectName);
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.OnMouseObjectName):
                if (_vm.OnMouseObjectName != null)
                {
                    TMP_OnMouseObjectName.text = _vm.OnMouseObjectName;
                }
                else
                {
                    TMP_OnMouseObjectName.text = "";
                }
                break;
            case nameof(_vm.MousePosition):
                _originPos_TMP_OnMouseObjectName = _vm.MousePosition;                
                break;
        }
    }
}
