using EnumTypes;
using System;
using UnityEngine;

public interface IInteractable
{
    public string GetName();
    public EntityType GetEntityType();
    public Vector3 GetPos(Vector3 hitPos);
    public float InteractSpeedGain();
    public bool TryInteract(Vector3 hitPos, Vector3 originPos, float checkRange);
    public bool TrySelect(Vector3 hitPos, Vector3 originPos, float checkRange);
    public void DeSelect();
    public void MouseEnter();
    public void MouseExit();
}

public interface IModule
{
    public void Active_Wdw();
    public void Close_Wdw();
    public void OnBuildingInit();
    public void OnBuildingDismantle();
    public Inventory TryGetInputInventory();
    public Inventory TryGetOutputInventory();
}
public interface IBuildTool
{
    public void OnClick(Vector3Int pos);
    public void OnMove(Vector3Int pos);
    public void OnKeyDown(KeyCode key);
    public void SetBuildingData(BuildingData buildingData);
    public void DeActive();
}
public abstract class Node
{
    public NodeType nodeType { get; protected set; }
    public ITransporter transporter;
    public Vector3Int gridPos;
    public abstract Node PreviousNode { get; set; }
    public abstract Node NextNode { get; set; }
    public abstract void Init();
    public abstract void Remove();
}
public interface IWindow
{
    public void Active(IModule module);    
    public void Close();
    public void Command_Close();
}

public interface ITransporter//벨트, 인서터, 투입 가능 건물 등
{
    public bool CanItemOut(ITransporter nextNode);
    public void ItemOut(ITransporter nextNode);
    public bool CanItemIn(ushort itemId);
    public void ItemIn(ushort itemId, Vector3 inPos);
    public ushort GetItem();
    public void LogicInit();
    public void ExcuteLogic_OnUpdate(float deltaTime);
}