using EnumTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable
{
    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;

    public List<Transform> occupiedNodeList;

    BuildingData _buildingData;

    public IModule MainModule { get; private set; }
    public ITransporter Transporter { get; private set; }

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
        if (occupiedNodeList.Count > 0)
        {
            return occupiedNodeList.FindClosest(hitPos).position;
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
            MainModule.Active_Wdw();
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
        MainModule.Close_Wdw();        
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

    public void Init()
    {
        MainModule = GetComponent<IModule>();
        Transporter = GetComponent<ITransporter>();

        MainModule.OnBuildingInit();
    }
    public void Register_OnDismantle(Action OnDismantle)
    {
        this.OnDismantle += OnDismantle;
    }

    Action OnDismantle;
    public void Dismantle()
    {
        OnDismantle?.Invoke();
        OnDismantle = null;

        MainModule.OnBuildingDismantle();
        MainModule.Close_Wdw();
        Player.Instance.GetItem(ItemKey);
    }
}