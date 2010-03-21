namespace QueueingExample
{
    using System;
    using System.Reflection;

    [Serializable]
    public class QueuedMethodCall
    {
        public object Instance { get; set; }

        public MethodInfo Method { get; set; }

        public object[] Arguments { get; set; }

        public QueuedMethodCall(object instance, MethodInfo method, object[] arguments)
        {
            this.Instance = instance;
            this.Method = method;
            this.Arguments = arguments;
        }
    }
}