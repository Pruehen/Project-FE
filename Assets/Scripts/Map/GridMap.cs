using System.Collections.Generic;
using UnityEngine;

public static class GridMap
{
    public static Dictionary<Vector3Int, Node> NodeDic_NormalDepth = new Dictionary<Vector3Int, Node>();//벨트, 구조물 등의 계층
    public static Dictionary<Vector3Int, Node> NodeDic_InteractableDepth = new Dictionary<Vector3Int, Node>();//투입기 등의 계층

    public static BeltNode CreateBelt(Vector3Int gridPos)
    {
        BeltNode node = new BeltNode(gridPos);

        NodeDic_NormalDepth.Add(gridPos, node);
        return node;
    }
    public static SorterNode CreateSorter(Vector3Int gridPos)
    {
        SorterNode sorterNode = new SorterNode(gridPos);
        sorterNode.PreviousNode = NodeDic_NormalDepth[gridPos].PreviousNode;
        sorterNode.NextNode = NodeDic_NormalDepth[gridPos].NextNode;

        NodeDic_NormalDepth.Remove(gridPos);
        NodeDic_NormalDepth.Add(gridPos, sorterNode);

        return sorterNode;
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
