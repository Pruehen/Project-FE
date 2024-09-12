using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class Belt : MonoBehaviour, IInteractable, ITransporter
{
    BeltNode beltNode;    

    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;

    [SerializeField] Transform itemStayPoint;
    ItemObject moveItemObject;

    ushort _mi_id;    

    float moveLogicSpeed = 2f;
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
    public bool TrySelect(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        return false;
    }
    public void DeSelect()
    {

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
        for (int i = 0; i < Prefab_BeltPart.Count; i++)
        {
            Prefab_BeltPart[i].SetActive((int)beltType == i);
        }

        this.beltNode = beltNode;
        timeValue_ItemMove = 0;        
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
        //moveLogicSpeed *= 2;
        moveLogicTime = 1 / moveLogicSpeed;
    }    

    public ushort MoveItemKey
    {
        get { return _mi_id; }
        set
        {
            if (_mi_id != value)
            {
                _mi_id = value;
                if (_mi_id == 0)
                {
                    ObjectPoolManager.Instance.EnqueueObject(moveItemObject.gameObject);
                }
                else
                {
                    moveItemObject = ObjectPoolManager.Instance.DequeueObject(JsonDataManager.GetItem(_mi_id).GetItemPrefab()).GetComponent<ItemObject>();
                }
            }            
        }
    }
    
    public bool CanItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (MoveItemKey == 0) return false;
        if (nextNode.CanItemIn(MoveItemKey) == false) return false;        
        if (timeValue_ItemMove <= moveLogicTime) return false;

        return true;
    }
    public void ItemOut(ITransporter nextNode)
    {
        nextNode.ItemIn(MoveItemKey, itemStayPoint.position);
        MoveItemKey = 0;
        timeValue_ItemMove -= moveLogicTime;
    }
    public bool CanItemIn(ushort itemId)
    {
        return MoveItemKey == 0;
    }
    public void ItemIn(ushort itemId, Vector3 inPos)
    {
        MoveItemKey = itemId;
        moveItemObject.SetPos(inPos, itemStayPoint.position);
    }
    public ushort GetItem()
    {
        return MoveItemKey;
    }

    float timeValue_ItemMove = 0;
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

        if (beltNode.NextNode != null)
        {
            beltNode.NextNode.transporter.ExcuteLogic_OnUpdate(deltaTime);
        }

        if (timeValue_ItemMove > moveLogicTime)//아이템이 도착했는지
        {
            if (beltNode.NextNode != null && CanItemOut(beltNode.NextNode.transporter))
            {
                ItemOut(beltNode.NextNode.transporter);
            }
            else
            {
                timeValue_ItemMove = moveLogicTime;
            }
        }

        if (MoveItemKey != 0)
        {            
            moveItemObject.ItemMove(timeValue_ItemMove * moveLogicSpeed);
            timeValue_ItemMove += deltaTime;
        }

        if (beltNode.PreviousNode != null)
        {
            beltNode.PreviousNode.transporter.ExcuteLogic_OnUpdate(deltaTime);
        }        
    }
}