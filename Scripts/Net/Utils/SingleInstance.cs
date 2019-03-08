/*****************
author：myth
*****************/
using UnityEngine;

public class SingleInstance<T> : MonoBehaviour where T : SingleInstance<T>
{
    private static T _Instance;
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                T[] objects = FindObjectsOfType<T>();
                if (objects.Length == 1)
                {
                    _Instance = objects[0];
                }
                else if (objects.Length > 1)
                {
                    Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}", typeof(T).ToString(), objects.Length);
                }
            }
            return _Instance;
        }
    }

    protected virtual void OnDestroy()
    {
        _Instance = null;
    }
}
