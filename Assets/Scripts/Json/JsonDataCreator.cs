using Newtonsoft.Json;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class ItemData
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public ushort Id_UShort { get; private set; }
    [JsonProperty] public ItemType ItemType { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Desc { get; private set; }
    [JsonProperty] public int MaxStack { get; private set; }
    [JsonProperty] public float EnergyReserves { get; private set; }

    [JsonProperty("Icon")] public string Icon_Path { get; private set; }
    [JsonProperty("ItemMesh")] public string ItemMesh_Path { get; private set; }
    [JsonProperty("DropMesh")] public string DropMesh_Path { get; private set; }

    [JsonConstructor]
    public ItemData(string id, ushort id_short, ItemType itemType, string name, string desc, int maxStack, float energyReserves, string iconPath, string itemMeshPath, string dropMeshPath )
    {
        Id = id;
        Id_UShort = id_short;
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
    public GameObject GetItemPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{Id}");
        return (prefab != null) ? prefab : Resources.Load<GameObject>($"Prefabs/Item_Default");        
    }
}
public class ItemDataTable
{
    [JsonProperty] public Dictionary<string, ItemData> dic;
    [JsonIgnore] public Dictionary<ushort, ItemData> dic_Short;
    [JsonConstructor]
    public ItemDataTable(Dictionary<string, ItemData> dic)
    {
        this.dic = dic;

        dic_Short = new Dictionary<ushort, ItemData>();
        foreach (var item in dic)
        {
            dic_Short.Add(item.Value.Id_UShort, item.Value);
        }
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
    [JsonProperty] public string RecipyGroup { get; private set; }
    [JsonProperty] public float DeploySizeX { get; private set; }
    [JsonProperty] public float DeploySizeY { get; private set; }
    [JsonProperty] public float DeploySizeZ { get; private set; }
    [JsonProperty] public bool UseEnergy { get; private set; }
    [JsonProperty] public float EnergyEfficiency { get; private set; }
    [JsonProperty] public float SpeedEfficiency { get; private set; }

    [JsonConstructor]
    public BuildingData(string id, BuildingType buildingType, string recipyGroupId, float deploySizeX, float deploySizeY, float deploySizeZ, bool isUseEnergy, float energyEfficiency, float speedEfficiency)
    {
        Id = id;
        BuildingType = buildingType;
        RecipyGroup = recipyGroupId;
        DeploySizeX = deploySizeX;
        DeploySizeY = deploySizeY;
        DeploySizeZ = deploySizeZ;
        UseEnergy = isUseEnergy;
        EnergyEfficiency = energyEfficiency;
        SpeedEfficiency = speedEfficiency;
    }

    public BuildingData()
    {
        Id = "Building_Crafter_T1";
        BuildingType = BuildingType.Crafting;
        RecipyGroup = "RG_Crafter_T1";
        DeploySizeX = 2;
        DeploySizeY = 1.5f;
        DeploySizeZ = 2;
        UseEnergy = true;
        EnergyEfficiency = 1;
        SpeedEfficiency = 1;
    }
    public GameObject GetBuildingPrefab()
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{Id}");

        if(prefab == null)
        {
            Debug.LogError($"{Id} ¿¡ ÇØ´çÇÏ´Â ÇÁ¸®ÆÕÀÌ ¾ø½À´Ï´Ù.");            
        }

        return prefab;
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
        return "/Data/Table/Item/BuildingData.json";
    }
}
public class RecipyGroupData
{
    [JsonProperty] public string GroupId { get; private set; }
    [JsonProperty] public string Recipy { get; private set; }

    [JsonConstructor]
    public RecipyGroupData(string groupId, string recipy)
    {
        GroupId = groupId;
        Recipy = recipy;
    }
    public RecipyGroupData()
    {
        GroupId = "null";
        Recipy = "null";
    }
}
public class RecipyGroupDataTable
{
    [JsonProperty] List<RecipyGroupData> list;
    [JsonIgnore] public Dictionary<string, List<string>> dic { get; private set; }
    [JsonConstructor]
    public RecipyGroupDataTable(List<RecipyGroupData> list)
    {
        this.list = list;
        Init();
    }
    public RecipyGroupDataTable()
    {
        list = new List<RecipyGroupData>();    
        list.Add(new RecipyGroupData());
        list.Add(new RecipyGroupData());
        Init();
    }
    void Init()
    {
        dic = new Dictionary<string, List<string>>();
        foreach (var item in list)
        {
            if(dic.ContainsKey(item.GroupId))
            {
                dic[item.GroupId].Add(item.Recipy);
            }
            else
            {
                dic.Add(item.GroupId, new List<string>());
                dic[item.GroupId].Add(item.Recipy);
            }
        }
    }
    public static string FilePath()
    {
        return "/Data/Table/Item/RecipyGroupData.json";
    }
}


