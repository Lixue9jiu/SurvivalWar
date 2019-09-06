using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Jobs;

public class TaskManager : MonoBehaviour
{
    List<JobHandle> jobHandles = new List<JobHandle>();
    List<GCHandle> gcHandles = new List<GCHandle>();

    public void SchaduleTask(ITask task)
    {
        var gc = GCHandle.Alloc(task);
        var job = new ManagedJob{ handle = gc }.Schedule();
        gcHandles.Add(gc);
        jobHandles.Add(job);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < jobHandles.Count;)
        {
            var job = jobHandles[i];
            if (job.IsCompleted)
            {
                job.Complete();
                var gc = gcHandles[i];
                (gc.Target as ITask).CallBack();
                gc.Free();
                jobHandles.RemoveAt(i);
                gcHandles.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
