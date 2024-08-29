using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button_Toggle : MonoBehaviour
{
    [SerializeField] Image led;
    [SerializeField] bool startState;

    bool _isOn = false;
    bool IsOn
    {
        get { return _isOn; }
        set
        {
            _isOn = value;
            if(_isOn)
            {
                led.color = Color.green;
            }
            else
            {
                led.color = Color.red;
            }
        }
    }

    public UnityEvent<bool> unityEvent;

    public void OnClick()
    {
        IsOn = !IsOn;
        unityEvent.Invoke(IsOn);
    }

    private void Awake()
    {
        IsOn = startState;
    }
}
