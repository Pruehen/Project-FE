using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : SceneSingleton<BuildingManager>, IBuildTool
{
    public GameObject Prefab_inserterPart;

    InserterCrafter inserterCrafter = new InserterCrafter();
    Vector3Int posTemp;
    GridDir buildDir;

    public void OnClick(Vector3Int pos)
    {
        BuildInserter(pos);
    }
    public void OnMove(Vector3Int pos)
    {
        if (posTemp != pos)
        {
            posTemp = pos;
            CheckBuildInserter(pos);
        }
    }
    public void OnKeyDown(KeyCode key)//건설 방향을 바꿈
    {
        if (key == KeyCode.R)
        {
            buildDir++;
            if ((int)buildDir >= 4)
            {
                buildDir = 0;
            }
            CheckBuildInserter(posTemp);
        }
    }
    public void DeActive()
    {
        inserterCrafter.DeActive();
    }

    void CheckBuildInserter(Vector3Int mouseNode)
    {
        inserterCrafter.CheckBuildInserter(mouseNode, buildDir);
    }
    void BuildInserter(Vector3Int lastNode)
    {
        inserterCrafter.BuildInserter(lastNode, buildDir);
    }
}

