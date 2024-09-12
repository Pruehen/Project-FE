using System.Collections.Generic;
using UnityEngine;

public class Sorter : MonoBehaviour, IInteractable, ITransporter
{
    List<Node> inputNodeList;
    List<Node> outputNodeList;

    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;

    [SerializeField] ItemObject[] moveItemObjectArray;
    [SerializeField] Transform itemStayPoint;

    float moveLogicSpeed = 1f;
    float moveLogicTime;
    

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

    public void SetSorterPart(List<Node> inputNodeList, List<Node> outputNodeList)
    {
        this.inputNodeList = inputNodeList;
        this.outputNodeList = outputNodeList;
        for (int i = 0; i < 4; i++)
        {
            timeValueArray_ItemMove[i] = 0;
        }
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
        //moveLogicSpeed *= 2;
        moveLogicTime = 1 / moveLogicSpeed;
    }    
    void Update()
    {
        LogicInit();
        ExcuteLogic_OnUpdate(Time.deltaTime);
    }

    public bool CanItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (moveItemIdArray[nextOutItemIndex] == 0) return false;
        if (nextNode.CanItemIn(moveItemIdArray[nextOutItemIndex]) == false) return false;        

        return true;
    }
    public short GetItem()
    {
        return moveItemIdArray[nextOutItemIndex];
    }
    public void ItemOut(ITransporter nextNode)
    {
        Add_NextOutItemIndex();

        timeValueArray_ItemMove[nextOutItemIndex] = 0;
        
        nextNode.ItemIn(moveItemIdArray[nextOutItemIndex], itemStayPoint.position);
        moveItemIdArray[nextOutItemIndex] = 0;

        moveItemObjectArray[nextOutItemIndex].gameObject.SetActive(moveItemIdArray[nextOutItemIndex] != 0);

        itemHaveCount--;
    }
    public bool CanItemIn(short itemId)
    {
        return itemHaveCount < 4;
    }
    public void ItemIn(short itemId, Vector3 inPos)
    {
        Add_NextOInItemIndex();//nextInItemIndex 변경됨

        //로직 변수 설정
        moveItemIdArray[nextInItemIndex] = itemId;
        timeValueArray_ItemMove[nextInItemIndex] = 0;

        //그래픽 관련 변수 설정
        moveItemObjectArray[nextInItemIndex].gameObject.SetActive(moveItemIdArray[nextInItemIndex] != 0);
        moveItemObjectArray[nextInItemIndex].SetPos(inPos, itemStayPoint.position);

        itemHaveCount++;
    }
    
    int itemHaveCount = 0;
    float[] timeValueArray_ItemMove = { 0, 0, 0, 0 };
    short[] moveItemIdArray = { 0, 0, 0, 0 };

    int nextInItemIndex = 0;//선입 선출을 위한 인덱스 변수
    int nextOutItemIndex = 0;//선입 선출을 위한 인덱스 변수
    int nextOutPortIndex = 0;//아이템 균등 배출을 위한 인덱스 변수

    void Add_NextOInItemIndex()
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
            if (outputNodeList[nextOutPortIndex].transporter.CanItemIn(0))
            {
                transporter = outputNodeList[nextOutPortIndex].transporter;
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
                item.transporter.ExcuteLogic_OnUpdate(deltaTime);
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
            if (moveItemIdArray[i] != 0)
            {
                moveItemObjectArray[i].ItemMove(timeValueArray_ItemMove[i] * moveLogicSpeed);
                timeValueArray_ItemMove[i] += deltaTime;
            }
        }

        if (inputNodeList.Count > 0)
        {
            foreach (var item in inputNodeList)
            {
                item.transporter.ExcuteLogic_OnUpdate(deltaTime);
            }
        }
    }
}