using UnityEngine;
public class SingletonBehaviour : MonoBehaviour
{
    private static SingletonBehaviour instance;

    void Awake()
    {
        if (instance)
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(gameObject);
#endif
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
