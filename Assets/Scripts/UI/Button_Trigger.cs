using UnityEngine;
using UnityEngine.Events;

public class Button_Trigger : MonoBehaviour
{    
    public UnityEvent unityEvent;

    public void OnClick()
    {
        unityEvent.Invoke();
    }
}
