using UnityEngine;

public class InserterModule : MonoBehaviour, ITransporter, IModule
{
    Node PreviousNode { get; set; }
    Node NextNode { get; set; }

    [SerializeField] Transform startNodePos;
    [SerializeField] Transform endNodePos;
    [SerializeField] Transform grabTrf;
    
    ItemObject grabItemObject;
    ushort _grab_id;

    float moveLogicSpeed;
    float moveLogicTime;

    Vector3 itemStayPoint_First;
    Vector3 itemStayPoint_Last;

    public void OnBuildingInit()
    {        
        //아이템이 이동할 포지션
        itemStayPoint_First = startNodePos.position + new Vector3(0, 0.4f, 0);
        itemStayPoint_Last = endNodePos.position + new Vector3(0, 0.4f, 0);

        PreviousNode = FindNode_OnUpdate(startNodePos.position.ToVector3Int());
        NextNode = FindNode_OnUpdate(endNodePos.position.ToVector3Int());

        this.moveLogicSpeed = 1;//TODO : 인서터 동작 속도를 테이블에서 가져올 것
        this.moveLogicSpeed *= 2f / Vector3.Distance(itemStayPoint_First, itemStayPoint_Last);
        moveLogicTime = 1 / this.moveLogicSpeed;

        timeValue = 0;
    }
    public void OnBuildingDismantle()
    {
        Player.Instance.GetItem(GrabObject);
        GrabObject = 0;
    }
    public ushort GrabObject
    {
        get { return _grab_id; }
        set
        {
            _grab_id = value;
            if (_grab_id == 0)
            {
                grabItemObject?.RemoveObject();
            }
            else
            {
                grabItemObject = ItemObjectManager.CreateObject(_grab_id);
            }
        }
    }

    bool isExcuteLogic = false;
    bool State_ItemTransport = false;
    float timeValue = 0;

    public bool CanItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (GrabObject == 0) return false;
        if (nextNode.CanItemIn(GrabObject) == false) return false;        

        return true;
    }
    public void ItemOut(ITransporter nextNode)
    {
        nextNode.ItemIn(GrabObject, itemStayPoint_Last);
        GrabObject = 0;
        State_ItemTransport = false;        
    }
    public bool CanItemIn(ushort itemId)
    {
        return GrabObject == 0;
    }
    public void ItemIn(ushort itemId, Vector3 inPos)
    {
        GrabObject = itemId;
        grabItemObject.SetPos((inPos == Vector3.zero) ? itemStayPoint_First : inPos, itemStayPoint_Last);
        State_ItemTransport = true;
    }
    public ushort GetItem()
    {
        return GrabObject;
    }
    void TryGrapItem()
    {
        if (PreviousNode == null)
            return;

        ITransporter grabTarget = PreviousNode?.transporter;
        ITransporter dropTarget = NextNode?.transporter;

        if (grabTarget != null && dropTarget != null)
        {
            if (grabTarget.CanItemOut(this) && dropTarget.CanItemIn(grabTarget.GetItem()))
            {
                grabTarget.ItemOut(this);
            }
        }        
    }
    public void LogicInit()
    {
        //isExcuteLogic = false;
    }
    public void ExcuteLogic_OnUpdate(float deltaTime)
    {      
        if(NextNode == null)
        {
            NextNode = FindNode_OnUpdate(endNodePos.position.ToVector3Int());
            return;
        }
        if(PreviousNode == null)
        {
            PreviousNode = FindNode_OnUpdate(startNodePos.position.ToVector3Int());
            return;
        }


        if (timeValue >= moveLogicTime)//아이템이 도착했는지
        {
            if (State_ItemTransport)
            {
                if (NextNode != null && CanItemOut(NextNode.transporter))
                {
                    ItemOut(NextNode.transporter);
                    timeValue -= moveLogicTime;                    
                }
                else
                {
                    timeValue = moveLogicTime;
                }
            }
            else
            {
                TryGrapItem();
                if (GrabObject != 0)
                {
                    timeValue -= moveLogicTime;
                }
                else
                {
                    timeValue = moveLogicTime;
                }
            }
        }

        GrabMove(timeValue * moveLogicSpeed);

        if (GrabObject != 0 && State_ItemTransport)
        {
            grabItemObject.ItemMove(timeValue * moveLogicSpeed);            
            timeValue += deltaTime;
        }
        else if(GrabObject == 0 && State_ItemTransport == false)
        {
            timeValue += deltaTime;
        }
    }

    void GrabMove(float lerpValue)
    {
        if (State_ItemTransport)
        {
            grabTrf.position = Vector3.Lerp(itemStayPoint_First, itemStayPoint_Last, lerpValue);
        }
        else
        {
            grabTrf.position = Vector3.Lerp(itemStayPoint_Last, itemStayPoint_First, lerpValue);
        }
    }
    Node FindNode_OnUpdate(Vector3Int findPos)
    {
        if(GridMap.Dic_OccupiedDepth.ContainsKey(findPos))
        {
            return GridMap.Dic_OccupiedDepth[findPos];
        }
        else
        {
            return null;
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