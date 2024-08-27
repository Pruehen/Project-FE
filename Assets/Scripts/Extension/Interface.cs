using UnityEngine;

public interface IInteractable
{
    public string GetName();
    public Vector3 GetPos();
    public float InteractSpeedGain();
    public bool TryInteract(Vector3 originPos, float checkRange);
    public void MouseEnter();
    public void MouseExit();
}

public interface ITable
{
    public string FilePath();
}

public interface IWindow
{
    public void Active();
    public void Close();    
}