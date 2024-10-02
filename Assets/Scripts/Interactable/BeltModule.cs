using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class BeltModule : MonoBehaviour, ITransporter, IModule
{
    BeltNode beltNode;    
    
    [SerializeField] Transform itemStayPoint;
    ItemObject moveItemObject;

    ushort _mi_id;

    float beltSpeed = 2;
    float moveLogicTime;

    public List<GameObject> Prefab_BeltPart;

    public void SetBeltPart(BeltType beltType, BeltNode beltNode, float beltSpeed)
    {
        for (int i = 0; i < Prefab_BeltPart.Count; i++)
        {
            Prefab_BeltPart[i].SetActive((int)beltType == i);
        }

        this.beltNode = beltNode;
        timeValue_ItemMove = 0;

        this.beltSpeed = beltSpeed;
        moveLogicTime = 1 / beltSpeed;
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
                    ItemObjectManager.RemoveObject(moveItemObject);                    
                }
                else
                {
                    moveItemObject = ItemObjectManager.CreateObject(_mi_id);
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
            moveItemObject.ItemMove(timeValue_ItemMove * beltSpeed);
            timeValue_ItemMove += deltaTime;
        }

        if (beltNode.PreviousNode != null)
        {
            beltNode.PreviousNode.transporter.ExcuteLogic_OnUpdate(deltaTime);
        }        
    }

    #region IModule
    void IModule.Active_Wdw()
    {
        Debug.Log("구현되지 않은 메서드 호출됨");
    }

    void IModule.Close_Wdw()
    {
        Debug.Log("구현되지 않은 메서드 호출됨");
    }

    Inventory IModule.TryGetInputInventory()
    {
        Debug.Log("구현되지 않은 메서드 호출됨");
        return null;
    }

    Inventory IModule.TryGetOutputInventory()
    {
        Debug.Log("구현되지 않은 메서드 호출됨");
        return null;
    }
    #endregion
}