using System;
using System.Collections.Concurrent;
using System.Threading;

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
    public class WorkQueueThread
    {
        protected Thread workThread;
        protected CancellationTokenSource source = new CancellationTokenSource(); // Saw this somewhere, not sure why they used it.
                                                                                  // Using it here to remind myself to look into it.
                                                                                  //protected EventWaitHandle threadSuspend = new EventWaitHandle(false, EventResetMode.ManualReset);
                                                                                  //protected EventWaitHandle threadSuspend = new EventWaitHandle(false, EventResetMode.ManualReset);
        protected ManualResetEventSlim re = new ManualResetEventSlim(false);
        protected ConcurrentQueue<ThreadQueueItem> workQueue = new ConcurrentQueue<ThreadQueueItem>();

        public WorkQueueThread()
        {
            workThread = new Thread(ThreadProc);
            workThread.IsBackground = true;
            workThread.Start();
        }

        protected void ThreadProc()
        {
            while (!source.IsCancellationRequested)
            {

                ThreadQueueItem workItem;
                if (workQueue.TryDequeue(out workItem))
                {
                    workItem.taskStatus = QueueItemStatus.InProgress;
                    workItem.Task();
                    //workItem.Task.Invoke(workItem.Args!);
                    workItem.taskStatus = QueueItemStatus.Complete;
                }

                if (workQueue.IsEmpty)
                    re.Reset();
                re.Wait();
            }
            return;
        }

        public void Stop()
        {
            // Warning: there is no coming back from this.
            source.Cancel();
            re.Set();
        }

        // If you have arguments, use this version
        public ThreadQueueItem EnqueueWork(Action task)
        {
            ThreadQueueItem newItem = new ThreadQueueItem(task);
            workQueue.Enqueue(newItem);
            re.Set();
            return newItem;
        }

        public class ThreadQueueItem
        {
            private Action task;
            public Action Task
            {
                get => task;
            }
            internal QueueItemStatus taskStatus = QueueItemStatus.NotStarted;
            public QueueItemStatus TaskStatus
            {
                get => taskStatus;
            }

            internal ThreadQueueItem(Action method)
            {
                this.task = method;
            }
        }
        public enum QueueItemStatus { NotStarted, InProgress, Complete }
    }
}
