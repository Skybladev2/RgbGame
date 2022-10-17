using System.Collections;
using UnityEngine;

public class CoroutineStarter : ICoroutineStarter
{
    private MonoBehaviour coroutineStarter;

    public CoroutineStarter(MonoBehaviour coroutineStarter)
    {
        this.coroutineStarter = coroutineStarter;
    }

    public void StartCoroutine(IEnumerator routine)
    {
        coroutineStarter.StartCoroutine(routine);
    }
}
