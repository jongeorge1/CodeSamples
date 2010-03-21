namespace PostSharpDemo.Aspects.Test
{
    #region Using directives

    using System.ComponentModel;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;

    #endregion

    [TestClass]
    public class PersonTest
    {
        [TestMethod]
        public void INotifyPropertyChangedIsImplemented()
        {
            // Arrange
            Person sut = new Person();

            // Act
            INotifyPropertyChanged sutAsNotifyPropertyChanged = sut as INotifyPropertyChanged;

            // Assert
            Assert.IsNotNull(sutAsNotifyPropertyChanged);
        }

        [TestMethod]
        public void PropertyChangedEventIsRaisedWhenAPropertyIsChanged()
        {
            // Arrange
            Person sut = new Person();
            IPropertyChangedEventSubscriber eventSubscriber =
                MockRepository.GenerateMock<IPropertyChangedEventSubscriber>();

            INotifyPropertyChanged sutAsNotifyPropertyChanged = sut as INotifyPropertyChanged;
            sutAsNotifyPropertyChanged.PropertyChanged += eventSubscriber.Handler;

            // Act
            sut.FirstName = "Changed value";

            // Assert
            eventSubscriber.AssertWasCalled(
                e => e.Handler(sut, null),
                e => e.Constraints(
                         new[]
                             {
                                 Is.Equal(sut),
                                 Property.Value("PropertyName", "FirstName"),
                             }));
        }


        [TestMethod]
        public void PropertyChangedEventIsNotRaisedWhenAPropertyIsSetToItsExistingValue()
        {
            // Arrange
            Person sut = new Person();
            string firstName = "First name";
            sut.FirstName = firstName;

            IPropertyChangedEventSubscriber eventSubscriber =
                MockRepository.GenerateMock<IPropertyChangedEventSubscriber>();

            INotifyPropertyChanged sutAsNotifyPropertyChanged = sut as INotifyPropertyChanged;
            sutAsNotifyPropertyChanged.PropertyChanged += eventSubscriber.Handler;

            // Act
            sut.FirstName = firstName;

            // Assert
            eventSubscriber.AssertWasNotCalled(e => e.Handler(sut, null), e => e.IgnoreArguments());
        }
    }

    public interface IPropertyChangedEventSubscriber
    {
        void Handler(object sender, PropertyChangedEventArgs e);
    }
}