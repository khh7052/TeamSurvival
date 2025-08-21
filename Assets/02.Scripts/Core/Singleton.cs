using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ISingletonResetData
{
    void ResetData();
}


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    [SerializeField] protected bool dontDestroyOnLoad = true;

    // 이 플래그가 모든 싱글톤의 종료 상태를 관리합니다.
    public static bool IsApplicationQuit { get; private set; } = false;
    public static bool IsDestroying { get; private set; } = false;
    public static T Instance
    {
        get
        {
            if (IsApplicationQuit || !SingletonRegistry.IsCanCreateSingleton || IsDestroying)
                return null;

            if (instance == null)
            {
                if (!SingletonRegistry.IsCanCreateSingleton)
                {
                    return null;
                }
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

    protected virtual void OnDestroy()
    {
        IsDestroying = true; // 이 프레임에선 절대 Instance 생성 금지
        if (instance == this) instance = null;
        SingletonRegistry.Unregister(this);
        IsDestroying = false;
    }

    protected virtual void Awake()
    {
        // 게임 시작 시, 플래그를 초기화
        IsApplicationQuit = false;

        if (instance == null)
        {
            if (SingletonRegistry.IsCanCreateSingleton)
            {
                instance = this as T;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            SingletonRegistry.Register(this); //  등록

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Initialize() { }

    private void OnApplicationQuit()
    {
        Debug.Log("종료!");
        // 애플리케이션 종료 플래그를 true로 설정
        IsApplicationQuit = true;
    }

    public void Release()
    {
        instance = null;
        Destroy(gameObject);
    }

}