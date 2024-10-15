using EnumTypes;
using System.Collections.Generic;
using UnityEngine;

public class SorterModule : MonoBehaviour, ITransporter, IModule
{
    List<INode> inputNodeList;
    List<INode> outputNodeList;
    
    [SerializeField] Transform itemStayPoint;

    ItemObject[] moveItemObjectArray = new ItemObject[4];

    float moveLogicSpeed = 2f;
    float moveLogicTime;    

    public void SetSorterPart(List<INode> inputNodeList, List<INode> outputNodeList)
    {
        this.inputNodeList = inputNodeList;
        this.outputNodeList = outputNodeList;
    }
    public void OnBuildingInit()
    {
        for (int i = 0; i < 4; i++)
        {
            timeValueArray_ItemMove[i] = 0;
        }

        moveLogicTime = 1 / moveLogicSpeed;
    }
    public void OnBuildingDismantle()
    {
        for (int i = 0; i < 4; i++)
        {
            if (moveItemIdArray[i] != 0)
            {
                Player.Instance.GetItem(moveItemIdArray[i]);
            }
            moveItemObjectArray[nextOutItemIndex]?.RemoveObject();
        }
    }

    public bool CanItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (moveItemIdArray[nextOutItemIndex] == 0) return false;
        if (nextNode.CanItemIn(moveItemIdArray[nextOutItemIndex]) == false) return false;        

        return true;
    }
    public ushort GetItem()
    {
        return moveItemIdArray[nextOutItemIndex];
    }
    public void ItemOut(ITransporter nextNode)
    {
        timeValueArray_ItemMove[nextOutItemIndex] = 0;
        
        nextNode.ItemIn(moveItemIdArray[nextOutItemIndex], itemStayPoint.position);
        moveItemIdArray[nextOutItemIndex] = 0;

        moveItemObjectArray[nextOutItemIndex]?.RemoveObject();
        //moveItemObjectArray[nextOutItemIndex].gameObject.SetActive(moveItemIdArray[nextOutItemIndex] != 0);

        Add_NextOutItemIndex();
        itemHaveCount--;
    }
    public bool CanItemIn(ushort itemId)
    {
        return itemHaveCount < 4;
    }
    public void ItemIn(ushort itemId, Vector3 inPos)
    {
        //로직 변수 설정
        moveItemIdArray[nextInItemIndex] = itemId;
        timeValueArray_ItemMove[nextInItemIndex] = 0;

        //그래픽 관련 변수 설정 : 아이템 생성
        if(itemId != 0)
        {
            moveItemObjectArray[nextInItemIndex] = ItemObjectManager.CreateObject(itemId);
            moveItemObjectArray[nextInItemIndex].SetPos(inPos, itemStayPoint.position);
        }

        Add_NextInItemIndex();//nextInItemIndex 변경됨
        itemHaveCount++;
    }
    
    int itemHaveCount = 0;
    float[] timeValueArray_ItemMove = { 0, 0, 0, 0 };
    ushort[] moveItemIdArray = { 0, 0, 0, 0 };

    int nextInItemIndex = 0;//선입 선출을 위한 인덱스 변수
    int nextOutItemIndex = 0;//선입 선출을 위한 인덱스 변수
    int nextOutPortIndex = 0;//아이템 균등 배출을 위한 인덱스 변수

    void Add_NextInItemIndex()
    {
        nextInItemIndex++;
        if (nextInItemIndex >= 4)
        {
            nextInItemIndex = 0;
        }
    }
    void Add_NextOutItemIndex()
    {
        nextOutItemIndex++;
        if (nextOutItemIndex >= 4)
        {
            nextOutItemIndex = 0;
        }
    }
    void Add_NextOutPortIndex()
    {
        nextOutPortIndex++;
        if (nextOutPortIndex >= outputNodeList.Count)
        {
            nextOutPortIndex = 0;
        }
    }
    ITransporter Find_ValidOutPort()
    {
        ITransporter transporter = null;
        for (int i = 0; i < outputNodeList.Count; i++)
        {
            if (outputNodeList[nextOutPortIndex].Transporter.CanItemIn(0))
            {
                transporter = outputNodeList[nextOutPortIndex].Transporter;
                Add_NextOutPortIndex();
                break;
            }
            else
            {
                Add_NextOutPortIndex();
            }
        }
        return transporter;
    }

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

        if (outputNodeList.Count > 0)
        {
            foreach (var item in outputNodeList)
            {
                item.Transporter.ExcuteLogic_OnUpdate(deltaTime);
            }
        }

        for (int i = 0; i < 4; i++)
        {            
            if (timeValueArray_ItemMove[i] > moveLogicTime)//아이템이 도착했는지
            {
                ITransporter targetTransporter = Find_ValidOutPort();
                if (CanItemOut(targetTransporter))
                {
                    ItemOut(targetTransporter);
                    timeValueArray_ItemMove[i] -= moveLogicTime;
                }
                else
                {
                    timeValueArray_ItemMove[i] = moveLogicTime;
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (moveItemIdArray[i] != 0 && moveItemObjectArray[i] != null)
            {
                moveItemObjectArray[i].ItemMove(timeValueArray_ItemMove[i] * moveLogicSpeed);
                timeValueArray_ItemMove[i] += deltaTime;
            }
        }

        if (inputNodeList.Count > 0)
        {
            foreach (var item in inputNodeList)
            {
                item.Transporter.ExcuteLogic_OnUpdate(deltaTime);
            }
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