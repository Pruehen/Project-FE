using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image bar;

    public void SetBarRatio(float ratio)
    {
        bar.fillAmount = ratio;
    }
}
