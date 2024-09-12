using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;
using EnumTypes;
using System.Collections.Generic;

public static class JsonDataManager
{
    public static JsonCache jsonCache = new JsonCache();
    public static T DataTableListLoad<T>(string saveDataFileName) where T : class, new()
    {
        string filePath = Application.dataPath + saveDataFileName;

        if (!File.Exists(filePath))
            return new T();

        string fileData = File.ReadAllText(filePath);

        // JSON에서 받은 경로를 Unity 경로로 변환
        string unityPath = fileData.Replace("\\", "/");
        unityPath = unityPath.Replace("//", "/");
        unityPath = unityPath.Replace("/UI/", "UI/");
        unityPath = unityPath.Replace("/Prefabs", "Prefabs/");
        unityPath = unityPath.Replace("/Resources/", ""); // "Resources/" 부분 제거
        unityPath = unityPath.Replace(".png", ""); // 파일 확장자 제거

        T data;

        try
        {
            data = JsonConvert.DeserializeObject<T>(unityPath);
            if (data == null)
            {
                data = new T();
                Debug.Log("새 저장 데이터 생성");
            }

            Debug.Log($"데이터 불러오기 완료 : {typeof(T).Name}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 불러오기 실패 : {e.Message}");
            return new T();
        }
    }
    public static void DataSaveCommand<T>(T jsonCacheData, string saveDataFileName)
    {
        Task task = DataSaveAsync(jsonCacheData, saveDataFileName);

        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogError($"데이터 저장 중 오류 발생: {t.Exception}");
            }
        });
    }
    static async Task DataSaveAsync<T>(T jsonCacheData, string saveDataFileName)
    {
        string filePath = Application.dataPath + saveDataFileName;

        string data = JsonConvert.SerializeObject(jsonCacheData, Formatting.Indented);

        await File.WriteAllTextAsync(filePath, data);

        Debug.Log($"<color=#FFFF00>데이터 저장 완료</color> : {typeof(T).Name}");
    }

    public static ItemData GetItem(string key)
    {
        if (jsonCache.ItemDataTableCache.dic.ContainsKey(key))
        {
            return jsonCache.ItemDataTableCache.dic[key];
        }
        else
        {
            Debug.LogError($"존재하지 않는 아이템 키입니다. : {key}");
            return null;
        }
    }
    public static ItemData GetItem(ushort key)
    {
        if (jsonCache.ItemDataTableCache.dic_Short.ContainsKey(key))
        {
            return jsonCache.ItemDataTableCache.dic_Short[key];
        }
        else
        {
            Debug.LogError($"존재하지 않는 아이템 키입니다. : {key}");
            return null;
        }
    }

    public static BuildingData GetBuilding(string key)
    {
        if (jsonCache.BuildingDataTableCache.dic.ContainsKey(key))
        {
            return jsonCache.BuildingDataTableCache.dic[key];
        }
        else
        {
            Debug.LogError("존재하지 않는 건물 아이템 키입니다.");
            return null;
        }
    }
    public static RecipyData GetRecipyData(string key)
    {
        if (jsonCache.RecipyDataTableCache.dic.ContainsKey(key))
        {
            return jsonCache.RecipyDataTableCache.dic[key];
        }
        else
        {
            Debug.LogError("존재하지 않는 레시피 데이터 키입니다.");
            return null;
        }
    }
    public static List<string> GetRecipyGroupData(string key)
    {
        if (jsonCache.RecipyGroupDataTableCache.dic.ContainsKey(key))
        {
            return jsonCache.RecipyGroupDataTableCache.dic[key];
        }
        else
        {
            Debug.LogError("존재하지 않는 레시피 그룹 데이터 키입니다.");
            return null;
        }
    }

    public static string GetText(string key, Language language)
    {
        return jsonCache.TextDataTableCache.dic[key].Text_Kr;
    }

    public class JsonCache
    {
        ItemDataTable _itemDataTableCache;
        public ItemDataTable ItemDataTableCache
        {
            get
            {
                if (_itemDataTableCache == null)
                {
                    _itemDataTableCache = JsonDataManager.DataTableListLoad<ItemDataTable>(ItemDataTable.FilePath());
                }
                return _itemDataTableCache;
            }
        }
        //=======================================================================
        BuildingDataTable _buildingDataTableCache;
        public BuildingDataTable BuildingDataTableCache
        {
            get
            {
                if (_buildingDataTableCache == null)
                {
                    _buildingDataTableCache = JsonDataManager.DataTableListLoad<BuildingDataTable>(BuildingDataTable.FilePath());
                }
                return _buildingDataTableCache;
            }
        }
        //=======================================================================
        RecipyGroupDataTable _recipyGroupDataTableCache;
        public RecipyGroupDataTable RecipyGroupDataTableCache
        {
            get
            {
                if (_recipyGroupDataTableCache == null)
                {
                    _recipyGroupDataTableCache = JsonDataManager.DataTableListLoad<RecipyGroupDataTable>(RecipyGroupDataTable.FilePath());
                }
                return _recipyGroupDataTableCache;
            }
        }
        //=======================================================================
        RecipyDataTable _recipyDataTableCache;
        public RecipyDataTable RecipyDataTableCache
        {
            get
            {
                if(_recipyDataTableCache == null)
                {
                    _recipyDataTableCache = JsonDataManager.DataTableListLoad<RecipyDataTable>(RecipyDataTable.FilePath());
                }
                return _recipyDataTableCache;
            }
        }
        //=======================================================================
        TextDataTable _textDataTableCache;
        public TextDataTable TextDataTableCache
        {
            get
            {
                if (_textDataTableCache == null)
                {
                    _textDataTableCache = JsonDataManager.DataTableListLoad<TextDataTable>(TextDataTable.FilePath());
                }
                return _textDataTableCache;
            }
        }
        public void Lode()
        {
            _itemDataTableCache = ItemDataTableCache;
            _buildingDataTableCache = BuildingDataTableCache;
            _recipyGroupDataTableCache = RecipyGroupDataTableCache;
            _recipyDataTableCache = RecipyDataTableCache;
            _textDataTableCache = TextDataTableCache;
        }
        public void Save()
        {
            JsonDataManager.DataSaveCommand(_itemDataTableCache, ItemDataTable.FilePath());
            JsonDataManager.DataSaveCommand(_buildingDataTableCache, BuildingDataTable.FilePath());
            JsonDataManager.DataSaveCommand(_recipyGroupDataTableCache, RecipyGroupDataTable.FilePath());
            JsonDataManager.DataSaveCommand(_recipyDataTableCache, RecipyDataTable.FilePath());
            JsonDataManager.DataSaveCommand(_textDataTableCache, TextDataTable.FilePath());
        }
    }
}