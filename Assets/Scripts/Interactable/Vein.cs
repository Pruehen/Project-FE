using EnumTypes;
using System;
using UnityEngine;

public class Vein : MonoBehaviour, IInteractable
{
    [SerializeField] public string itemKey;
    [SerializeField] public float extractTimeGain = 1;
    [SerializeField] public int reserves = 10000;

    Action<Vein> OnRemoveVein;
    public void Register_OnRemoveVein(Action<Vein> callback)
    {
        OnRemoveVein += callback;
    }

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

    public void ExtractVein(int count, out int extractCount)
    {
        extractCount = count;
        reserves -= count;

        if(reserves <= 0)
        {
            extractCount += reserves;
            RemoveVein();
        }
    }
    void RemoveVein()
    {
        OnRemoveVein.Invoke(this);
        OnRemoveVein = null;

        GridMap.Remove_Dic_VeinDepth(this.transform.position.ToVector3Int());
    }

    public string GetName()
    {
        string name = JsonDataManager.GetItem(itemKey).Name;
        return name;
    }
    public ushort GetItemKey()
    {
        return JsonDataManager.GetItem(itemKey).Id_UShort;
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
