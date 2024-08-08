using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lode : MonoBehaviour, IInteractable
{
    [SerializeField] string lodeName;

    public string GetName()
    {
        return lodeName;
    }
    public Vector3 GetPos()
    {
        return this.transform.position;
    }
    public bool TryInteract(Vector3 originPos, float checkRange)
    {
        if(Vector3.Distance(originPos, GetPos()) > checkRange)
        {
            return false;
        }
        else
        {
            Debug.Log($"{lodeName} √§√Î");

            return true;
        }
    }
}
