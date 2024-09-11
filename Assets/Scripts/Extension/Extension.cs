
using EnumTypes;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;

public static class Extension
{
    public static Node FindClosest(this List<Node> list, Vector3 position)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("The transforms list is empty or null.");
            return null;
        }

        Node closestTransform = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Node node in list)
        {
            float distanceSqr = (node.gridPos - position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestTransform = node;
            }
        }

        return closestTransform;
    }

    public static void InsertionCellSort<T>(this List<T> list) where T : CellData, IComparable<T>
    {
        for (int i = 1; i < list.Count; i++)
        {
            T key = list[i];
            int j = i - 1;

            // 이동할 위치를 찾습니다
            while (j >= 0 && list[j].CompareTo(key) > 0)
            {
                j--;
            }

            // j+1이 key의 최종 위치입니다.
            // 위치가 i와 다를 경우에만 교환을 수행합니다.
            if (j + 1 != i)
            {
                list[j + 1].Swap_OnSort(list[i]);                
            }
        }
    }

    public class VM
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)//값이 변경되었을 때 이벤트를 발생시키기 위한 용도 (데이터 바인딩)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static void SetUIPos_WorldToScreenPos(this RectTransform rectTransform, Vector3 originPos)
    {
        Camera cam = GetHighestPriorityCamera();

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 screenPosition = cam.WorldToScreenPoint(originPos);
        bool isOutsideOfCamera = (screenPosition.z < 0);// ||
                                                        //screenPosition.x < 0 || screenPosition.x > screenSize.x ||
                                                        //screenPosition.y < 0 || screenPosition.y > screenSize.y);

        if (isOutsideOfCamera)
        {
            rectTransform.anchoredPosition = new Vector2(-3000, -3000);
        }
        else
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, originPos);
            Vector2 position = screenPoint - screenSize * 0.5f;
            rectTransform.anchoredPosition = position;
        }
    }
    public static void ClampToScreen(this RectTransform rectTransform)
    {
        // 화면 크기 가져오기
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // RectTransform의 부모 캔버스에서의 위치를 계산
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // RectTransform의 현재 앵커드 포지션 가져오기
        Vector2 anchoredPosition = rectTransform.anchoredPosition;

        // X 좌표 클램프 (왼쪽 및 오른쪽이 화면 밖으로 나가는 것을 방지)
        if (corners[0].x < 0) // RectTransform의 왼쪽이 화면 밖으로 나간 경우
        {
            anchoredPosition.x += 0 - corners[0].x;
        }
        else if (corners[2].x > screenWidth) // RectTransform의 오른쪽이 화면 밖으로 나간 경우
        {
            anchoredPosition.x -= corners[2].x - screenWidth;
        }

        // Y 좌표 클램프 (위쪽 및 아래쪽이 화면 밖으로 나가는 것을 방지)
        if (corners[0].y < 0) // RectTransform의 아래쪽이 화면 밖으로 나간 경우
        {
            anchoredPosition.y += 0 - corners[0].y;
        }
        else if (corners[1].y > screenHeight) // RectTransform의 위쪽이 화면 밖으로 나간 경우
        {
            anchoredPosition.y -= corners[1].y - screenHeight;
        }

        // 클램핑된 앵커드 포지션을 다시 설정
        rectTransform.anchoredPosition = anchoredPosition;
    }

    public static Camera GetHighestPriorityCamera()
    {
        Camera[] allCameras = Camera.allCameras;
        Camera highestPriorityCamera = null;
        float maxDepth = float.MinValue;

        foreach (Camera cam in allCameras)
        {
            if (cam.depth > maxDepth)
            {
                maxDepth = cam.depth;
                highestPriorityCamera = cam;
            }
        }

        return highestPriorityCamera;
    }

    public static string GetTextTable(this string id, Language languageType = Language.Kr)
    {
        return JsonDataManager.GetText(id, languageType);
    }
    public static void SetLoadSprite(this Image image, string path)
    {
        // Resources.Load를 사용하여 스프라이트를 로드합니다.
        Sprite lodeSprite = Resources.Load<Sprite>(path);
        if(lodeSprite != null)
        {
            image.sprite = lodeSprite;
        }
    }

    public static string SimplifyNumber(this float number)
    {
        if (number >= 1_000_000_000_000_000_000)
        {
            return $"{number / 1_000_000_000_000_000_000.0:F1}E";
        }
        else if (number >= 1_000_000_000_000_000)
        {
            return $"{number / 1_000_000_000_000_000.0:F1}P";
        }
        else if (number >= 1_000_000_000_000)
        {
            return $"{number / 1_000_000_000_000.0:F1}T";
        }
        else if (number >= 1_000_000_000)
        {
            return $"{number / 1_000_000_000.0:F1}G";
        }
        else if (number >= 1_000_000)
        {
            return $"{number / 1_000_000.0:F1}M";
        }
        else if (number >= 1_000)
        {
            return $"{number / 1_000.0:F1}k";
        }
        else
        {
            return $"{number:F0}";
        }
    }
}