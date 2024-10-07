using EnumTypes;
using UnityEngine;

public class CharactorMinerModule : MonoBehaviour
{
    Charactor _charactor;
    public void Init(Charactor charactor)
    {
        _charactor = charactor;
    }

    [SerializeField] float MiningSpeedGain = 5;

    Building currentBuilding;
    Vein currentVein;
    IInteractable currentInteractable;

    float miningTimeValue = 0;

    public void Mining_OnTryInteract(float deltaTime, IInteractable interactable)
    {
        if (interactable.GetEntityType() == EntityType.Vein)
        {
            VeinMining(deltaTime, interactable);
        }       
        else if(interactable.GetEntityType() == EntityType.Building)
        {
            DismantleBuilding(deltaTime, interactable);
        }
    }

    void VeinMining(float deltaTime, IInteractable interactable)
    {
        if (currentInteractable != interactable)
        {
            currentInteractable = interactable;
            currentVein = interactable as Vein;
            miningTimeValue = 0;
        }

        miningTimeValue += deltaTime * MiningSpeedGain;

        if (miningTimeValue >= currentVein.extractTimeGain)
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

    void DismantleBuilding(float deltaTime, IInteractable interactable)
    {
        if (currentInteractable != interactable)
        {
            currentInteractable = interactable;
            currentBuilding = interactable as Building;
            miningTimeValue = 0;
        }

        miningTimeValue += deltaTime * MiningSpeedGain;

        if (miningTimeValue >= 1)
        {
            miningTimeValue = 0;
            currentBuilding.Dismantle();     
        }
    }
}