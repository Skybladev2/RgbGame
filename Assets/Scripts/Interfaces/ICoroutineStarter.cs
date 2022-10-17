using System.Collections;

public interface ICoroutineStarter
{
    void StartCoroutine(IEnumerator routine);
}