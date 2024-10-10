using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

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
}

public class SaveData
{
    [JsonProperty] public Dictionary<Vector3Int, SaveData_Building> dic_building;

    [JsonConstructor]
    public SaveData(Dictionary<Vector3Int, SaveData_Building> dic)
    {
        this.dic_building = dic;
        if (this.dic_building == null)
        {
            this.dic_building = new Dictionary<Vector3Int, SaveData_Building>();
        }
    }
    public SaveData()
    {
        dic_building = new Dictionary<Vector3Int, SaveData_Building>();
    }
    public void TryAddBuildingData(Vector3Int gridPos, Building building)
    {
        if(dic_building.ContainsKey(gridPos))
        {
            Debug.Log("이미 설치된 건물입니다.");
            return;
        }

        dic_building.Add(gridPos, new SaveData_Building(building.BuildingData, building.transform.position.ToIntVector(), building.transform.rotation));
    }
    public static string FilePath()
    {
        return "/Data/Save/TestSaveFile.json";
    }
}

public class JsonSaveDataCreator : MonoBehaviour
{

}
