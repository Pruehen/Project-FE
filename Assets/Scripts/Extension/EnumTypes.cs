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
        Start,
        End,
        Mid,
        Left,
        Right,
        Merge
    }

    public enum BuildMode
    { 
        None,
        Belt,
        Inserter,
        Building
    }
}
