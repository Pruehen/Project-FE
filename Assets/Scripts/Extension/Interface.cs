using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetName();
    public Vector3 GetPos();
    public float InteractSpeedGain();
    public bool TryInteract(Vector3 originPos, float checkRange);
    public void Select();
    public void MouseEnter();
    public void MouseExit();
}

public interface IModule
{
    public void Active_Wdw();
    public void Close_Wdw();
}
public interface IBuildTool
{
    public void OnClick(Vector3Int pos);
    public void OnMove(Vector3Int pos);
    public void DeActive();
}
public abstract class Node
{
    public Vector3Int gridPos;
    public Node PreviousNode = null;
    public Node NextNode = null;
}
public interface IWindow
{
    public void Active(IModule module);    
    public void Close();    
}