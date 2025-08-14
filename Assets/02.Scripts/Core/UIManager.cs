using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    Dictionary<string, BaseUI> uiInstances = new();
    public Transform mainCanvas;

    public T CreateUI<T>(Transform parent = null) where T : BaseUI
    {
        string className = typeof(T).Name;
        string path = UIPrefabPath.GetPrefabPath(className);

        if(parent == null)
        {
            if (mainCanvas == null)
                mainCanvas = FindMainCanvas();
            parent = mainCanvas;
        }

        Debug.Log(className);
        Debug.Log(path);
        try 
        {
            if (uiInstances.ContainsKey(className))
            {
                return uiInstances[className] as T;
            }
            // Create new UI Instance
            GameObject go = Instantiate(Resources.Load<GameObject>("UI/ConditionUI"), parent);
            T t = go.GetComponent<T>();
            AddUI<T>(t);

            return t;
        }
        catch(Exception e)
        {
            Debug.LogError($"Create UI Error : {e.Message}");
        }
        return default;
    } 

    public T ShowUI<T>(Transform parent = null) where T : BaseUI
    {
        string className = typeof(T).Name;

        // T UI is Already created, Show UI and return
        if (uiInstances.ContainsKey(className))
        {
            uiInstances[className].ShowUI();
            return uiInstances[className] as T;
        }

        // T UI doesn't created yet, Create New UI Instance
        T t = CreateUI<T>(parent);
        if(t != default)
        {
            t.ShowUI();
            return t;

        }

        // Failed create T UI, Return default and Log Error
        Debug.LogError($"{className} UI create failue... ");
        return default;
    }

    public void AddUI<T>(BaseUI ui) where T : BaseUI
    {
        // Registed created UI Instance in Dictioanry
        uiInstances[typeof(T).Name] = ui;
    }

    public void RemoveUI<T>() where T : BaseUI
    {
        string className = nameof(T);

        if (uiInstances.ContainsKey(className))
        {
            // Find Target UI Instance
            BaseUI targetUI = uiInstances[nameof(T)];
            uiInstances.Remove(className);

            // Destroy UI Instace
            Destroy(targetUI.gameObject);
        }
    }

    private Transform FindMainCanvas()
    {
        if(mainCanvas == null)
        {
            mainCanvas = Instantiate(Resources.Load<GameObject>(UIPrefabPath.GetPrefabPath(typeof(Canvas).Name))).transform;
        }
        return mainCanvas;
    }

    // Canvas 객체 아닌 경우 검증 위해 Canvas만 매개변수로
    public void RegisterCanvas(Canvas canvas) 
    {
        mainCanvas = canvas.transform;
    }
}
