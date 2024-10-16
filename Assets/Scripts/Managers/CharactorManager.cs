using System.Collections.Generic;
using UnityEngine;

public class CharactorManager : SceneSingleton<CharactorManager>
{
    [SerializeField] GameObject Prefab_Charactor;
    List<Charactor> charactorList = new List<Charactor>();

    public void GenerateCharactor(Vector3 pos)
    {
        Charactor newCharactor = Instantiate(Prefab_Charactor, pos, Quaternion.identity).GetComponent<Charactor>();
        charactorList.Add(newCharactor);
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
