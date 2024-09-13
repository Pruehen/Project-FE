using System.Collections.Generic;
using UnityEngine;

public static class ItemObjectManager
{
    static Dictionary<int, ItemObject> itemObjectDic = new Dictionary<int, ItemObject>();

    public static ItemObject CreateObject(ushort itemId)
    {
        GameObject obj = ObjectPoolManager.Instance.DequeueObject(JsonDataManager.GetItem(itemId).GetItemPrefab());
        int instanceId = obj.GetInstanceID();

        if(itemObjectDic.ContainsKey(instanceId) == false)
        {
            itemObjectDic.Add(instanceId, obj.GetComponent<ItemObject>());
        }
        
        return itemObjectDic[instanceId];
    }

    public static void RemoveObject(ItemObject itemObject)
    {
        ObjectPoolManager.Instance.EnqueueObject(itemObject.gameObject);
    }
}
