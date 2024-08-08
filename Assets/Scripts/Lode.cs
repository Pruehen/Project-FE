using UnityEngine;

public class Lode : MonoBehaviour, IInteractable
{
    [SerializeField] int itemKey;
    [SerializeField] float extractTimeGain = 1;
    [SerializeField] int reserves = 10000;

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
