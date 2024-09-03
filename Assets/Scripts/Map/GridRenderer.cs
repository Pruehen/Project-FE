using UnityEngine;
using System.Collections.Generic;

public class GridRenderer : SceneSingleton<GridRenderer>
{
    const float gridSpacing = 1.0f; // 그리드 간격
    const float lineWidth = 0.05f; // 기본 그리드 선 두께
    const float lineWidth_Heavy = 0.15f; // 두꺼운 그리드 선 두께

    [SerializeField] Color gridColor = Color.gray; // 그리드 색상    
    [SerializeField] int gridSize = 100; // 그리드 크기

    List<GameObject> gridLines = new List<GameObject>(); // 그려진 라인을 저장할 리스트
    int currentLineIndex = 0; // 현재 재활용할 라인의 인덱스

    Vector3 drawTemp;
    bool isDraw = false;

    public void DrawGrid(Vector3 gridCenter)
    {
        if(isDraw == true && drawTemp == gridCenter)
        {
            return;
        }
        drawTemp = gridCenter;
        isDraw = true;

        // 먼저 이전에 그렸던 라인들을 모두 비활성화
        HideAllGridLines();

        // X 방향 라인 그리기 (Z축에 평행한 라인)
        for (float x = -gridSize + 1; x < gridSize; x += gridSpacing)
        {
            float width = Mathf.Abs(x + gridCenter.x) % 10 == 0 ? lineWidth_Heavy : lineWidth;
            DrawLine(new Vector3(x, 0, -gridSize) + gridCenter, new Vector3(x, 0, gridSize) + gridCenter, width);
        }

        // Z 방향 라인 그리기 (X축에 평행한 라인)
        for (float z = -gridSize + 1; z < gridSize; z += gridSpacing)
        {
            float width = Mathf.Abs(z + gridCenter.z) % 10 == 0 ? lineWidth_Heavy : lineWidth;
            DrawLine(new Vector3(-gridSize, 0, z) + gridCenter, new Vector3(gridSize, 0, z) + gridCenter, width);
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

        // LineRenderer 설정
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.startColor = gridColor;
        lineRenderer.endColor = gridColor;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;

        currentLineIndex++; // 인덱스를 증가시켜 다음 라인을 처리
    }

    public void HideAllGridLines()
    {
        if(isDraw == false)
        {
            return;
        }
        isDraw = true;

        // 그리드 라인을 모두 비활성화하고 재활용 준비
        for (int i = 0; i < gridLines.Count; i++)
        {
            gridLines[i].SetActive(false);
        }

        // 새로 그릴 때 처음부터 사용할 수 있도록 인덱스를 0으로 초기화
        currentLineIndex = 0;
    }
}