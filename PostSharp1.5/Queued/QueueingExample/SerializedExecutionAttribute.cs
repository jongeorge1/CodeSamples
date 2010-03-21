namespace QueueingExample
{
    using System;

    using PostSharp.Laos;

    [Serializable]
    public class SerializedExecutionAttribute : OnMethodInvocationAspect
    {
        private MethodInvocationQueueManager queueManager;

        public override void RuntimeInitialize(System.Reflection.MethodBase method)
        {
            this.queueManager = new MethodInvocationQueueManager();
        }

        public override void OnInvocation(MethodInvocationEventArgs eventArgs)
        {
            this.queueManager.Enqueue(eventArgs.Instance, eventArgs.Method, eventArgs.GetArgumentArray());
        }
    }
}
