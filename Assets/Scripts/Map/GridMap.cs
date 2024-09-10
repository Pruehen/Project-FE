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
        Node nodeTemp = NodeDic_NormalDepth[gridPos];
        Node previousNode = nodeTemp.PreviousNode;
        Node nextNode = nodeTemp.NextNode;

        //벨트와 소터 교체
        nodeTemp.Remove();
        NodeDic_NormalDepth.Remove(gridPos);

        SorterNode sorterNode = new SorterNode(gridPos);
        NodeDic_NormalDepth.Add(gridPos, sorterNode);
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
    public static InserterNode CreateInserter(Vector3Int firstPos, Vector3Int lastPos)
    {
        InserterNode node = new InserterNode(firstPos, lastPos);

        NodeDic_InteractableDepth.Add(firstPos, node);
        NodeDic_InteractableDepth.Add(lastPos, node);
        return node;
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
