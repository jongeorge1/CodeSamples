namespace AsynchronousExample
{
    using System;
    using System.Threading;

    using PostSharp.Laos;

    /// <summary>
    /// Makes the specified method asynchronous. Should only be applied to void methods.
    /// </summary>
    [Serializable]
    public class AsynchronousAttribute : OnMethodInvocationAspect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousAttribute"/> class.
        /// </summary>
        public AsynchronousAttribute()
            : this(AsynchronousInvocationOption.ThreadPool)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousAttribute"/> class.
        /// </summary>
        /// <param name="invocationOption">
        /// The invocation option.
        /// </param>
        public AsynchronousAttribute(AsynchronousInvocationOption invocationOption)
        {
            this.InvocationOption = invocationOption;
        }

        /// <summary>
        /// Gets or sets the InvocationOption.
        /// </summary>
        public AsynchronousInvocationOption InvocationOption { get; set; }

        public override void OnInvocation(MethodInvocationEventArgs eventArgs)
        {
            switch (this.InvocationOption)
            {
                case AsynchronousInvocationOption.BackgroundThread:
                    this.InvokeUsingBackgroundThread(eventArgs);
                    break;

                case AsynchronousInvocationOption.Delegate:
                    this.InvokeUsingDelegate(eventArgs);
                    break;

                default:
                    this.InvokeUsingThreadPool(eventArgs);
                    break;
            }
        }

        private void InvokeUsingBackgroundThread(MethodInvocationEventArgs eventArgs)
        {
            var thread = new Thread(eventArgs.Proceed) 
                {
                   IsBackground = true
                };

            thread.Start();
        }

        private void InvokeUsingDelegate(MethodInvocationEventArgs eventArgs)
        {
            var proceed = new Action(eventArgs.Proceed);
            proceed.BeginInvoke(proceed.EndInvoke, proceed);
        }

        private void InvokeUsingThreadPool(MethodInvocationEventArgs eventArgs)
        {
            ThreadPool.QueueUserWorkItem(delegate { eventArgs.Proceed(); });
        }
    }
}