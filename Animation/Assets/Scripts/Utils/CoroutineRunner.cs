using UnityEngine;
using System.Collections;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("CoroutineRunner");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }

    public static Coroutine Run(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static void Stop(Coroutine routine)
    {
        if (_instance != null && routine != null)
        {
            _instance.StopCoroutine(routine);
        }
    }

    public static void StopAll()
    {
        if (_instance != null)
        {
            _instance.StopAllCoroutines();
        }
    }
}
