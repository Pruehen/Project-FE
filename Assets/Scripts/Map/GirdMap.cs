using UnityEngine;

public static class GirdMap
{
    public static Vector3 ToIntVector(this Vector3 vector)
    {
        int x = Mathf.RoundToInt(vector.x); // x 값을 반올림하여 int로 변환
        int z = Mathf.RoundToInt(vector.z); // z 값을 반올림하여 int로 변환
        float y = Mathf.RoundToInt(vector.y) + 0.1f;

        return new Vector3(x, y, z); // 새로운 Vector3 반환
    }
}
