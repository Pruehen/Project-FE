using UnityEngine;

public class BuildMeshRenderer : SceneSingleton<BuildMeshRenderer>
{   
    [SerializeField] Material Material_Translucent;

    GameObject objTemp;
    Vector3Int drawTemp;
    bool isDraw = false;
    public void DrawMesh(Vector3Int center, Quaternion dir, GameObject prefab)
    {
        if (isDraw == true && drawTemp == center)
        {
            return;
        }
        drawTemp = center;
        isDraw = true;


        // 다른 타입일 경우 기존 오브젝트 삭제
        if (objTemp == null || objTemp.name != prefab.name)
        {
            RemoveMesh();
            objTemp = Instantiate(prefab, center, dir);

            // 1. obj의 모든 컴포넌트를 지운다.
            Component[] components = objTemp.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (!(component is Transform)) // Transform 컴포넌트 제외
                {
                    Destroy(component);
                }
            }

            // 2. obj에 있는 모든 메시를 검색해서 마테리얼을 변경한다.
            MeshRenderer[] meshRenderers = objTemp.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in meshRenderers)
            {
                renderer.material = Material_Translucent; // 새로운 마테리얼로 변경
            }
        }
        else
        {
            objTemp.transform.position = center;
            objTemp.transform.rotation = dir;
        }
    }

    public void RemoveMesh()
    {
        if(objTemp != null)
        {
            Destroy(objTemp);
            objTemp = null;
        }
        isDraw = false;
    }
}
