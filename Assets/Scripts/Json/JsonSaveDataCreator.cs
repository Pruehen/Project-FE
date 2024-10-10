using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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

    public void LodeData_Building()
    {
        BuildingNode createNode = GridMap.CreateBuildingNode_LodeData(JsonDataManager.GetBuilding(buildingDataId), 
            new Vector3(positionData[0], positionData[1], positionData[2]).ToIntVector(), 
            Quaternion.Euler(rotationData[0], rotationData[1], rotationData[2]));

        if (createNode != null)//빌딩 노드 생성에 성공했을 경우
        {
            createNode.Init();
        }
    }
}

public class SaveData
{
    [JsonProperty] public List<SaveData_Building> list_building;

    [JsonConstructor]
    public SaveData(List<SaveData_Building> list)
    {
        this.list_building = list;
        if (this.list_building == null)
        {
            this.list_building = new List<SaveData_Building>();
        }
    }
    public SaveData()
    {
        list_building = new List<SaveData_Building>();
    }
    public void TryAddBuildingData(Vector3Int gridPos, Building building)
    {
        //if(list_building.Contains(building))
        //{
        //    Debug.Log("이미 설치된 건물입니다.");
        //    return;
        //}
        list_building.Add(new SaveData_Building(building.BuildingData, gridPos, building.transform.rotation));
    }
    public static string FilePath()
    {
        return "/Data/Save/TestSaveFile.json";
    }
}

public class JsonSaveDataCreator : MonoBehaviour
{

}
