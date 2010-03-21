namespace AsynchronousExample
{
    using System.Threading;

    using MbUnit.Framework;

    [TestFixture]
    public class AsynchronousAttributeTests
    {
        public class AsynchronousAttributeTestClass
        {
            public AsynchronousAttributeTestClass()
            {
                this.IsComplete = false;
            }

            public void UnaspectedMethod()
            {
                Thread.Sleep(2000);
                this.IsComplete = true;
            }

            public bool IsComplete { get; set; }

            [Asynchronous(AsynchronousInvocationOption.BackgroundThread)]
            public void AspectedMethodUsingBackgroundThread()
            {
                Thread.Sleep(2000);
                this.IsComplete = true;
            }

            [Asynchronous(AsynchronousInvocationOption.Delegate)]
            public void AspectedMethodUsingDelegate()
            {
                Thread.Sleep(2000);
                this.IsComplete = true;
            }

            [Asynchronous(AsynchronousInvocationOption.ThreadPool)]
            public void AspectedMethodUsingThreadPool()
            {
                Thread.Sleep(2000);
                this.IsComplete = true;
            }
        }

        [Test]
        public void UnaspectedMethodBlocks()
        {
            // Arrange
            var testClass = new AsynchronousAttributeTestClass();

            // Act
            testClass.UnaspectedMethod();

            // Assert
            Assert.IsTrue(testClass.IsComplete);
        }

        [Test]
        public void AspectedMethodUsingBackgroundThreadDoesNotBlock()
        {
            // Arrange
            var testClass = new AsynchronousAttributeTestClass();

            // Act
            testClass.AspectedMethodUsingBackgroundThread();

            // Assert
            Assert.IsFalse(testClass.IsComplete);

            // Wait for the method to complete and try again
            Thread.Sleep(3000);
            Assert.IsTrue(testClass.IsComplete);
        }

        [Test]
        public void AspectedMethodUsingDelegateDoesNotBlock()
        {
            // Arrange
            var testClass = new AsynchronousAttributeTestClass();

            // Act
            testClass.AspectedMethodUsingDelegate();

            // Assert
            Assert.IsFalse(testClass.IsComplete);

            // Wait for the method to complete and try again
            Thread.Sleep(3000);
            Assert.IsTrue(testClass.IsComplete);
        }

        [Test]
        public void AspectedMethodUsingThreadPoolDoesNotBlock()
        {
            // Arrange
            var testClass = new AsynchronousAttributeTestClass();

            // Act
            testClass.AspectedMethodUsingThreadPool();

            // Assert
            Assert.IsFalse(testClass.IsComplete);

            // Wait for the method to complete and try again
            Thread.Sleep(3000);
            Assert.IsTrue(testClass.IsComplete);
        }
    }
}
