using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharactorManager : SceneSingleton<CharactorManager>
{
    [SerializeField] GameObject Prefab_Charactor;
    List<Charactor> charactorList = new List<Charactor>();

    public Charactor GenerateCharactor(SaveData_Charactor saveData_Charactor)
    {
        int[] posData = saveData_Charactor.positionData;
        Vector3 pos = new Vector3(posData[0], posData[1], posData[2]);

        Charactor newCharactor = Instantiate(Prefab_Charactor, pos, Quaternion.identity).GetComponent<Charactor>();
        charactorList.Add(newCharactor);

        newCharactor.Init(saveData_Charactor);
        return newCharactor;
    }

    public Charactor GetCharactor(int index)
    {
        if (charactorList.Count <= index)
        {
            return null;
        }
        else
        {
            return charactorList[index];
        }        
    }
}
