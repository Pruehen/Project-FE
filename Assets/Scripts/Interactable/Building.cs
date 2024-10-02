using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable
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
    public EntityType GetEntityType()
    {
        return EntityType.Building;
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
    public bool TrySelect(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        if (Vector3.Distance(originPos, GetPos(hitPos)) < checkRange)
        {
            _MainModule.Active_Wdw();
            Player.Instance.Command_CharactorInventoryOpen();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void DeSelect()
    {
        _MainModule.Close_Wdw();        
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
        if(Vector3.Distance(originPos, GetPos(hitPos)) < checkRange)
        {            
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
        //Vector3Int deployPos = this.transform.position.ToIntVector();

        //for (int x = 0; x < BuildingData.DeploySizeX; x++)
        //{
        //    for (int z = 0; z < BuildingData.DeploySizeZ; z++)
        //    {
        //        nodeTempList.Add(GridMap.CreateBuildingNode(deployPos + new Vector3Int(x, 0, z)));
        //    }
        //}
    }
}