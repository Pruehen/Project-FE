using UnityEngine;

public class Building : MonoBehaviour, IInteractable
{
    [SerializeField] int itemKey;

    public string GetName()
    {
        string name = JsonDataManager.GetItem(itemKey).Name;
        return name;
    }
    public Vector3 GetPos()
    {
        return this.transform.position;
    }
    public bool TryInteract(Vector3 originPos, float checkRange)
    {
        if(Vector3.Distance(originPos, GetPos()) > checkRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
