using UnityEngine;

public static class GirdMap
{
    public static Vector3 ToIntVector(this Vector3 vector)
    {
        int x = Mathf.RoundToInt(vector.x); // x 값을 반올림하여 int로 변환
        int z = Mathf.RoundToInt(vector.z); // z 값을 반올림하여 int로 변환
        int y = 0; // y 값을 0으로 설정

        return new Vector3(x, y, z); // 새로운 Vector3 반환
    }
}
