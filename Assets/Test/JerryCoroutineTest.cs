using UnityEngine;
using System.Collections;
using Jerry;

public class JerryCoroutineTest : MonoBehaviour
{
    private CoroutineTask task1;

    void Start()
    {
        task1 = new CoroutineTask(IE_Test1(), true);
        task1.Finished += (manual) =>
        {
            Debug.LogWarning("IE_Test1 Finished " + manual);
        };
    }

    void OnDestroy()
    {
        if (task1 != null)
        {
            task1.Stop();
        }
    }

    private IEnumerator IE_Test1()
    {
        Debug.LogWarning("IE_Test1 1");
        yield return Yielders.GetWaitForSeconds(1.0f);
        Debug.LogWarning("IE_Test1 2");
    }
}