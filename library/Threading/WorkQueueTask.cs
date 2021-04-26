using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

/*
    Threading constructs:
    lock (object) {code here} // This is synatic sugar for the Monitor object?
    Semaphore
    EventWaitHandle (ManualResetEvent/AutoResetEvent)
    Monitor (Monitor.Enter(object)/Monitor.Exit())
    SpinLock
    Interlocked
    Channels

    Any others?
*/

namespace SparkLib
{
    public class WorkQueueTask
    {
        public Task? workTask;

        public WorkQueueTask()
        {

        }

        // If you have arguments, use this version
        public Task EnqueueWork(Action task)
        {
            if (workTask == null)
            {
                workTask = new Task(task);
                workTask.Start();
            }
            else
            {
                workTask.ContinueWith((antecedent) => task());
            }
            return workTask;
        }
    }
}
