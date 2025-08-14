using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDataBase<T> where T : BaseScriptableObject
{
    private Dictionary<int, T> _data;
    public ScriptableObjectDataBase(string path) 
    {
        _data = new();
        T[] allDatas = Resources.LoadAll<T>(path);

        foreach(var data in allDatas)
        {
            if (!_data.ContainsKey(data.ID)) 
            {
                _data[data.ID] = data;
            }
        }
    }

    public T GetById(int id)
    {
        _data.TryGetValue(id, out var result);
        return result;
    }
}
