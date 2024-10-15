using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : SceneSingleton<GameLogicManager>
{
    public HashSet<INode> InserterNodeSet = new HashSet<INode>();
    public HashSet<INode> SorterNodeSet = new HashSet<INode>();
    public HashSet<INode> RootBeltNodeSet = new HashSet<INode>();

    // Update is called once per frame
    void Update()
    {
        GridMap.Command_LogicInit_OnUpdate();

        foreach (var node in InserterNodeSet)
        {
            node.Transporter.ExcuteLogic_OnUpdate(Time.deltaTime);
        }
        foreach (var node in SorterNodeSet)
        {
            node.Transporter.ExcuteLogic_OnUpdate(Time.deltaTime);
        }
        foreach (var node in RootBeltNodeSet)
        {
            node.Transporter.ExcuteLogic_OnUpdate(Time.deltaTime);
        }
    }
}
