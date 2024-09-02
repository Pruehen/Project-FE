using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image bar;
    private void Awake()
    {
        SetBarRatio(0);
    }
    public void SetBarRatio(float ratio)
    {
        bar.fillAmount = ratio;
    }
}
