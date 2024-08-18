using Newtonsoft.Json;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class ItemData
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public ItemType ItemType { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Desc { get; private set; }
    [JsonProperty] public int MaxStack { get; private set; }
    [JsonProperty] public float EnergyReserves { get; private set; }

    [JsonProperty] public string Icon_Path { get; private set; }
    [JsonProperty] public string ItemMesh_Path { get; private set; }
    [JsonProperty] public string DropMesh_Path { get; private set; }

    [JsonConstructor]
    public ItemData(string id, ItemType itemType, string name, string desc, int maxStack, float energyReserves, string iconPath, string itemMeshPath, string dropMeshPath )
    {
        Id = id;
        ItemType = itemType;
        Name = name;
        Desc = desc;
        MaxStack = maxStack;
        EnergyReserves = energyReserves;
        Icon_Path = iconPath;
        ItemMesh_Path = itemMeshPath;
        DropMesh_Path = dropMeshPath;
    }
    public ItemData(string id)
    {
        Id = id;
        ItemType = ItemType.Resource;
        Name = "Text_Iron_Name";
        Desc = "Text_Iron_Desc";
        MaxStack = 100;
        EnergyReserves = 0;
        Icon_Path = "UI/Icon/ItemData/Icon_Iron";
        ItemMesh_Path = "Prefabs/Iron";
        DropMesh_Path = "Prefabs/Fe";
    }
    public ItemData()
    {
        Id = "Item_Iron";
        ItemType = ItemType.Resource;
        Name = "Text_Iron_Name";
        Desc = "Text_Iron_Desc";
        MaxStack = 100;
        EnergyReserves = 0;
        Icon_Path = "UI/Icon/ItemData/Icon_Iron";
        ItemMesh_Path = "Prefabs/Iron";
        DropMesh_Path = "Prefabs/Fe";
    }
}
public class ItemDataTable
{
    public Dictionary<string, ItemData> dic;
    [JsonConstructor]
    public ItemDataTable(Dictionary<string, ItemData> dic)
    {
        this.dic = dic;
    }
    public ItemDataTable()
    {
        dic = new Dictionary<string, ItemData>();
    }
    public static string FilePath()
    {
        return "/Data/Table/Item/ItemDataTable.json";
    }
}

public class JsonDataCreator : MonoBehaviour
{
    private void Awake()
    {
        JsonDataManager.jsonCache.Lode();
        JsonDataManager.jsonCache.ItemDataTableCache.dic.Add("Item_Iron", new ItemData("Item_Iron"));
        JsonDataManager.jsonCache.ItemDataTableCache.dic.Add("Item_Copper", new ItemData("Item_Copper"));
        JsonDataManager.DataSaveCommand(JsonDataManager.jsonCache.ItemDataTableCache, ItemDataTable.FilePath());
    }
}