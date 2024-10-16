using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


public class SaveData_Charactor
{
    [JsonProperty] int[] positionData;
    [JsonProperty] float[] rotationData;

    [JsonConstructor]
    public SaveData_Charactor(int[] positionData, float[] rotationData)
    {        
        this.positionData = positionData;
        this.rotationData = rotationData;
    }
    public SaveData_Charactor(Vector3Int gridPos, Quaternion rotateion)
    {
        SaveData(gridPos, rotateion);
    }

    public void SaveData(Vector3Int gridPos, Quaternion rotateion)
    {
        positionData = new int[] { gridPos.x, gridPos.y, gridPos.z };

        Vector3 eulerAngle = rotateion.eulerAngles;
        rotationData = new float[] { eulerAngle.x, eulerAngle.y, eulerAngle.z };
    }
    public void LodeData_Charactor()
    {
        CharactorManager.Instance.GenerateCharactor(new Vector3(positionData[0], positionData[1], positionData[2]));
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

public class SaveData
{
    [JsonProperty] public List<SaveData_Charactor> list_Charactor;
    [JsonProperty] public Dictionary<string, SaveData_Building> dic_Building;

    [JsonConstructor]
    public SaveData(List<SaveData_Charactor> list_Charactor, Dictionary<string, SaveData_Building> list_building)
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
    }
    public SaveData()
    {
        this.list_Charactor = new List<SaveData_Charactor>();
        this.dic_Building = new Dictionary<string, SaveData_Building>();
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
        list_Charactor.Add(new SaveData_Charactor(new Vector3Int(0, 1, 0), Quaternion.identity));
        return list_Charactor[list_Charactor.Count - 1];
    }
    public void RemoveCharactor(int index)
    {        

    }

    public void DataSave()
    {
        for (int i = 0; i < list_Charactor.Count; i++)
        {
            Charactor charactor = CharactorManager.Instance.GetCharactor(i);
            list_Charactor[i].SaveData(charactor.transform.position.ToVector3Int(), charactor.transform.rotation);
        }
    }
    public void AllDataLode()
    {
        if(list_Charactor.Count == 0)
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

public class JsonSaveDataCreator : MonoBehaviour
{

}
