using UnityEngine;

namespace ViewModel.Extensions
{
    public static class Wdw_InventoryViewModelExtension
    {
        public static void Register_OnMouseObjectNameChanged(this Wdw_InventoryViewModel vm)
        {
            Player.Instance.OnMouseObjectNameChanged += vm.OnMouseObjectNameChanged;
            Player.Instance.OnLookTargetPosSet += vm.OnMousePosChanged;
        }

        public static void UnRegister_OnMouseObjectNameChanged(this Wdw_InventoryViewModel vm)
        {
            Player.Instance.OnMouseObjectNameChanged -= vm.OnMouseObjectNameChanged;
            Player.Instance.OnLookTargetPosSet -= vm.OnMousePosChanged;
        }

        public static void Refresh_OnMouseObjectNameChanged(this Wdw_InventoryViewModel vm)
        {
            vm.OnMouseObjectNameChanged(null);
        }


        public static void OnMouseObjectNameChanged(this Wdw_InventoryViewModel vm, string objectName)//콜백
        {
            vm.OnMouseObjectName = objectName;
        }
        public static void OnMousePosChanged(this Wdw_InventoryViewModel vm, Vector3 mousePos)//콜백
        {
            vm.MousePosition = mousePos;
        }
    }
}