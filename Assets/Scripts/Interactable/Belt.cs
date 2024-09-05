using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System;

public class Belt : MonoBehaviour, IInteractable
{
    BeltNode node;

    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;
    [SerializeField] List<Transform> ItemStayPointList;
    public GameObject TestPrefab_ItemObject;

    public Transform itemStayPointMid {  get; private set; }
    public Transform itemStayPointLast { get; private set; }

    ItemObject _firstToMidObject;
    ItemObject _midToLastObject;

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

    float movePerSec = 1f;
    float moveTime;

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
    public Vector3 GetPos()
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

    public bool TryInteract(Vector3 originPos, float checkRange)
    {
        if (Vector3.Distance(originPos, GetPos()) > checkRange)
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

        MidToLastObject = Instantiate(TestPrefab_ItemObject).GetComponent<ItemObject>();
        MidToLastObject.SetPos(itemStayPointMid.position, itemStayPointLast.position);
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
        moveTime = 0.5f / movePerSec;
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
        

        if (timeValue_midToLast >= moveTime)
        {
            if (MidToLastObject != null && nextBelt != null && nextBelt.beltPart.FirstToMidObject == null)
            {
                nextBelt.beltPart.FirstToMidObject = MidToLastObject;
                MidToLastObject = null;
                timeValue_midToLast -= moveTime;
            }
            else
            {
                timeValue_midToLast = moveTime;
            }
        }
        if (timeValue_firstToMid >= moveTime)
        {
            if (FirstToMidObject != null && MidToLastObject == null)
            {
                MidToLastObject = FirstToMidObject;
                FirstToMidObject = null;
                timeValue_firstToMid -= moveTime;
            }
            else
            {
                timeValue_firstToMid = moveTime;
            }
        }

        if (FirstToMidObject != null)
        {
            timeValue_firstToMid += deltaTime;
            FirstToMidObject.transform.position = Vector3.Lerp(FirstToMidObject.startPos, FirstToMidObject.targetPos, timeValue_firstToMid * movePerSec * 2);
        }
        if (MidToLastObject != null)
        {
            timeValue_midToLast += deltaTime;
            MidToLastObject.transform.position = Vector3.Lerp(MidToLastObject.startPos, MidToLastObject.targetPos, timeValue_midToLast * movePerSec * 2);
        }
        
        if (previousNode != null)
        {
            previousNode.beltPart.ExcuteLogic_OnUpdate(deltaTime);
        }
    }

    public Action OnItemPosMid;
}
