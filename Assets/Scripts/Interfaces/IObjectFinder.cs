using System.Collections.Generic;
using UnityEngine;

public interface IObjectFinder
{
    T GetObject<T>();
    IEnumerable<T> GetObjects<T>();
    IEnumerable<GameObject> GetChildObjectsWithTag(Transform transform, string tag);
}
