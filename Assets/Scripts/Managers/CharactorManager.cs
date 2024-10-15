using System.Collections.Generic;

public class CharactorManager : SceneSingleton<CharactorManager>
{
    List<Charactor> charactorList = new List<Charactor>();

    public void GenerateCharactor_OnNewGame()
    {

    }
    public Charactor GetCharactor(int index)
    {
        if (charactorList.Count >= index)
        {
            return null;
        }
        else
        {
            return charactorList[index];
        }        
    }
}
