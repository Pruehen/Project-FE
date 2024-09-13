using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : SceneSingleton<GameLogicManager>
{
    public HashSet<Node> InserterNodeSet = new HashSet<Node>();    
    public HashSet<Node> RootBeltNodeSet = new HashSet<Node>();

    // Update is called once per frame
    void Update()
    {
        GridMap.Command_LogicInit_OnUpdate();

        foreach (var node in InserterNodeSet)
        {
            node.transporter.ExcuteLogic_OnUpdate(Time.deltaTime);
        }
        foreach (var node in RootBeltNodeSet)
        {
            node.transporter.ExcuteLogic_OnUpdate(Time.deltaTime);
        }
    }
}
