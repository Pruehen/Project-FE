using System.Collections;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    Vector3 startPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;

    public void SetPos(Vector3 startPos, Vector3 targetPos)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.transform.position = startPos;
    }
    public void ItemMove(float lerpValue)
    {
        this.transform.position = Vector3.Lerp(startPos, targetPos, lerpValue);
    }
}