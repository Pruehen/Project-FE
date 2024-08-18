using Newtonsoft.Json;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using System.Xml.Linq;

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

public class BuildingData
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public BuildingType BuildingType { get; private set; }
    [JsonProperty] public string RecipyGroupId { get; private set; }
    [JsonProperty] public float DeploySize_X { get; private set; }
    [JsonProperty] public float DeploySize_Y { get; private set; }
    [JsonProperty] public float DeploySize_Z { get; private set; }
    [JsonProperty] public bool IsUseEnergy { get; private set; }
    [JsonProperty] public float EnergyEfficiency { get; private set; }
    [JsonProperty] public float SpeedEfficiency { get; private set; }

    [JsonConstructor]
    public BuildingData(string id, BuildingType buildingType, string recipyGroupId, float deploySizeX, float deploySizeY, float deploySizeZ, bool isUseEnergy, float energyEfficiency, float speedEfficiency)
    {
        Id = id;
        BuildingType = buildingType;
        RecipyGroupId = recipyGroupId;
        DeploySize_X = deploySizeX;
        DeploySize_Y = deploySizeY;
        DeploySize_Z = deploySizeZ;
        IsUseEnergy = isUseEnergy;
        EnergyEfficiency = energyEfficiency;
        SpeedEfficiency = speedEfficiency;
    }

    public BuildingData(string id)
    {
        Id = id;
        BuildingType = BuildingType.Crafting;
        RecipyGroupId = "RG_Crafter_T1";
        DeploySize_X = 2;
        DeploySize_Y = 1.5f;
        DeploySize_Z = 2;
        IsUseEnergy = true;
        EnergyEfficiency = 1;
        SpeedEfficiency = 1;
    }
}

public class BuildingDataTable
{
    public Dictionary<string, BuildingData> dic;
    [JsonConstructor]
    public BuildingDataTable(Dictionary<string, BuildingData> dic)
    {
        this.dic = dic;
    }
    public BuildingDataTable()
    {
        dic = new Dictionary<string, BuildingData>();
    }
    public static string FilePath()
    {
        return "/Data/Table/Building/BuildingDataTable.json";
    }
}

public class JsonDataCreator : MonoBehaviour
{
    private void Awake()
    {
        JsonDataManager.jsonCache.Lode();

        //JsonDataManager.jsonCache.BuildingDataTableCache.dic.Add("Building_Crafter_T1", new BuildingData("Building_Crafter_T1"));
        //JsonDataManager.DataSaveCommand(JsonDataManager.jsonCache.BuildingDataTableCache, BuildingDataTable.FilePath());
    }
}