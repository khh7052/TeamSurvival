using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    [SerializeField] private bool dontDestroyOnLoad = true;
    private static bool IsApplicationQuit = false;
    public static T Instance
    {
        get
        {
            if (IsApplicationQuit)
                return null;
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singletonObject = new(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Initialize();

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Initialize() { }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
        IsApplicationQuit = true;
    }
}
