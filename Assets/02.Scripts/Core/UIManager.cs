using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    Dictionary<string, BaseUI> uiInstances = new();
    private Transform mainCanvas;
    private Dictionary<string, Transform> canvasDictionary = new();

    private T CreateUI<T>(Transform parent = null) where T : BaseUI
    {
        string className = typeof(T).Name;
        string path = UIPrefabPath.GetPrefabPath(className);

        if(parent == null)
        {
            if (mainCanvas == null)
                mainCanvas = FindMainCanvas();
            parent = mainCanvas;
        }

        try 
        {
            if (uiInstances.ContainsKey(className))
            {
                return uiInstances[className] as T;
            }
            // Create new UI Instance
            if (UIPrefabPath.paths.ContainsKey(className))
            {
                Debug.Log(UIPrefabPath.paths[className]);
                GameObject go = Instantiate(Resources.Load<GameObject>(UIPrefabPath.paths[className]), parent);
                T t = go.GetComponent<T>();
                AddUI<T>(t);

                return t;
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Create UI Error : {e.Message}");
        }
        return default;
    } 

    public T ShowUI<T>(bool isDrawMainCanvas = false) where T : BaseUI
    {
        string className = typeof(T).Name;

        // T UI is Already created, Show UI and return
        if (uiInstances.ContainsKey(className))
        {
            uiInstances[className].ShowUI();
            return uiInstances[className] as T;
        }

        // T UI doesn't created yet, Create New UI Instance
        T t = CreateUI<T>(isDrawMainCanvas ? FindMainCanvas() : CreateCanvasTransform(className, false));
        if(t != default)
        {
            t.ShowUI();
            return t;

        }

        // Failed create T UI, Return default and Log Error
        Debug.LogError($"{className} UI create failue... ");
        return default;
    }

    private void AddUI<T>(BaseUI ui) where T : BaseUI
    {
        // Registed created UI Instance in Dictioanry
        uiInstances[typeof(T).Name] = ui;
    }

    private void RemoveUI<T>() where T : BaseUI
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

        if (canvasDictionary.ContainsKey(className))
        {
            Transform targetTrans = canvasDictionary[className];
            canvasDictionary.Remove(className);

            Destroy(targetTrans.gameObject);
        }
    }

    private Transform FindMainCanvas()
    {
        if(mainCanvas == null)
        {
            mainCanvas = CreateCanvasTransform("MainCanvas", true);
//            mainCanvas = Instantiate(Resources.Load<GameObject>(UIPrefabPath.GetPrefabPath(typeof(Canvas).Name))).transform;

        }
        return mainCanvas;
    }

    // Canvas ��ü �ƴ� ��� ���� ���� Canvas�� �Ű�������
    public void RegisterCanvas(Canvas canvas) 
    {
        mainCanvas = canvas.transform;
    }

    public Transform CreateCanvasTransform(string name, bool isMainCanvas = false)
    {
        if (canvasDictionary.ContainsKey(name)) return canvasDictionary[name];

        Transform go = Instantiate(Resources.Load<GameObject>(UIPrefabPath.GetPrefabPath(typeof(Canvas).Name))).transform;
        go.name = name;
        canvasDictionary[name] = go;
        if (isMainCanvas)
        {
            RegisterCanvas(go.gameObject.GetComponent<Canvas>());
        }
        return go;
    }
}
