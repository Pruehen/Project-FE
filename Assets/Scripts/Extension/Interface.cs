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
public interface INode
{
    public NodeType NodeType { get; set; }
    public ITransporter Transporter { get; set; }
    public Vector3Int GridPos { get; set; }
    public INode PreviousNode { get; set; }
    public INode NextNode { get; set; }
    public void Init();
    public void Remove();
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