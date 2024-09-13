using UnityEngine;

public class TimeManager : SceneSingleton<TimeManager>
{
    float time_h;
    Quaternion startRotation;
    [SerializeField] float dayTime_min = 1;

    public float Time_H
    {
        get { return time_h; }
        set
        {
            time_h = value % 24;
            sun.transform.rotation = startRotation * GetSunPos(time_h);
            sun.intensity = sunIntencity.Evaluate(time_h);
            sun.color = sunColor.Evaluate(time_h * 0.0416f);
        }
    }

    Quaternion GetSunPos(float time)
    {
        return Quaternion.Euler(time * 15, 0, 0);
    }

    [SerializeField] Light sun;
    [SerializeField] AnimationCurve sunIntencity;
    [SerializeField] Gradient sunColor;

    // Start is called before the first frame update
    void Start()
    {        
        startRotation = sun.transform.rotation;
        Time_H = 12;
    }

    // Update is called once per frame
    void Update()
    {
        Time_H += Time.deltaTime * 24 * (1 / (60 * dayTime_min));
    }
}