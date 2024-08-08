using UnityEngine;

public interface IInteractable
{
    public string GetName();
    public Vector3 GetPos();

    public bool TryInteract(Vector3 originPos, float checkRange);
}