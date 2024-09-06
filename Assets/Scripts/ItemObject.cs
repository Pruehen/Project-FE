using System.Collections;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 targetPos;

    public void SetPos(Vector3 startPos, Vector3 targetPos)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
    }
    public void SetPos(Vector3 targetPos)
    {
        this.startPos = this.targetPos;
        this.targetPos = targetPos;
    }
    public void ItemMove(float lerpValue)
    {
        this.transform.position = Vector3.Lerp(startPos, targetPos, lerpValue);
    }
}
