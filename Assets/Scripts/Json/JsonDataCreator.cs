using Newtonsoft.Json;
using System.Collections.Generic;
using EnumTypes;

public class Item
{
    [JsonProperty] public int Id { get; private set; }
    [JsonProperty] public ItemType ItemType { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Icon { get; private set; }
    [JsonProperty] public int MaxStack { get; private set; }
    [JsonProperty] public float EnergyReserves { get; private set; }

    [JsonConstructor]
    public Item(int id, ItemType itemType, string name, string icon, int maxStack, float energyReserves)
    {
        Id = id;
        ItemType = itemType;
        Name = name;
        Icon = icon;
        MaxStack = maxStack;
        EnergyReserves = energyReserves;
    }
    public Item()
    {
        Id = 0;
        ItemType = ItemType.Material;
        Name = "기본 자원 이름";
        Icon = "기본 아이콘 이름";
        MaxStack = 100;
        EnergyReserves = 0;
    }
}
public class ItemTable
{
    public Dictionary<int, Item> dic;
    [JsonConstructor]
    public ItemTable(Dictionary<int, Item> dic)
    {
        this.dic = dic;
    }
    public ItemTable()
    {
        dic = new Dictionary<int, Item>();
        dic.Add(0, new Item());
    }
    public static string FilePath()
    {
        return "/Data/Table/Item/ItemTable.json";
    }
}