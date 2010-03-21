namespace AsynchronousExample
{
    /// <summary>
    /// The asynchronous invocation options.
    /// </summary>
    public enum AsynchronousInvocationOption
    {
        /// <summary>
        /// Executes the method by creating a delegate for it and calling BeginInvoke with no callback.
        /// </summary>
        Delegate, 

        /// <summary>
        /// Executes the method using ThreadPool.QueueUserWorkItem
        /// </summary>
        ThreadPool, 

        /// <summary>
        /// Executes the method using a newly created thread with IsBackground set to true.
        /// </summary>
        BackgroundThread
    }
}