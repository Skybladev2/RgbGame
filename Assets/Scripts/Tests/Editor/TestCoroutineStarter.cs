using System;
using System.Collections;

internal class TestCoroutineStarter : ICoroutineStarter
{
    public void StartCoroutine(IEnumerator routine)
    {
        while (routine.MoveNext()) { };
    }
}