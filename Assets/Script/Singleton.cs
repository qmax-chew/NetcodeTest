using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    private static bool applicationIsQuiting = false;

    protected virtual void Awake()
    {
        // 複数のインスタンスを防止
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this as T)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public static T Instance()
    {
        if (applicationIsQuiting)
        {
            return null;
        }

        if (_instance == null)
        {
            // インスタンスがなかったら生成する
            _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                _instance = obj.AddComponent<T>();
            }
        }

        //インスタンスを返す
        return _instance;
    }

    private void OnApplicationQuit()
    {
        applicationIsQuiting = true;
    }
}