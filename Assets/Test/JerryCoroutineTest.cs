using System.Collections;
using Jerry;
using UnityEngine;

public class JerryCoroutineTest : MonoBehaviour
{
    public int taskId = 0;
    private JerryCoroutine.CorTask task = null;

    void Start()
    {
        AddOneTask();
    }

    [ContextMenu("AddOneTask")]
    private void AddOneTask()
    {
        task = new JerryCoroutine.CorTask(IE_Test(taskId), true, null, task);
    }

    void OnDestroy()
    {
        JerryCoroutine.StopTask(task);
    }

    private IEnumerator IE_Test(int id)
    {
        while (true)
        {
            Debug.LogWarning("IE_Test " + id);
            yield return Yielders.GetWaitForSeconds(1.0f);
        }
    }
}