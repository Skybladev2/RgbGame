using System.Collections.Generic;
using UnityEngine;

internal class ObjectFinder : IObjectFinder
{
    public IEnumerable<GameObject> GetChildObjectsWithTag(Transform transform, string tag)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.tag == tag)
            {
                children.Add(child.gameObject);
            }
        }

        return children;
    }

    public T GetObject<T>()
    {
        MonoBehaviour[] objects = GameObject.FindObjectsOfType<MonoBehaviour>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is T)
            {
                return (T)(System.Object)objects[i];
            }
        }

        return default(T);
    }

    public IEnumerable<T> GetObjects<T>()
    {
        MonoBehaviour[] objects = GameObject.FindObjectsOfType<MonoBehaviour>();
        List<T> foundObjects = new List<T>();

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is T)
            {
                foundObjects.Add((T)(System.Object)objects[i]);
            }
        }

        return foundObjects;
    }
}
