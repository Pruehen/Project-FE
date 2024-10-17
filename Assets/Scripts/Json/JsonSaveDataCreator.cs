using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


public class SaveData_Charactor
{
    [JsonProperty] public int[] positionData;
    [JsonProperty] public Dictionary<ushort, int> dic_ItemId_Count = new Dictionary<ushort, int>();//캐릭터의 인벤토리 정보

    [JsonConstructor]
    public SaveData_Charactor(int[] positionData)
    {        
        this.positionData = positionData;        
    }
    public SaveData_Charactor(Vector3Int gridPos)
    {
        SaveData(gridPos);
    }

    public void SaveData(Vector3Int gridPos)
    {
        positionData = new int[] { gridPos.x, gridPos.y, gridPos.z };        
    }
    public void SaveData(Inventory inventory)
    {
        dic_ItemId_Count.Clear();

        foreach (var cellData in inventory.CellDataList)
        {
            if(dic_ItemId_Count.ContainsKey(cellData.Id) == false)
            {
                dic_ItemId_Count.Add(cellData.Id, cellData.Count);
            }
            else
            {
                dic_ItemId_Count[cellData.Id] += cellData.Count;
            }
        }
    }

    public void LodeData_Charactor()
    {
        CharactorManager.Instance.GenerateCharactor(this);
    }
}

public class SaveData_Building
{
    [JsonProperty] string buildingDataId;
    [JsonProperty] int[] positionData;
    [JsonProperty] float[] rotationData;    

    [JsonConstructor]
    public SaveData_Building(string buildingDataId, int[] positionData, float[] rotationData)
    {
        this.buildingDataId = buildingDataId;
        this.positionData = positionData;
        this.rotationData = rotationData;        
    }
    public SaveData_Building(BuildingData buildingData, Vector3Int gridPos, Quaternion rotateion)
    {
        this.buildingDataId = buildingData.Id;
        positionData = new int[] { gridPos.x, gridPos.y, gridPos.z };

        Vector3 eulerAngle = rotateion.eulerAngles;
        rotationData = new float[] { eulerAngle.x, eulerAngle.y, eulerAngle.z };       
    }

    public void LodeData_Building()
    {
        BuildingNode createNode = GridMap.CreateBuildingNode_LodeData(JsonDataManager.GetBuilding(buildingDataId), 
            new Vector3(positionData[0], positionData[1], positionData[2]).ToVector3Int(), 
            Quaternion.Euler(rotationData[0], rotationData[1], rotationData[2]));

        if (createNode != null)//빌딩 노드 생성에 성공했을 경우
        {
            createNode.Init();
        }
    }
}
public struct SaveData_Vein
{
    [JsonProperty] public string itemKey;
    [JsonProperty] public int count;

    [JsonConstructor]
    public SaveData_Vein(string itemKey, int count)
    {        
        this.itemKey = itemKey;
        this.count = count;
    }
}

public class SaveData
{
    [JsonProperty] public List<SaveData_Charactor> list_Charactor;
    [JsonProperty] public Dictionary<string, SaveData_Building> dic_Building;
    [JsonProperty] public Dictionary<string, SaveData_Vein> dic_Vein;

    [JsonConstructor]
    public SaveData(List<SaveData_Charactor> list_Charactor, Dictionary<string, SaveData_Building> list_building, Dictionary<string, SaveData_Vein> dic_Vein)
    {
        this.list_Charactor = list_Charactor;
        if (this.list_Charactor == null)
        {
            this.list_Charactor = new List<SaveData_Charactor>();
        }

        this.dic_Building = list_building;
        if (this.dic_Building == null)
        {
            this.dic_Building = new Dictionary<string, SaveData_Building>();
        }

        this.dic_Vein = dic_Vein;
        if(this.dic_Vein == null)
        {
            this.dic_Vein = new Dictionary<string, SaveData_Vein>();
        }
    }
    public SaveData()
    {
        this.list_Charactor = new List<SaveData_Charactor>();
        this.dic_Building = new Dictionary<string, SaveData_Building>();
        this.dic_Vein = new Dictionary<string, SaveData_Vein>();
    }
    public void TryAddBuildingData(Vector3Int gridPos, Building building)
    {
        dic_Building.Add(gridPos.ToString(), new SaveData_Building(building.BuildingData, gridPos, building.transform.rotation));
    }
    public void TryRemoveBuildingData(Vector3Int gridPos)
    {
        dic_Building.Remove(gridPos.ToString());
    }
    public SaveData_Charactor AddCharactor()
    {
        list_Charactor.Add(new SaveData_Charactor(new Vector3Int(0, 1, 0)));
        return list_Charactor[list_Charactor.Count - 1];
    }
    public void RemoveCharactor(int index)
    {
        
    }

    public void AllDataSave()
    {
        DataSave_Charactor();
        DataSave_Vein();
    }
    void DataSave_Charactor()
    {
        for (int i = 0; i < list_Charactor.Count; i++)
        {
            Charactor charactor = CharactorManager.Instance.GetCharactor(i);
            list_Charactor[i].SaveData(charactor.transform.position.ToVector3Int());
            list_Charactor[i].SaveData(charactor.BuiltIn_InventoryModule.Inventory);
        }
    }
    void DataSave_Vein()
    {
        dic_Vein.Clear();
        foreach (var item in GridMap.Dic_VeinDepth)
        {
            dic_Vein.Add(item.Key.ToString(), new SaveData_Vein(item.Value.itemKey, item.Value.reserves));
        }
    }

    public void AllDataLode()
    {
        DataLode_Charactor();
        DataLode_Vein();
        DataLode_Building();
    }
    void DataLode_Charactor()
    {
        if (list_Charactor.Count == 0)
        {
            AddCharactor().LodeData_Charactor();
        }
        else
        {
            foreach (var charactor in list_Charactor)
            {
                charactor.LodeData_Charactor();
            }
        }
        Player.Instance.Init();
    }
    void DataLode_Vein()
    {

    }
    void DataLode_Building()
    {
        foreach (var building in dic_Building)
        {
            building.Value.LodeData_Building();
        }
    }
    public static string FilePath()
    {
        return "/Data/Save/TestSaveFile.json";
    }
}