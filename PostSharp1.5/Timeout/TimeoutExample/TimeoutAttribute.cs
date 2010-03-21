namespace TimeoutExample
{
    using System;

    using PostSharp.Laos;

    /// <summary>
    /// The timeout attribute.
    /// </summary>
    [Serializable]
    public class TimeoutAttribute : OnMethodInvocationAspect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutAttribute"/> class.
        /// </summary>
        /// <param name="timeoutMs">
        /// The timeout period in ms.
        /// </param>
        public TimeoutAttribute(int timeoutMs)
        {
            this.TimeoutMs = timeoutMs;
        }

        /// <summary>
        /// The timeout period in ms.
        /// </summary>
        public int TimeoutMs { get; set; }

        public override void OnInvocation(MethodInvocationEventArgs eventArgs)
        {
            Action proceedAction = eventArgs.Proceed;
            IAsyncResult result = proceedAction.BeginInvoke(
                null, 
                null);

            bool completed = result.AsyncWaitHandle.WaitOne(this.TimeoutMs);

            if (!completed)
            {
                throw new TimeoutException();
            }

            proceedAction.EndInvoke(result);
        }
    }
}