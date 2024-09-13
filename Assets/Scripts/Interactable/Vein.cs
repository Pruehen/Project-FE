using EnumTypes;
using UnityEngine;

public class Vein : MonoBehaviour, IInteractable
{
    [SerializeField] string itemKey;
    [SerializeField] public float extractTimeGain = 1;
    [SerializeField] int reserves = 10000;

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

    public string GetName()
    {
        string name = JsonDataManager.GetItem(itemKey).Name;
        return name;
    }
    public EntityType GetEntityType()
    {
        return EntityType.Vein;
    }
    public Vector3 GetPos(Vector3 hitPos)
    {
        return this.transform.position;
    }
    public float InteractSpeedGain()
    {
        return extractTimeGain;
    }
    public bool TrySelect(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        return false;
    }
    public void DeSelect()
    {

    }
    public bool TryInteract(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        if(Vector3.Distance(originPos, this.transform.position) > checkRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void MouseEnter()
    {
        Outline.IsOutlineEnabled = true;
    }

    public void MouseExit()
    {
        Outline.IsOutlineEnabled = false;
    }
}
