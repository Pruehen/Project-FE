using System.Collections.Generic;
using UnityEngine;

public class BuildLineRenderer : SceneSingleton<BuildLineRenderer>
{    
    const float lineFloat = 0.2f; // 그리드 선 부유 거리
    const float lineWidth = 0.05f; // 기본 그리드 선 두께
    const float lineWidth_Heavy = 0.075f; // 두꺼운 그리드 선 두께

    List<GameObject> gridLines = new List<GameObject>(); // 그려진 라인을 저장할 리스트
    List<GameObject> pathNodes = new List<GameObject>(); // 그려진 라인을 저장할 리스트

    int currentLineIndex = 0; // 현재 재활용할 라인의 인덱스       

    [SerializeField] GameObject Mesh_SelectNodePoint;
    [SerializeField] GameObject Mesh_PathNodePoint;

    public void DrawBeltLine(List<Vector3> linePosList)
    {
        // 먼저 이전에 그렸던 라인들을 모두 비활성화
        HideAllGridLinesAndNodes();

        //라인 경로 그리기
        for (int i = 1; i < linePosList.Count; i++)
        {
            DrawLine(linePosList[i - 1], linePosList[i], lineWidth);
        }

        for (int i = 0; i < linePosList.Count; i++)
        {
            DrawPathNode(linePosList[i]);
        }
    }
    public void DrawBeltLine(List<Vector3Int> linePosList)
    {
        // 먼저 이전에 그렸던 라인들을 모두 비활성화
        HideAllGridLinesAndNodes();

        //라인 경로 그리기
        for (int i = 1; i < linePosList.Count; i++)
        {
            DrawLine(linePosList[i - 1], linePosList[i], lineWidth);
        }

        for (int i = 0; i < linePosList.Count; i++)
        {
            DrawPathNode(linePosList[i]);
        }
    }

    void DrawLine(Vector3 start, Vector3 end, float width)
    {
        GameObject line;

        // 재활용 가능한 라인이 있으면 가져와서 재활용
        if (currentLineIndex < gridLines.Count)
        {
            line = gridLines[currentLineIndex];
            line.SetActive(true); // 활성화
        }
        else
        {
            // 재활용 가능한 라인이 없으면 새로 생성
            line = new GameObject("GridLine");
            line.transform.parent = this.transform;
            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            gridLines.Add(line); // 리스트에 추가
        }

        start.y += lineFloat;
        end.y += lineFloat;

        // BuildLineRenderer 설정
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;

        currentLineIndex++; // 인덱스를 증가시켜 다음 라인을 처리
    }
    void DrawPathNode(Vector3 point)
    {
        pathNodes.Add(ObjectPoolManager.Instance.DequeueObject(Mesh_PathNodePoint, point));
    }

    public void HideAllGridLinesAndNodes()
    {
        // 그리드 라인을 모두 비활성화하고 재활용 준비
        for (int i = 0; i < gridLines.Count; i++)
        {
            gridLines[i].SetActive(false);
        }

        // 새로 그릴 때 처음부터 사용할 수 있도록 인덱스를 0으로 초기화
        currentLineIndex = 0;

        foreach (var item in pathNodes)
        {
            ObjectPoolManager.Instance.EnqueueObject(item);
        }
        pathNodes.Clear();
    }    
}
