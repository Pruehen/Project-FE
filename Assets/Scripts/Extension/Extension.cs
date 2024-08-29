
using EnumTypes;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class Extension
{
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
                list[j + 1].Swap(list[i]);                
            }
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
        image.sprite = Resources.Load<Sprite>(path);
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