using System.Collections.Generic;
using UnityEngine;

public class Inserter : MonoBehaviour, IInteractable
{
    [SerializeField] string ItemKey;
    [SerializeField] string BuildingKey;

    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] LineRenderer lineRenderer;

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
    public void Init(Vector3 startPos, Vector3 endPos)
    {
        start.transform.position = startPos + new Vector3(0, 0.8f, 0);
        end.transform.position = endPos + new Vector3(0, 0.8f, 0);
        lineRenderer.SetPosition(0, startPos + new Vector3(0, 0.6f, 0));
        lineRenderer.SetPosition(1, endPos + new Vector3(0, 0.6f, 0));
    }

    private void Awake()
    {
        _MainModule = GetComponent<IModule>();
    }    
}
