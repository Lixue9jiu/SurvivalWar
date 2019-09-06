using Unity.Jobs;
using System.Runtime.InteropServices;

public struct ManagedJob : IJob
{
    public GCHandle handle;
    public void Execute()
    {
        ITask t = (ITask)handle.Target;
        t.Execute();
    }
}