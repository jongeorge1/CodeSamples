namespace QueueingExample
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    [Serializable]
    public class MethodInvocationQueueManager
    {
        private const int EmptyQueuePauseInterval = 1000;

        private readonly Queue<QueuedMethodCall> queue = new Queue<QueuedMethodCall>();

        private Timer pauseTimer;

        public MethodInvocationQueueManager()
        {
            this.pauseTimer = new Timer(this.ProcessQueue, null, EmptyQueuePauseInterval, Timeout.Infinite);
        }

        public void Enqueue(object instance, MethodInfo method, object[] arguments)
        {
            this.queue.Enqueue(new QueuedMethodCall(instance, method, arguments));
        }

        private void ProcessQueue(object state)
        {
            while (this.queue.Count > 0)
            {
                var queuedMethodCall = this.queue.Dequeue();

                queuedMethodCall.Method.Invoke(
                    queuedMethodCall.Instance,
                    queuedMethodCall.Arguments);
            }

            this.pauseTimer.Change(EmptyQueuePauseInterval, Timeout.Infinite);
        }
    }
}
