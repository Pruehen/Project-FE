using UnityEngine;

public class Building : MonoBehaviour, IInteractable
{
    [SerializeField] string Key;
    BuildingData _buildingData;
    public BuildingData BuildingData
    {
        get 
        {
            if(_buildingData == null)
            {
                _buildingData = JsonDataManager.GetBuilding(Key);
            }
            return _buildingData; 
        }
    }

    public string GetName()
    {
        ItemData data = JsonDataManager.GetItem(Key);
        if(data != null)
        {
            string name = data.Name;
            return name;
        }
        else
        {
            return "키를 찾을 수 없음";
        }
    }
    public Vector3 GetPos()
    {
        return this.transform.position;
    }
    public float InteractSpeedGain()
    {
        return 1;
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
