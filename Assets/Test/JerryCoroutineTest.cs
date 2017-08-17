using System.Collections;
using Jerry;
using UnityEngine;

public class JerryCoroutineTest : MonoBehaviour
{
    private JerryCoroutine.CorTask task1;

    void Start()
    {
        task1 = new JerryCoroutine.CorTask(IE_Test1(), true, (manual) =>
        {
            Debug.LogWarning("IE_Test1 Finished " + manual);
        });
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