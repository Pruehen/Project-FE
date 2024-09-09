using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System;

public class Belt : MonoBehaviour, IInteractable, ITransporter
{
    BeltNode node;
    BeltNode nextBelt;
    BeltNode previousNode;

    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;
    [SerializeField] List<Transform> ItemStayPointList;

    public Transform itemStayPointMid {  get; private set; }
    public Transform itemStayPointLast { get; private set; }

    [SerializeField] ItemObject firstToMidObject;
    [SerializeField] ItemObject midToLastObject;

    int _fm_id;
    int _ml_id;

    float moveLogicSpeed = 6f;
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

        nextBelt = node.NextNode as BeltNode;
        previousNode = node.PreviousNode as BeltNode;

        itemStayPointMid = ItemStayPointList[0];
        itemStayPointLast = ItemStayPointList[1];
        midToLastObject.SetPos(itemStayPointMid.position, itemStayPointLast.position);
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
        moveLogicSpeed *= 2;
        moveLogicTime = 1 / moveLogicSpeed;
    }    
    void Update()
    {
        LogicInit();
        ExcuteLogic_OnUpdate(Time.deltaTime);
    }

    public int FirstToMidId
    {
        get { return _fm_id; }
        set
        {
            _fm_id = value;
            firstToMidObject.gameObject.SetActive(_fm_id != 0);
        }
    }
    public int MidToLastId
    {
        get { return _ml_id; }
        set
        {
            _ml_id = value;
            midToLastObject.gameObject.SetActive(_ml_id != 0);
        }
    }

    public bool TryItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (nextNode.CanItemIn() == false) return false;
        if (MidToLastId == 0) return false;

        nextNode.ItemIn(MidToLastId, itemStayPointLast.position);
        MidToLastId = 0;
        return true;
    }
    public bool CanItemIn()
    {
        return FirstToMidId == 0;
    }
    public void ItemIn(int itemId, Vector3 inPos)
    {
        FirstToMidId = itemId;
        firstToMidObject.SetPos(inPos, itemStayPointMid.position);
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
        
        if (timeValue_midToLast > moveLogicTime)//마지막 아이템이 도착했는지
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
        if (timeValue_firstToMid > moveLogicTime)//중간 아이템이 도착했는지 : 해당 아이템을 인서터가 잡을 수 있는지
        {
            if (FirstToMidId != 0 && MidToLastId == 0)
            {
                MidToLastId = FirstToMidId;
                FirstToMidId = 0;
                timeValue_firstToMid -= moveLogicTime;
            }
            else
            {
                timeValue_firstToMid = moveLogicTime;
            }
        }

        if (FirstToMidId != 0)
        {            
            firstToMidObject.ItemMove(timeValue_firstToMid * moveLogicSpeed);
            timeValue_firstToMid += deltaTime;
        }
        if (MidToLastId != 0)
        {                        
            midToLastObject.ItemMove(timeValue_midToLast * moveLogicSpeed);
            timeValue_midToLast += deltaTime;
        }
        
        if (previousNode != null)
        {
            previousNode.beltPart.ExcuteLogic_OnUpdate(deltaTime);
        }
    }

    public Action OnItemPosMid;
}