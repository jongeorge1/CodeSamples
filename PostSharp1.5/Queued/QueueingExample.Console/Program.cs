using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueueingExample.Console
{
    using System.Threading;

    using Console=System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            // Fire off a load of calls to "DoSomeStuff" on several different threads
            ThreadPool.QueueUserWorkItem(CallDoSomeStuffTenTimes);
            ThreadPool.QueueUserWorkItem(CallDoSomeStuffTenTimes);
            ThreadPool.QueueUserWorkItem(CallDoSomeStuffTenTimes);

            // Should result in DoSomeStuff being called 30 times in quick succession... because the code is currently
            // a bit crappy, we need to put a wait in to allow all executions to finish before terminating the app. There's
            // probably a better way to do this.
            Thread.Sleep(TimeSpan.FromSeconds(35));
        }

        [SerializedExecution]
        private static void DoSomeStuff()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.WriteLine(DateTime.Now.ToLongTimeString());
        }

        private static void CallDoSomeStuffTenTimes(object state)
        {
            for (int i = 0; i < 10; i++)
            {
                DoSomeStuff();
            }
        }
    }
}