public class RecipyData
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Desc { get; private set; }
    [JsonProperty("Icon")] public string Icon_Path { get; private set; }
    //========================================================================
    [JsonProperty("InputItem-1")] public string InputItem_1 { get; private set; }
    [JsonProperty("InputItemCount-1")] public int InputItemCount_1 { get; private set; }
    [JsonProperty("InputItem-2")] public string InputItem_2 { get; private set; }
    [JsonProperty("InputItemCount-2")] public int InputItemCount_2 { get; private set; }
    [JsonProperty("InputItem-3")] public string InputItem_3 { get; private set; }
    [JsonProperty("InputItemCount-3")] public int InputItemCount_3 { get; private set; }
    [JsonProperty("InputItem-4")] public string InputItem_4 { get; private set; }
    [JsonProperty("InputItemCount-4")] public int InputItemCount_4 { get; private set; }
    //========================================================================
    [JsonProperty("OutputItem-1")] public string OutputItem_1 { get; private set; }
    [JsonProperty("OutputItemCount-1")] public int OutputItemCount_1 { get; private set; }
    [JsonProperty("OutputItem-2")] public string OutputItem_2 { get; private set; }
    [JsonProperty("OutputItemCount-2")] public int OutputItemCount_2 { get; private set; }
    [JsonProperty("OutputItem-3")] public string OutputItem_3 { get; private set; }
    [JsonProperty("OutputItemCount-3")] public int OutputItemCount_3 { get; private set; }
    [JsonProperty("OutputItem-4")] public string OutputItem_4 { get; private set; }
    [JsonProperty("OutputItemCount-4")] public int OutputItemCount_4 { get; private set; }
    //========================================================================

    [JsonProperty] public float CraftingTime { get; private set; }

    [JsonIgnore] public List<ItemGroup_UsedInRecipe> InputItemGroup { get; private set; }
    [JsonIgnore] public List<ItemGroup_UsedInRecipe> OutputItemGroup { get; private set; }

    [JsonConstructor]
    public RecipyData(
        string id,
        string name,
        string desc,
        string icon_Path,
        string inputItem_1, int inputItemCount_1,
        string inputItem_2, int inputItemCount_2,
        string inputItem_3, int inputItemCount_3,
        string inputItem_4, int inputItemCount_4,
        string outputItem_1, int outputItemCount_1,
        string outputItem_2, int outputItemCount_2,
        string outputItem_3, int outputItemCount_3,
        string outputItem_4, int outputItemCount_4,
        float craftingTime
    )
    {
        Id = id;
        Name = name;
        Desc = desc;
        Icon_Path = icon_Path;

        InputItemGroup = new List<ItemGroup_UsedInRecipe>();
        OutputItemGroup = new List<ItemGroup_UsedInRecipe>();

        //================================================================
        InputItem_1 = inputItem_1;
        InputItemCount_1 = inputItemCount_1;
        if(InputItem_1 != null)
        {
            InputItemGroup.Add(new ItemGroup_UsedInRecipe(InputItem_1, InputItemCount_1));
        }
        InputItem_2 = inputItem_2;
        InputItemCount_2 = inputItemCount_2;
        if (InputItem_2 != null)
        {
            InputItemGroup.Add(new ItemGroup_UsedInRecipe(InputItem_2, InputItemCount_2));
        }
        InputItem_3 = inputItem_3;
        InputItemCount_3 = inputItemCount_3;
        if (InputItem_3 != null)
        {
            InputItemGroup.Add(new ItemGroup_UsedInRecipe(InputItem_3, InputItemCount_3));
        }
        InputItem_4 = inputItem_4;
        InputItemCount_4 = inputItemCount_4;
        if (InputItem_4 != null)
        {
            InputItemGroup.Add(new ItemGroup_UsedInRecipe(InputItem_4, InputItemCount_4));
        }
        //================================================================
        OutputItem_1 = outputItem_1;
        OutputItemCount_1 = outputItemCount_1;
        if (OutputItem_1 != null)
        {
            OutputItemGroup.Add(new ItemGroup_UsedInRecipe(OutputItem_1, OutputItemCount_1));
        }
        OutputItem_2 = outputItem_2;
        OutputItemCount_2 = outputItemCount_2;
        if (OutputItem_2 != null)
        {
            OutputItemGroup.Add(new ItemGroup_UsedInRecipe(OutputItem_2, OutputItemCount_2));
        }
        OutputItem_3 = outputItem_3;
        OutputItemCount_3 = outputItemCount_3;
        if (OutputItem_3 != null)
        {
            OutputItemGroup.Add(new ItemGroup_UsedInRecipe(OutputItem_3, OutputItemCount_3));
        }
        OutputItem_4 = outputItem_4;
        OutputItemCount_4 = outputItemCount_4;
        if (OutputItem_4 != null)
        {
            OutputItemGroup.Add(new ItemGroup_UsedInRecipe(OutputItem_4, OutputItemCount_4));
        }
        //================================================================

        CraftingTime = craftingTime;
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
    public ItemData data;
    public int Count;

    public ItemGroup_UsedInRecipe(string id, int count)
    {
        data = JsonDataManager.GetItem(id);
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
        return "/Data/Table/Item/RecipyData.json";
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