using UnityEngine;

namespace ViewModel.Extensions
{
    public static class PlayerViewModelExtension
    {
        public static void Register_OnMouseObjectNameChanged(this PlayerViewModel vm)
        {
            Player.Instance.OnMouseObjectNameChanged += vm.OnMouseObjectNameChanged;
            Player.Instance.OnLookTargetPosSet += vm.OnMousePosChanged;
        }

        public static void UnRegister_OnMouseObjectNameChanged(this PlayerViewModel vm)
        {
            Player.Instance.OnMouseObjectNameChanged -= vm.OnMouseObjectNameChanged;
            Player.Instance.OnLookTargetPosSet -= vm.OnMousePosChanged;
        }

        public static void Refresh_OnMouseObjectNameChanged(this PlayerViewModel vm)
        {
            vm.OnMouseObjectNameChanged(null);
        }


        public static void OnMouseObjectNameChanged(this PlayerViewModel vm, string objectName)//�ݹ�
        {
            vm.OnMouseObjectName = objectName;
        }
        public static void OnMousePosChanged(this PlayerViewModel vm, Vector3 mousePos)//�ݹ�
        {
            vm.MousePosition = mousePos;
        }
    }
}