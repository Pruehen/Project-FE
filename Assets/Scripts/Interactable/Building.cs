using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable, ITransporter
{
    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;
    [SerializeField] List<Transform> validGridPos;
    Dictionary<Vector3Int, BuildingNode> buildingNodeDicTemp = new Dictionary<Vector3Int, BuildingNode>();

    BuildingData _buildingData;
    IModule _MainModule;

    Outline _outline;
    Outline Outline
    {
        get
        {
            if (_outline == null)
                _outline = GetComponent<Outline>();
            return _outline;
        }
    }

    public BuildingData BuildingData
    {
        get 
        {
            if(_buildingData == null)
            {
                _buildingData = JsonDataManager.GetBuilding(BuildingKey);
            }
            return _buildingData; 
        }
    }
    public List<string> RecipyGroupData
    {
        get
        {
            return JsonDataManager.GetRecipyGroupData(BuildingData.RecipyGroup);
        }
    }

    public string GetName()
    {
        ItemData data = JsonDataManager.GetItem(ItemKey);
        if(data != null)
        {
            string name = data.Name;
            return name;
        }
        else
        {
            return "키를 찾을 수 없음";
        }
    }
    public Vector3 GetPos(Vector3 hitPos)
    {
        if (validGridPos.Count > 0)
        {
            return validGridPos.FindClosest(hitPos).position;
        }
        else
        {
            return this.transform.position;
        }
    }
    public float InteractSpeedGain()
    {
        return 1;
    }
    public void Select()
    {
        UIManager.Instance.Active_BuildingMainModuleUIWdw(_MainModule);
    }

    public void MouseEnter()
    {
        Outline.IsOutlineEnabled = true;
    }

    public void MouseExit()
    {
        Outline.IsOutlineEnabled = false;
    }

    public bool TryInteract(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        if(Vector3.Distance(originPos, GetPos(hitPos)) > checkRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();

        foreach (var item in validGridPos)
        {
            buildingNodeDicTemp.Add(item.position.ToIntVector(), new BuildingNode(item.position.ToIntVector(), this));
        }
    }

    public bool TryItemOut(ITransporter nextNode)
    {
        return true;
    }

    public bool CanItemIn()
    {
        return true;
    }

    public void ItemIn(int itemId, Vector3 inPos)
    {
        
    }

    public void LogicInit() { }   
    public void ExcuteLogic_OnUpdate(float deltaTime) { }
}
public class BuildingNode : Node
{
    public override Node PreviousNode { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override Node NextNode { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public BuildingNode(Vector3Int gridPos, Building building)
    {
        this.nodeType = EnumTypes.NodeType.BuildingNode;
        this.gridPos = gridPos;
        GridMap.NodeDic_NormalDepth.Add(gridPos, this);

        transporter = building;
    }
    public override void Init()
    {

    }
    public override void Remove() { }
}