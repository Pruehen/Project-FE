using UnityEngine;

namespace ViewModel.Extensions
{
    public static class Wdw_CraftingModuleViewModelExtension
    {
        //public static void Register_OnMouseObjectNameChanged(this Wdw_InventoryViewModel vm)
        //{
        //    Player.Instance.OnDataChanged += vm.OnDataChanged;
        //    Player.Instance.OnLookTargetPosSet += vm.OnMousePosChanged;
        //}


        public static void OnCraftingModuleChanged(this Wdw_CraftingModuleViewModel vm, CraftingModule craftingModule)//콜백
        {
            vm.CraftingModule = craftingModule;
        }
        public static void OnRecipyDataChanged(this Wdw_CraftingModuleViewModel vm, RecipyData recipyData)//콜백
        {
            vm.RecipyData = recipyData;
        }
    }
}