using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridMap
{
    public static Dictionary<Vector3Int, Node> Dic_OccupiedDepth = new Dictionary<Vector3Int, Node>();//점유 공간 확인 계층
    public static Dictionary<Vector3Int, Building> Dic_BuildingDepth = new Dictionary<Vector3Int, Building>();//빌딩 계층. 빌딩 관리에 사용됨
    public static Dictionary<Vector3Int, Node> Dic_BeltDepth = new Dictionary<Vector3Int, Node>();//벨트 계층. 벨트 로직에 사용됨

    static void Add_Dic_BeltDepth(Vector3Int gridPos, Node node)
    {
        Dic_BeltDepth.Add(gridPos, node);
        Dic_OccupiedDepth.Add(gridPos, node);
    }
    public static void Remove_Dic_BeltDepth(Vector3Int gridPos)
    {
        Dic_BeltDepth.Remove(gridPos);
        Dic_OccupiedDepth.Remove(gridPos);
    }
    static void Add_Dic_BuildingDepth(Vector3Int gridPos, Building building, Node node)
    {
        Dic_BuildingDepth.Add(gridPos, building);
        foreach (Transform item in building.occupiedNodeList)
        {
            Dic_OccupiedDepth.Add(item.position.ToIntVector(), node);
        }

    }
    public static void Remove_Dic_BuildingDepth(Vector3Int gridPos)
    {
        foreach (Transform item in Dic_BuildingDepth[gridPos].occupiedNodeList)
        {
            Dic_OccupiedDepth.Remove(item.position.ToIntVector());
        }
        Dic_BuildingDepth.Remove(gridPos);
    }


    public static BeltNode CreateBeltNode(Vector3Int gridPos)//벨트 건설
    {
        BeltNode node = new BeltNode(gridPos);

        Add_Dic_BeltDepth(gridPos, node);
        return node;
    }
    public static SorterNode CreateSorterNode(Vector3Int gridPos)//소터 건설
    {
        Node nodeTemp = Dic_BeltDepth[gridPos];//소터가 위치할 포지션
        Node previousNode = nodeTemp.PreviousNode;
        Node nextNode = nodeTemp.NextNode;

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
    public static BuildingNode CreateBuildingNode(GameObject prefab, Vector3Int gridPos, Quaternion dir)//빌딩 건설
    {
        Building building = ObjectPoolManager.Instance.DequeueObject(prefab, gridPos).GetComponent<Building>();
        building.transform.rotation = dir;

        foreach (Transform item in building.occupiedNodeList)
        {
            if (Dic_OccupiedDepth.ContainsKey(item.position.ToIntVector()))
            {
                Debug.Log("이미 사용 중인 공간입니다.");
                ObjectPoolManager.Instance.EnqueueObject(building.gameObject);
                return null;
            }
        }

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
            item.Value.transporter.LogicInit();
        }
    }
    public static Vector3Int ToIntVector(this Vector3 vector)
    {
        int x = Mathf.RoundToInt(vector.x); // x 값을 반올림하여 int로 변환
        int z = Mathf.RoundToInt(vector.z); // z 값을 반올림하여 int로 변환
        int y = 0;// Mathf.RoundToInt(vector.y);        

        return new Vector3Int(x, y, z); // 새로운 Vector3 반환
    }

    public static Vector3Int Up(this Vector3Int vector3Int)
    {
        vector3Int.z++;
        return vector3Int;
    }
    public static Vector3Int Down(this Vector3Int vector3Int)
    {
        vector3Int.z--;
        return vector3Int;
    }
    public static Vector3Int Left(this Vector3Int vector3Int)
    {
        vector3Int.x--;
        return vector3Int;
    }
    public static Vector3Int Right(this Vector3Int vector3Int)
    {
        vector3Int.x++;
        return vector3Int;
    }
}
