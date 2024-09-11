using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable, ITransporter
{
    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;
    
    List<Node> nodeTempList = new List<Node>();

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
        if (nodeTempList.Count > 0)
        {
            return nodeTempList.FindClosest(hitPos).gridPos;
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
        Vector3Int deployPos = this.transform.position.ToIntVector();

        for (int x = 0; x < BuildingData.DeploySizeX; x++)
        {
            for (int z = 0; z < BuildingData.DeploySizeZ; z++)
            {
                nodeTempList.Add(GridMap.CreateBuildingNode(deployPos + new Vector3Int(x, 0, z), this));
            }
        }
    }

    public bool CanItemOut(ITransporter nextNode)
    {        
        Inventory outputinventory = _MainModule.TryGetOutputInventory();
        if(outputinventory != null)
        {
            return outputinventory.CanGrabItem();
        }
        else
        {
            return false;
        }        
    }
    public void ItemOut(ITransporter nextNode)
    {
        Inventory outputinventory = _MainModule.TryGetOutputInventory();
        if (outputinventory != null)
        {
            outputinventory.GrabItem(out int itemId, out int itemCount);
            nextNode.ItemIn(itemId, Vector3.zero);
        }
    }

    public bool CanItemIn(int itemId)
    {
        Inventory inputInventory = _MainModule.TryGetInputInventory();
        if (inputInventory != null)
        {
            return inputInventory.CanAddItem(itemId, 1);
        }
        else
        {
            return false;
        }
    }

    public void ItemIn(int itemId, Vector3 inPos)
    {
        Inventory inputInventory = _MainModule.TryGetInputInventory();
        if (inputInventory != null)
        {
            inputInventory.AddItem(itemId, 1, out int r);
        }
    }
    public int GetItem()
    {
        Inventory outputinventory = _MainModule.TryGetOutputInventory();
        if (outputinventory != null)
        {
            return outputinventory.GetNextGrabItem();
        }
        else
        {
            return 0;
        }
    }

    public void LogicInit() { }   
    public void ExcuteLogic_OnUpdate(float deltaTime) { }
}
public class BuildingNode : Node
{
    public override Node PreviousNode { get { return null; } set { } }
    public override Node NextNode { get { return null; } set { } }

    public BuildingNode(Vector3Int gridPos, Building building)
    {
        this.nodeType = EnumTypes.NodeType.BuildingNode;
        this.gridPos = gridPos;
        this.transporter = building;
    }

    public override void Init()
    {

    }

    public override void Remove() 
    { 

    }
}