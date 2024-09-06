using System.Collections.Generic;
using UnityEngine;

public class Inserter : MonoBehaviour, IInteractable, ITransporter
{
    InserterNode node;

    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;

    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] GameObject grab;
    [SerializeField] LineRenderer lineRenderer;
    
    [SerializeField] ItemObject grabObject;        
    int _grab_id;

    float moveLogicSpeed = 6f;
    float moveLogicTime;

    Vector3 itemStayPoint_First;
    Vector3 itemStayPoint_Last;

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
    public void Init(Vector3 startPos, Vector3 endPos, InserterNode inserterNode)
    {
        node = inserterNode;

        this.transform.position = startPos;
        start.transform.position = startPos + new Vector3(0, 0.8f, 0);
        end.transform.position = endPos + new Vector3(0, 0.8f, 0);

        lineRenderer.SetPosition(0, startPos + new Vector3(0, 0.6f, 0));
        lineRenderer.SetPosition(1, endPos + new Vector3(0, 0.6f, 0));

        itemStayPoint_First = startPos + new Vector3(0, 0.6f, 0);
        itemStayPoint_Last = endPos + new Vector3(0, 0.6f, 0);

        moveLogicSpeed *= 2f / Vector3.Distance(itemStayPoint_First, itemStayPoint_Last);
        moveLogicTime = 1 / moveLogicSpeed;

        ItemIn(1, Vector3.zero);
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
    }
    void Update()
    {
        LogicInit();
        ExcuteLogic_OnUpdate(Time.deltaTime);
    }

    public int GrabObject
    {
        get { return _grab_id; }
        set
        {
            _grab_id = value;
            grabObject.gameObject.SetActive(_grab_id != 0);
        }
    }

    bool isExcuteLogic = false;
    bool State_ItemTransport = true;
    float timeValue;

    public bool TryItemOut(ITransporter nextNode)
    {
        if (nextNode == null) return false;
        if (nextNode.CanItemIn() == false) return false;
        if (GrabObject == 0) return false;

        nextNode.ItemIn(GrabObject, itemStayPoint_Last);
        GrabObject = 0;
        State_ItemTransport = false;
        return true;
    }
    public bool CanItemIn()
    {
        return GrabObject == 0;
    }
    public void ItemIn(int itemId, Vector3 inPos)
    {
        GrabObject = itemId;
        grabObject.SetPos(itemStayPoint_First, itemStayPoint_Last);
        State_ItemTransport = true;
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

        Node nextNode = node.NextNode;

        if (timeValue >= moveLogicTime)//아이템이 도착했는지
        {
            if (State_ItemTransport)
            {
                if (nextNode != null && TryItemOut(nextNode.transporter))
                {
                    timeValue -= moveLogicTime;                    
                }
                else
                {
                    timeValue = moveLogicTime;
                }
            }
            else
            {
                ItemIn(1, Vector3.zero);
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
}
