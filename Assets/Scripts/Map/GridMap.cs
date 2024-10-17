using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public static Dictionary<Vector3Int, INode> Dic_OccupiedDepth = new Dictionary<Vector3Int, INode>();//점유 공간 확인 계층
    public static Dictionary<Vector3Int, Building> Dic_BuildingDepth = new Dictionary<Vector3Int, Building>();//빌딩 계층. 빌딩 관리에 사용됨
    public static Dictionary<Vector3Int, INode> Dic_BeltDepth = new Dictionary<Vector3Int, INode>();//벨트 계층. 벨트 로직에 사용됨
    public static Dictionary<Vector3Int, Vein> Dic_VeinDepth = new Dictionary<Vector3Int, Vein>();//광맥 계층. 채굴기의 광맥 체크 로직에 사용됨     


    static void Add_Dic_BeltDepth(Vector3Int gridPos, INode node)
    {
        Dic_BeltDepth.Add(gridPos, node);
        Dic_OccupiedDepth.Add(gridPos, node);
    }
    public static void Remove_Dic_BeltDepth(Vector3Int gridPos)
    {
        Dic_BeltDepth.Remove(gridPos);
        Dic_OccupiedDepth.Remove(gridPos);
    }
    static void Add_Dic_BuildingDepth(Vector3Int gridPos, Building building, INode node)
    {
        Dic_BuildingDepth.Add(gridPos, building);

        foreach (Transform item in building.occupiedNodeList)
        {
            Dic_OccupiedDepth.Add(item.position.ToVector3Int(), node);
        }
    }
    public static void Remove_Dic_BuildingDepth(Vector3Int gridPos)
    {
        foreach (Transform item in Dic_BuildingDepth[gridPos].occupiedNodeList)
        {
            Dic_OccupiedDepth.Remove(item.position.ToVector3Int());
        }
        Dic_BuildingDepth.Remove(gridPos);
        JsonDataManager.SaveData_TryRemoveBuildingData(gridPos);
    }
    public static void Add_Dic_VeinDepth(Vector3Int gridPos, Vein vein)
    {
        Dic_VeinDepth.Add(gridPos, vein);
    }
    public static void Remove_Dic_VeinDepth(Vector3Int gridPos)
    {
        Dic_VeinDepth[gridPos].gameObject.SetActive(false);
        Dic_VeinDepth.Remove(gridPos);        
    }

    public static BeltNode CreateBeltNode(Vector3Int gridPos)//벨트 건설
    {
        BeltNode node = new BeltNode(gridPos);

        Add_Dic_BeltDepth(gridPos, node);
        return node;
    }
    public static SorterNode CreateSorterNode(Vector3Int gridPos)//소터 건설
    {
        INode nodeTemp = Dic_BeltDepth[gridPos];//소터가 위치할 포지션
        INode previousNode = nodeTemp.PreviousNode;
        INode nextNode = nodeTemp.NextNode;

        //벨트와 소터 교체
        nodeTemp.Remove();
        Remove_Dic_BeltDepth(gridPos);

        SorterNode sorterNode = new SorterNode(gridPos);
        Add_Dic_BeltDepth(gridPos, sorterNode);
        sorterNode.Init();

        //소터와 기존 벨트간의 연결
        sorterNode.PreviousNode = previousNode;
        sorterNode.NextNode = nextNode;

        if (previousNode != null)
        {
            previousNode.NextNode = sorterNode;
        }
        if(nextNode != null)
        {
            nextNode.PreviousNode = sorterNode;
        }            

        return sorterNode;
    }
    public static BuildingNode CreateBuildingNode(BuildingData buildingData, Vector3Int gridPos, Quaternion dir)//빌딩 건설. 일반적으로 플레이어가 빌딩을 설치 시도한 상황에서 호출함.
    {
        Building building = ObjectPoolManager.Instance.DequeueObject(buildingData.GetBuildingPrefab(), gridPos).GetComponent<Building>();
        building.transform.rotation = dir;

        foreach (Transform item in building.occupiedNodeList)
        {
            if (Dic_OccupiedDepth.ContainsKey(item.position.ToVector3Int()))
            {
                Debug.Log("이미 사용 중인 공간입니다.");
                ObjectPoolManager.Instance.EnqueueObject(building.gameObject);
                return null;
            }
        }

        BuildingNode node = new BuildingNode(building, gridPos);
        Add_Dic_BuildingDepth(gridPos, building, node);

        JsonDataManager.SaveData_TryAddBuildingData(gridPos, building);

        return node;
    }
    public static BuildingNode CreateBuildingNode_LodeData(BuildingData buildingData, Vector3Int gridPos, Quaternion dir)
        //빌딩 건설. 로드 시에만 호출할 것. 점유 공간 검사 기능과 세이브데이터에 건물 추가 기능이 삭제된 메서드임.
    {
        Building building = ObjectPoolManager.Instance.DequeueObject(buildingData.GetBuildingPrefab(), gridPos).GetComponent<Building>();
        building.transform.rotation = dir;

        BuildingNode node = new BuildingNode(building, gridPos);
        Add_Dic_BuildingDepth(gridPos, building, node);

        return node;
    }
    //public static InserterNode CreateInserter(Vector3Int firstPos, Vector3Int lastPos)//인서터 건설
    //{
    //    InserterNode node = new InserterNode(firstPos, lastPos);

    //    NodeDic_InteractableDepth.Add(firstPos, node);
    //    NodeDic_InteractableDepth.Add(lastPos, node);
    //    return node;
    //}
    public static void Command_LogicInit_OnUpdate()
    {
        foreach (var item in Dic_BeltDepth)
        {
            item.Value.Transporter.LogicInit();
        }
    }
}