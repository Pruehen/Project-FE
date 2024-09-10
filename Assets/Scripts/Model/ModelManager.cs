using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager
{
    static ModelManager _instance;
    public static ModelManager Instance
    {
        get 
        { 
            if(_instance == null)
            {
                _instance = new ModelManager();
            }
            return _instance;            
        }       
    }

    public static Dictionary<int, CraftingModuleModel> _craftingModuleModelDic = new Dictionary<int, CraftingModuleModel>();
    public static Dictionary<int, MinerModuleModel> _minerModuleModelDic = new Dictionary<int, MinerModuleModel>();
    public static T NewModel<T>(int instanceId) where T : class, new()
    {
        // 타입을 확인하고, 인스턴스를 생성합니다.
        if (typeof(T) == typeof(CraftingModuleModel))
        {
            if (_craftingModuleModelDic.ContainsKey(instanceId))
            {
                throw new ArgumentException($"An instance with ID {instanceId} already exists.");
            }

            // 인스턴스를 생성하고, 딕셔너리에 추가합니다.
            T instance = new T();
            _craftingModuleModelDic.Add(instanceId, instance as CraftingModuleModel);
            return instance;
        }
        else if (typeof(T) == typeof(MinerModuleModel))
        {
            if (_craftingModuleModelDic.ContainsKey(instanceId))
            {
                throw new ArgumentException($"An instance with ID {instanceId} already exists.");
            }

            // 인스턴스를 생성하고, 딕셔너리에 추가합니다.
            T instance = new T();
            _minerModuleModelDic.Add(instanceId, instance as MinerModuleModel);
            return instance;
        }
        else
        {
            // 지원하지 않는 타입에 대한 처리
            throw new ArgumentException("Unsupported model type", nameof(T));
        }
    }
}