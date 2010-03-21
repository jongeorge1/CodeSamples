namespace TimeoutExample
{
    using System;
    using System.Threading;

    using MbUnit.Framework;

    [TestFixture]
    public class TimeoutAttributeTests
    {
        private class TimeoutAttributeTestClass
        {
            [Timeout(1000)]
            public bool AspectedMethod(int sleepDurationMs)
            {
                Thread.Sleep(sleepDurationMs);
                return true;
            }
        }

        [Test]
        public void AspectedMethodThatCompletesWithinTimeoutDoesNotThrowException()
        {
            // Arrange
            var testClass = new TimeoutAttributeTestClass();
            
            // Act
            var result = testClass.AspectedMethod(500);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public void AspectedMethodThatDoesNotCompleteWithinTimeoutThrowsAnException()
        {
            // Arrange
            var testClass = new TimeoutAttributeTestClass();

            // Act
            var result = testClass.AspectedMethod(2000);

            // Assert handled by the ExpectedException attribute... this should never be hit.
            Assert.Fail();
        }
    }
}