using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System;

public class Belt : MonoBehaviour, IInteractable, ITransporter
{
    BeltNode node;

    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;
    [SerializeField] List<Transform> ItemStayPointList;

    public Transform itemStayPointMid {  get; private set; }
    public Transform itemStayPointLast { get; private set; }

    ItemObject _firstToMidObject;
    ItemObject _midToLastObject;

    float moveLogicSpeed = 1f;
    float moveLogicTime;

    public List<GameObject> Prefab_BeltPart;

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
            if (_buildingData == null)
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
        if (data != null)
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
        return this.transform.position;
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
        if (Vector3.Distance(originPos, GetPos(hitPos)) > checkRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetBeltPart(BeltType beltType, BeltNode beltNode)
    {
        node = beltNode;

        for (int i = 0; i < Prefab_BeltPart.Count; i++)
        {
            Prefab_BeltPart[i].SetActive((int)beltType == i);
        }

        switch (beltType)
        {
            case BeltType.Start:
            case BeltType.End:
            case BeltType.Mid:
            case BeltType.Merge:
                itemStayPointMid = ItemStayPointList[1];
                itemStayPointLast = ItemStayPointList[2];
                break;
            case BeltType.Left:
                itemStayPointMid = ItemStayPointList[1];
                itemStayPointLast = ItemStayPointList[2];
                break;
            case BeltType.Right:
                itemStayPointMid = ItemStayPointList[1];
                itemStayPointLast = ItemStayPointList[2];
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
        moveLogicSpeed *= 2;
        moveLogicTime = 1 / moveLogicSpeed;
    }

    public ItemObject FirstToMidObject
    {
        get { return _firstToMidObject; }
        set
        {
            _firstToMidObject = value;
            if (itemStayPointMid != null && _firstToMidObject != null)
            {
                _firstToMidObject.SetPos(itemStayPointMid.position);
            }
        }
    }
    public ItemObject MidToLastObject
    {
        get { return _midToLastObject; }
        set
        {
            _midToLastObject = value;
            if (itemStayPointLast != null && _midToLastObject != null)
            {
                _midToLastObject.SetPos(itemStayPointLast.position);
            }
        }
    }

    public bool TryItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (nextNode.CanItemIn() == false) return false;
        if (MidToLastObject == null) return false;

        nextNode.ItemIn(MidToLastObject);
        MidToLastObject = null;
        return true;
    }
    public bool CanItemIn()
    {
        return FirstToMidObject == null;
    }
    public void ItemIn(ItemObject inItem)
    {
        FirstToMidObject = inItem;
    }

    float timeValue_firstToMid = 0;
    float timeValue_midToLast = 0;
    bool isExcuteLogic = false;
    public void LogicInit()
    {
        isExcuteLogic = false;
    }
    public void ExcuteLogic_OnUpdate(float deltaTime)
    {
        if (isExcuteLogic)
            return;

        isExcuteLogic = true;

        BeltNode nextBelt = node.NextNode as BeltNode;
        BeltNode previousNode = node.PreviousNode as BeltNode;
        

        if (timeValue_midToLast >= moveLogicTime)//마지막 아이템이 도착했는지
        {
            if (nextBelt != null && TryItemOut(nextBelt.beltPart))
            {
                timeValue_midToLast -= moveLogicTime;
            }
            else
            {
                timeValue_midToLast = moveLogicTime;
            }
        }
        if (timeValue_firstToMid >= moveLogicTime)//중간 아이템이 도착했는지 : 해당 아이템을 인서터가 잡을 수 있는지
        {
            if (FirstToMidObject != null && MidToLastObject == null)
            {
                MidToLastObject = FirstToMidObject;
                FirstToMidObject = null;
                timeValue_firstToMid -= moveLogicTime;
            }
            else
            {
                timeValue_firstToMid = moveLogicTime;
            }
        }

        if (FirstToMidObject != null)
        {            
            FirstToMidObject.ItemMove(timeValue_firstToMid * moveLogicSpeed);
            timeValue_firstToMid += deltaTime;
        }
        if (MidToLastObject != null)
        {                        
            MidToLastObject.ItemMove(timeValue_midToLast * moveLogicSpeed);
            timeValue_midToLast += deltaTime;
        }
        
        if (previousNode != null)
        {
            previousNode.beltPart.ExcuteLogic_OnUpdate(deltaTime);
        }
    }

    public Action OnItemPosMid;
}
