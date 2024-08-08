using Newtonsoft.Json;
using System.Collections.Generic;
using EnumTypes;

public class Item
{
    int _id;
    ItemType _itemType;
    string _name;
    string _icon;
    int _maxStack;

    [JsonConstructor]
    public Item(int id, ItemType itemType, string name, string icon, int maxStack)
    {
        _id = id;
        _itemType = itemType;
        _name = name;
        _icon = icon;
        _maxStack = maxStack;
    }
    public Item()
    {
        _id = -1;
        _itemType = ItemType.Material;
        _name = "�⺻ �ڿ� �̸�";
        _icon = "�⺻ ������ �̸�";
        _maxStack = 100;
    }
}
public class ItemTable
{
    public List<Item> list = new List<Item>();
    [JsonConstructor]
    public ItemTable(List<Item> list)
    {
        this.list = list;
    }
    public ItemTable()
    {
        list = new List<Item>();
    }
    public static string FilePath()
    {
        return "/Data/Table/Item/ItemTable.json";
    }
}