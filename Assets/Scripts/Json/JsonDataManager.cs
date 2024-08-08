using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;

public static class JsonDataManager
{
    public static JsonCache jsonCache = new JsonCache();
    public static T DataTableListLoad<T>(string saveDataFileName) where T : class, new()
    {
        string filePath = Application.dataPath + saveDataFileName;

        if (!File.Exists(filePath))
            return new T();

        string fileData = File.ReadAllText(filePath);
        T data;

        try
        {
            data = JsonConvert.DeserializeObject<T>(fileData);
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
}

public class JsonCache
{
    ItemTable _itemTableCache;
    public ItemTable ItemTableCache
    {
        get
        {
            if (_itemTableCache == null)
            {
                _itemTableCache = JsonDataManager.DataTableListLoad<ItemTable>(ItemTable.FilePath());
            }
            return _itemTableCache;
        }
    }

    public void Lode()
    {
        _itemTableCache = ItemTableCache;
    }
}