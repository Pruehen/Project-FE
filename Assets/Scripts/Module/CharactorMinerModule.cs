using UnityEngine;

public class CharactorMinerModule : MonoBehaviour
{
    Charactor _charactor;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
    }

    [SerializeField] float MiningSpeedGain = 5;

    Vein currentVein;
    IInteractable currentInteractable;

    float miningTimeValue = 0;

    public void Mining(float deltaTime, IInteractable interactable)
    {
        if (interactable.GetEntityType() != EnumTypes.EntityType.Vein) return;       
        
        if(currentInteractable != interactable)
        {
            currentInteractable = interactable;
            currentVein = interactable as Vein;
            miningTimeValue = 0;
        }

        miningTimeValue += deltaTime * MiningSpeedGain;

        if(miningTimeValue >= currentVein.extractTimeGain)
        {
            miningTimeValue = 0;
            currentVein.ExtractVein(1, out int extractCount);

            for (int i = 0; i < extractCount; i++)
            {
                ItemObject itemObject = ItemObjectManager.CreateObject(currentVein.GetItemKey());
                itemObject.ItemDrop(currentVein.transform.position);
                itemObject.PoolItem(_charactor);
            }
        }
    }
}