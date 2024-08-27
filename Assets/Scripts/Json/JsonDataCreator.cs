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

    [JsonProperty("Icon")] public string Icon_Path { get; private set; }
    [JsonProperty("ItemMesh")] public string ItemMesh_Path { get; private set; }
    [JsonProperty("DropMesh")] public string DropMesh_Path { get; private set; }

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
        dic.Add("Item_Iron", new ItemData());
    }
    public static string FilePath()
    {
        return "/Data/Table/Item/ItemData.json";
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

    public BuildingData()
    {
        Id = "Building_Crafter_T1";
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
        dic.Add("Building_Crafter_T1", new BuildingData());
    }
    public static string FilePath()
    {
        return "/Data/Table/Building/BuildingDataTable.json";
    }
}
public class RecipyGroupData
{
    [JsonProperty] public string GroupId { get; private set; }
    [JsonProperty] public List<string> RecipyList { get; private set; }

    [JsonConstructor]
    public RecipyGroupData(string groupId, List<string> recipyList)
    {
        this.GroupId = groupId;
        RecipyList = recipyList;
    }
    public RecipyGroupData()
    {
        this.GroupId = "RG_Crafter_T1";
        RecipyList = new List<string>();

        RecipyList.Add("Recipy_IronPlate");
        RecipyList.Add("Recipy_IronPlate_Alternative");
        RecipyList.Add("Recipy_CopperPlate");
        RecipyList.Add("Recipy_CopperPlate_Alternative");
        RecipyList.Add("Recipy_Crafter");
        RecipyList.Add("Recipy_Miner");
        RecipyList.Add("Recipy_Refinery");
        RecipyList.Add("Recipy_Belt");
        RecipyList.Add("Recipy_Inserter");
        RecipyList.Add("Recipy_Generator");
    }
}
public class RecipyGroupDataTable
{
    public Dictionary<string, RecipyGroupData> dic;
    [JsonConstructor]
    public RecipyGroupDataTable(Dictionary<string, RecipyGroupData> dic)
    {
        this.dic = dic;
    }
    public RecipyGroupDataTable()
    {
        dic = new Dictionary<string, RecipyGroupData>();
        dic.Add("RG_Crafter_T1", new RecipyGroupData());
    }
    public static string FilePath()
    {
        return "/Data/Table/Recipy/RecipyGroupDataTable.json";
    }
}


public class RecipyData
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Desc { get; private set; }
    [JsonProperty] public string Icon_Path { get; private set; }
    [JsonProperty] public List<ItemGroup_UsedInRecipe> InputItemGroup { get; private set; }
    [JsonProperty] public List<ItemGroup_UsedInRecipe> OutputItemGroup { get; private set; }
    [JsonProperty] public float CraftingTime { get; private set; }

    [JsonConstructor]
    public RecipyData(string id, string name, string desc, string icon_Path, List<ItemGroup_UsedInRecipe> inputItemGroup, List<ItemGroup_UsedInRecipe> outputItemGroup, float craftingTime)
    {
        this.Id = id;
        this.Name = name;
        this.Desc = desc;
        this.Icon_Path = icon_Path;
        this.InputItemGroup = inputItemGroup;
        this.OutputItemGroup = outputItemGroup;
        this.CraftingTime = craftingTime;
    }
    public RecipyData()
    {
        this.Id = "Recipy_IronPlate";
        this.Name = "Text_Recipy_IronPlate_Name";
        this.Desc = "Text_Recipy_IronPlate_Desc";
        this.Icon_Path = "UI/Icon/Recipy/Icon_Recipy_IronPlate";
        this.InputItemGroup = new List<ItemGroup_UsedInRecipe>();
        this.OutputItemGroup = new List<ItemGroup_UsedInRecipe>();
        this.CraftingTime = 1;

        InputItemGroup.Add(new ItemGroup_UsedInRecipe("Item_Iron", 1));
        InputItemGroup.Add(new ItemGroup_UsedInRecipe("Item_Iron", 1));
        OutputItemGroup.Add(new ItemGroup_UsedInRecipe("Item_Iron", 1));
        OutputItemGroup.Add(new ItemGroup_UsedInRecipe("Item_Iron", 1));
    }
}
public struct ItemGroup_UsedInRecipe
{
    public string Id;
    public int Count;

    public ItemGroup_UsedInRecipe(string id, int count)
    {
        Id = id;
        Count = count;
    }
}

public class RecipyDataTable
{
    public Dictionary<string, RecipyData> dic;
    [JsonConstructor]
    public RecipyDataTable(Dictionary<string, RecipyData> dic)
    {
        this.dic = dic;
    }
    public RecipyDataTable()
    {
        dic = new Dictionary<string, RecipyData>();
        dic.Add("Recipy_IronPlate", new RecipyData());
    }
    public static string FilePath()
    {
        return "/Data/Table/Recipy/RecipyDataTable.json";
    }
}

public class TextData
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public string Text_Kr { get; private set; }

    [JsonConstructor]
    public TextData(string id, string text_kr)
    {
        this.Id = id;
        this.Text_Kr = text_kr;
    }
    public TextData()
    {
        Id = "Text_Iron_Name";
        Text_Kr = "Ã¶±¤¼®";
    }
}
public class TextDataTable
{
    public Dictionary<string, TextData> dic;
    [JsonConstructor]
    public TextDataTable(Dictionary<string, TextData> dic)
    {
        this.dic = dic;
    }
    public TextDataTable()
    {
        dic = new Dictionary<string, TextData>();
        dic.Add("Text_Iron_Name", new TextData());
    }
    public static string FilePath()
    {
        return "/Data/Table/Item/TextData.json";
    }
}

public class JsonDataCreator : MonoBehaviour
{
    private void Awake()
    {
        JsonDataManager.jsonCache.Lode();
        JsonDataManager.jsonCache.Save();
    }
}