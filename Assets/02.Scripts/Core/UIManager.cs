using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager : Singleton<UIManager>, IInitializableAsync
{
    Dictionary<string, BaseUI> uiInstances = new();
    private Transform mainCanvas;
    private Dictionary<string, Transform> canvasDictionary = new();
    private Dictionary<string, bool> uiEnableDict = new();

    private Dictionary<string, GameObject> uiInstanceCacheDict = new();

    public bool IsInitialized { get; private set; }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
    }
    public async void InitializeAsync()
    {
        var handles = Addressables.LoadAssetsAsync<GameObject>("UIInstance", obj =>
        {
            var ui = obj.GetComponent<BaseUI>();
            if(ui != null)
            {
                var uiClassName = ui.GetType().Name;
                uiInstanceCacheDict[uiClassName] = obj;
                Debug.Log(uiClassName);
            }
        });

        var canvasHandle = Addressables.LoadAssetsAsync<GameObject>("Canvas", obj =>
        {
            var canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                uiInstanceCacheDict["Canvas"] = obj;
            }
        });

        await handles.Task;
        await canvasHandle.Task;

        foreach(var obj in uiInstanceCacheDict)
        {
            Debug.Log($"{obj.Key} : {obj.Value}");
        }

        IsInitialized = true;
    }

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
                if (uiInstanceCacheDict.ContainsKey(className))
                {
                    GameObject go = Instantiate(uiInstanceCacheDict[className], parent); 
                    T t = go.GetComponent<T>();
                    AddUI<T>(t);
                    return t;
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Create UI Error : {e.Message}");
        }
        return default;
    } 

    public async Task<T> ShowUI<T>(bool isDrawMainCanvas = false) where T : BaseUI
    {
        await GameManager.Instance.WaitInitializedAsync();
        string className = typeof(T).Name;

        // T UI is Already created, Show UI and return
        if (uiInstances.ContainsKey(className))
        {
            uiInstances[className].ShowUI();
            uiEnableDict[className] = true;
            return uiInstances[className] as T;
        }

        // T UI doesn't created yet, Create New UI Instance
        T t = CreateUI<T>(isDrawMainCanvas ? FindMainCanvas() : CreateCanvasTransform(className, false));
        if(t != default)
        {
            uiInstances[className] = t;
            uiEnableDict[className] = true;
            t.ShowUI();
            return t;
        }

        // Failed create T UI, Return default and Log Error
        Debug.LogError($"{className} UI create failue... ");
        return default;
    }

    public void CloseUI<T>() where T : BaseUI
    {
        string className = typeof(T).Name;

        if (uiInstances.ContainsKey(className))
        {
            Debug.Log($"Close UI {className}");
            uiInstances[className].ExitUI();
            uiEnableDict[className] = false;
        }
        else
        {
            Debug.Log("없는데?");
        }
    }

    public bool IsEnableUI<T>() where T : BaseUI
    {
        string className = typeof(T).Name;
        if (uiEnableDict.ContainsKey(className))
        {
            Debug.Log($"있어. {uiEnableDict[className]}");
            return uiEnableDict[className];
        }

        Debug.Log($"없어. {false}");
        return false;
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
            uiEnableDict.Remove(className);
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

    // Canvas 객체 아닌 경우 검증 위해 Canvas만 매개변수로
    public void RegisterCanvas(Canvas canvas) 
    {
        mainCanvas = canvas.transform;
    }

    public Transform CreateCanvasTransform(string name, bool isMainCanvas = false)
    {
        Debug.Log("캔버스 만들기 시도");
        if (canvasDictionary.ContainsKey(name)) return canvasDictionary[name];
        Debug.Log($"{name} 캔버스 없음. 만들거임");

        Transform go = Instantiate(uiInstanceCacheDict["Canvas"]).transform;
        Debug.Log($"{go.name} 만들었음!");
        go.name = name;
        canvasDictionary[name] = go;
        if (isMainCanvas)
        {
            RegisterCanvas(go.gameObject.GetComponent<Canvas>());
        }
        return go;
    }

}
