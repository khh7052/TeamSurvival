using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private readonly TaskCompletionSource<bool> _initializeTcs = new();

    protected override void Initialize()
    {
        base.Initialize();
        uiInstances.Clear();
        canvasDictionary.Clear();
        uiEnableDict.Clear();
        uiInstanceCacheDict.Clear();

        InitializeAsync();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var ui in uiInstances.Values.ToList())
        {
            if (ui != null)
            {
                Addressables.ReleaseInstance(ui.gameObject);
            }
        }
        foreach (var canvas in canvasDictionary.Values.ToList())
        {
            if (canvas != null)
            {
                Addressables.ReleaseInstance(canvas.gameObject);
            }
        }

        uiInstances.Clear();
        canvasDictionary.Clear();
        uiEnableDict.Clear();
        uiInstanceCacheDict.Clear();
        mainCanvas = null;
    }

    public async void InitializeAsync()
    {
        var handles = Addressables.LoadAssetsAsync<GameObject>("UIInstance", obj =>
        {
            var ui = obj.GetComponent<BaseUI>();
            if (ui != null)
            {
                var uiClassName = ui.GetType().Name;
                uiInstanceCacheDict[uiClassName] = obj;
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

        IsInitialized = true;
        _initializeTcs.TrySetResult(true);
    }

    // UIManager의 초기화가 완료될 때까지 기다리는 메서드
    public Task WaitInitializedAsync()
    {
        if (IsInitialized)
        {
            return Task.CompletedTask;
        }
        return _initializeTcs.Task;
    }

    private T CreateUI<T>(Transform parent = null) where T : BaseUI
    {
        string className = typeof(T).Name;

        if (parent == null)
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
            if (uiInstanceCacheDict.ContainsKey(className))
            {
                GameObject go = Instantiate(uiInstanceCacheDict[className], parent);
                T t = go.GetComponent<T>();
                AddUI<T>(t);
                return t;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Create UI Error : {e.Message}");
        }
        return default;
    }

    public async Task<T> ShowUI<T>(bool isDrawMainCanvas = false) where T : BaseUI
    {
        // UIManager의 초기화가 완료될 때까지 기다립니다.
        await WaitInitializedAsync();

        // GameManager의 초기화 대기 로직은 GameManager에서 처리해야 합니다.
        // 현재 코드에서는 UIManager의 초기화 대기만으로도 충분할 수 있습니다.
        // await GameManager.Instance.WaitInitializedAsync();

        string className = typeof(T).Name;

        if (uiInstances.ContainsKey(className))
        {
            uiInstances[className]?.ShowUI();
            uiEnableDict[className] = true;
            return uiInstances[className] as T;
        }

        T t = CreateUI<T>(isDrawMainCanvas ? FindMainCanvas() : CreateCanvasTransform(className, false));
        if (t != default)
        {
            uiInstances[className] = t;
            uiEnableDict[className] = true;
            t.ShowUI();
            return t;
        }

        Debug.LogError($"{className} UI create failue... ");
        return default;
    }

    public void CloseUI<T>() where T : BaseUI
    {
        string className = typeof(T).Name;
        if (uiInstances.ContainsKey(className))
        {
            uiInstances[className].ExitUI();
            uiEnableDict[className] = false;
        }
    }

    public bool IsEnableUI<T>() where T : BaseUI
    {
        string className = typeof(T).Name;
        return uiEnableDict.ContainsKey(className) && uiEnableDict[className];
    }

    private void AddUI<T>(BaseUI ui) where T : BaseUI
    {
        uiInstances[typeof(T).Name] = ui;
    }

    private void RemoveUI<T>() where T : BaseUI
    {
        string className = typeof(T).Name;
        if (uiInstances.ContainsKey(className))
        {
            BaseUI targetUI = uiInstances[className];
            uiInstances.Remove(className);
            uiEnableDict.Remove(className);
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
        if (mainCanvas == null)
        {
            mainCanvas = CreateCanvasTransform("MainCanvas", true);
        }
        return mainCanvas;
    }

    public void RegisterCanvas(Canvas canvas)
    {
        mainCanvas = canvas.transform;
    }

    public Transform CreateCanvasTransform(string name, bool isMainCanvas = false)
    {
        if (canvasDictionary.ContainsKey(name)) return canvasDictionary[name];

        if (uiInstanceCacheDict.ContainsKey("Canvas"))
        {
            GameObject go = Instantiate(uiInstanceCacheDict["Canvas"]);
            go.name = name;
            Transform trans = go.transform;
            canvasDictionary[name] = trans;

            if (isMainCanvas)
            {
                RegisterCanvas(go.GetComponent<Canvas>());
            }
            return trans;
        }
        Debug.LogError("Canvas 프리팹을 찾을 수 없습니다.");
        return null;
    }
}