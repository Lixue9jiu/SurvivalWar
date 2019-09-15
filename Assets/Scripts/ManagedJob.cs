using UnityEngine;
using Unity.Jobs;
using System.Runtime.InteropServices;

public struct ManagedJob : IJob
{
    public GCHandle handle;
    public void Execute()
    {
        try
        {
            ITask t = (ITask)handle.Target;
            t.Execute();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
}