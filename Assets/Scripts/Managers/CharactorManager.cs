using System.Collections.Generic;
using UnityEngine;

public class CharactorManager : SceneSingleton<CharactorManager>
{
    [SerializeField] GameObject Prefab_Charactor;
    List<Charactor> charactorList = new List<Charactor>();

    public void GenerateCharactor_OnNewGame()
    {
        if(charactorList.Count == 0)
        {
            Charactor newCharactor = Instantiate(Prefab_Charactor, new Vector3(0, 1, 0), Quaternion.identity).GetComponent<Charactor>();
            charactorList.Add(newCharactor);
        }
    }
    public Charactor GetCharactor(int index)
    {
        if (charactorList.Count >= index)
        {
            GenerateCharactor_OnNewGame();
            return (charactorList.Count >= index) ? charactorList[index] : null;
        }
        else
        {
            return charactorList[index];
        }        
    }
}
