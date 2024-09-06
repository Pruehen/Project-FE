using EnumTypes;
using UnityEngine;

public interface IInteractable
{
    public string GetName();
    public Vector3 GetPos(Vector3 hitPos);
    public float InteractSpeedGain();
    public bool TryInteract(Vector3 hitPos, Vector3 originPos, float checkRange);
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
    public NodeType nodeType;
    public ITransporter transporter;
    public Vector3Int gridPos;
    public Node PreviousNode = null;
    public Node NextNode = null;
}
public interface IWindow
{
    public void Active(IModule module);    
    public void Close();    
}

public interface ITransporter
{
    public bool TryItemOut(ITransporter nextNode);
    public bool CanItemIn();
    public void ItemIn(int itemId, Vector3 inPos);
    public void LogicInit();
    public void ExcuteLogic_OnUpdate(float deltaTime);
}