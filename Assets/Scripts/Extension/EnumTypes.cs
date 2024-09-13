using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace EnumTypes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemType
    {
        Resource,       
        Parts,
        Building
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BuildingType
    {
        Crafting,
        Refinery,
        Mining,
        Conveying,
        Inserter,
        Generator,
        Storage
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Language
    {
        Kr
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BeltType
    {
        Mid,
        Left,
        Right,
        Sorter
    }
    public enum NodeType
    {
        BeltNode,
        SorterNode,
        BuildingNode,
        InserterNode
    }

    public enum BuildMode
    { 
        None,
        Belt,
        Inserter,
        Building
    }
    public enum GridDir
    {
        Top,
        Right,
        Bottom,
        Left
    }
    public enum InventoryType
    {
        Input,
        CharactorStorage,
        Storage,
        Output
    }
    public enum EntityType
    {
        Item,
        Charactor,
        Vein,
        Building
    }
}
