using UnityEngine;

public class InserterModule : MonoBehaviour, ITransporter, IModule
{
    InserterNode node;

    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] GameObject grab;
    [SerializeField] LineRenderer lineRenderer;
    
    [SerializeField] ItemObject grabObject;
    ushort _grab_id;

    float moveLogicSpeed = 1f;
    float moveLogicTime;

    Vector3 itemStayPoint_First;
    Vector3 itemStayPoint_Last;
   
    public void Init(Vector3 startPos, Vector3 endPos, InserterNode inserterNode)
    {
        node = inserterNode;

        //인서터 파츠의 좌표 설정
        this.transform.position = startPos;
        start.transform.position = startPos + new Vector3(0, 0.8f, 0);
        end.transform.position = endPos + new Vector3(0, 0.8f, 0);

        //파츠간의 라인을 그리는 임시 기능
        lineRenderer.SetPosition(0, startPos + new Vector3(0, 0.6f, 0));
        lineRenderer.SetPosition(1, endPos + new Vector3(0, 0.6f, 0));

        //아이템이 이동할 포지션
        itemStayPoint_First = startPos + new Vector3(0, 0.6f, 0);
        itemStayPoint_Last = endPos + new Vector3(0, 0.6f, 0);

        moveLogicSpeed *= 2f / Vector3.Distance(itemStayPoint_First, itemStayPoint_Last);
        moveLogicTime = 1 / moveLogicSpeed;

        timeValue = 0;
    }

    public ushort GrabObject
    {
        get { return _grab_id; }
        set
        {
            _grab_id = value;
            grabObject.gameObject.SetActive(_grab_id != 0);
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
        grabObject.SetPos((inPos == Vector3.zero) ? itemStayPoint_First : inPos, itemStayPoint_Last);
        State_ItemTransport = true;
    }
    public ushort GetItem()
    {
        return GrabObject;
    }
    void TryGrapItem()
    {
        if (node.PreviousNode == null)
            return;

        ITransporter grabTarget = node.PreviousNode?.transporter;
        ITransporter dropTarget = node.NextNode?.transporter;

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
        isExcuteLogic = false;
    }
    public void ExcuteLogic_OnUpdate(float deltaTime)
    {
        if (isExcuteLogic)
            return;
        isExcuteLogic = true;        

        if (timeValue >= moveLogicTime)//아이템이 도착했는지
        {
            if (State_ItemTransport)
            {
                if (node.NextNode != null && CanItemOut(node.NextNode.transporter))
                {
                    ItemOut(node.NextNode.transporter);
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
            grabObject.ItemMove(timeValue * moveLogicSpeed);            
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
            grab.transform.position = Vector3.Lerp(itemStayPoint_First, itemStayPoint_Last, lerpValue);
        }
        else
        {
            grab.transform.position = Vector3.Lerp(itemStayPoint_Last, itemStayPoint_First, lerpValue);
        }
    }

    #region IModule
    void IModule.Active_Wdw()
    {
        throw new System.NotImplementedException();
    }

    void IModule.Close_Wdw()
    {
        throw new System.NotImplementedException();
    }

    Inventory IModule.TryGetInputInventory()
    {
        throw new System.NotImplementedException();
    }

    Inventory IModule.TryGetOutputInventory()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}