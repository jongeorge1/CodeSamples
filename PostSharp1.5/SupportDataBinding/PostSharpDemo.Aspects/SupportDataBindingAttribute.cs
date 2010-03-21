namespace PostSharpDemo.Aspects
{
    #region Using directives

    using System;
    using System.ComponentModel;
    using System.Reflection;
    using PostSharp.Extensibility;
    using PostSharp.Laos;

    #endregion

    /// <summary>
    /// Custom attribute that, when applied on a type (designated <i>target type</i>), implements the interface
    /// <see cref="INotifyPropertyChanged"/> and raises the <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// event when any property of the target type is modified.
    /// </summary>
    /// <remarks>
    /// Event raising is implemented by appending logic to the <b>set</b> accessor of properties. The 
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> is raised only when accessors successfully complete and the
    /// underlying value is really changed.
    /// </remarks>
    [MulticastAttributeUsage(MulticastTargets.Class | MulticastTargets.Struct)]
    [Serializable()]
    public sealed class SupportDataBindingAttribute : CompoundAspect
    {
        #region "Private Variables"

        [NonSerialized()]
        private int myAspectPriority = 0;

        #endregion

        public int AspectPriority
        {
            get
            {
                return this.myAspectPriority;
            }
            set
            {
                this.myAspectPriority = value;
            }
        }

        /// <summary>
        /// Method called at compile time to get individual aspects required by the current compound
        /// aspect.
        /// </summary>
        /// <param name="targetElement">Metadata element (<see cref="Type"/> in our case) to which
        /// the current custom attribute instance is applied.</param>
        /// <param name="collection">Collection of aspects to which individual aspects should be
        /// added.</param>
        public override void ProvideAspects(object targetElement, LaosReflectionAspectCollection collection)
        {
            // Get the target type.
            Type targetType = (Type) targetElement;
            // On the type, add a Composition aspect to implement the INotifyPropertyChanged interface.
            collection.AddAspect(targetType, new AddNotifyPropertyChangedInterfaceSubAspect());
            // On the type, add a Composition aspect to implement the IEditableObject interface
            //collection.AddAspect(targetType, new AddEditableObjectInterfaceSubAspect());
            // Add a OnMethodBoundaryAspect on each writable non-static property. The implementation of
            // INotifyPropertyChanged.PropertyChanged needs the name of the property (not of the field), so we have to detect
            // changes on the property level, not on the field level. Unfortunately, there is no rule for naming properties and
            // their related fields. Even more, one property could access many fields, or gets its value out of one or
            // more fields. At this point, using an enhancer to add this functionallity is only recomended, as there exixts a design
            // rule, which relates one field to one and only one property and vice versa. Unfortunately, there is no compile time check available.
            // Please be careful!!
            // Personally, I tend to say, that implementing INotifyPropertyChanged is out of the scope of enhancers due to the 
            // possibility to pack logic within the properties implementation, which is never under the enhancers control.
            foreach (PropertyInfo pi in targetType.UnderlyingSystemType.GetProperties())
            {
                if (ReferenceEquals(pi.DeclaringType, targetType) && pi.CanWrite)
                {
                    MethodInfo mi = pi.GetSetMethod();
                    if (!mi.IsStatic)
                    {
                        collection.AddAspect(mi, new OnPropertySetSubAspect(pi.Name, this));
                    }
                }
            }
        }

        #region Nested type: AddNotifyPropertyChangedInterfaceSubAspect

        /// <summary>
        /// Implementation of <see cref="CompositionAspect"/> that adds the <see cref="INotifyPropertyChanged"/>
        /// interface to the type to which it is applied.
        /// </summary>
        [Serializable()]
        internal class AddNotifyPropertyChangedInterfaceSubAspect : CompositionAspect
        {
            /// <summary>
            /// Called at runtime, creates the implementation of the <see cref="INotifyPropertyChanged"/> interface.
            /// </summary>
            /// <param name="eventArgs">Execution context.</param>
            /// <returns>A new instance of <see cref="NotifyPropertyChangedImplementation"/>, which implements
            /// <see cref="INotifyPropertyChanged"/>.</returns>
            public override object CreateImplementationObject(InstanceBoundLaosEventArgs eventArgs)
            {
                return new NotifyPropertyChangedImplementation(eventArgs.Instance);
            }

            /// <summary>
            /// Called at compile-time, gets the interface that should be publicly exposed.
            /// </summary>
            /// <param name="containerType">Type on which the interface will be implemented.</param>
            /// <returns></returns>
            public override Type GetPublicInterface(Type containerType)
            {
                return typeof (INotifyPropertyChanged);
            }

            /// <summary>
            /// Gets weaving options.
            /// </summary>
            /// <returns>Weaving options specifying that the implementation accessor interface (<see cref="IComposed{T}"/>)
            /// should be exposed, and that the implementation of interfaces should be silently ignored if they are
            /// already implemented in the parent types.</returns>
            public override CompositionAspectOptions GetOptions()
            {
                return CompositionAspectOptions.GenerateImplementationAccessor
                       | CompositionAspectOptions.IgnoreIfAlreadyImplemented;
            }
        }

        #endregion

        #region Nested type: NotifyPropertyChangedImplementation

        /// <summary>
        /// Implementation of the <see cref="INotifyPropertyChanged"/> interface.
        /// </summary>
        internal class NotifyPropertyChangedImplementation : INotifyPropertyChanged
        {
            // Instance that exposes the current implementation.
            private readonly object myInstance;

            /// <summary>
            /// Initializes a new <see cref="NotifyPropertyChangedImplementation"/> instance.
            /// </summary>
            /// <param name="instance">Instance that exposes the current implementation.</param>
            public NotifyPropertyChangedImplementation(object instance)
            {
                this.myInstance = instance;
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            /// <summary>
            /// Event raised when a property is changed on the instance that
            /// exposes the current implementation.
            /// </summary>
            //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            //public delegate void PropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e);
            /// <summary>
            /// Raises the <see cref="PropertyChanged"/> event. Called by the
            /// property-level aspect (<see cref="AddNotifyPropertyChangedInterfaceSubAspect"/>)
            /// at the end of property set accessors.
            /// </summary>
            /// <param name="propertyName">Name of the changed property.</param>
            public void OnPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this.myInstance, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        #endregion

        #region Nested type: OnPropertySetSubAspect

        /// <summary>
        /// Implementation of <see cref="OnMethodBoundaryAspect"/> that raises the 
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event when a property set
        /// accessor completes successfully and the value really changes.
        /// </summary>
        [Serializable()]
        internal class OnPropertySetSubAspect : OnMethodBoundaryAspect
        {
            private readonly string myPropertyName;
            private object myOldValue;

            /// <summary>
            /// Initializes a new <see cref="OnPropertySetSubAspect"/>.
            /// </summary>
            /// <param name="propertyName">Name of the property to which this set accessor belong.</param>
            /// <param name="parent">Parent <see cref="NotifyPropertyChangedAttribute"/>.</param>
            public OnPropertySetSubAspect(string propertyName, SupportDataBindingAttribute parent)
            {
                this.AspectPriority = parent.AspectPriority;
                this.myPropertyName = propertyName;
            }

            public override void OnEntry(MethodExecutionEventArgs eventArgs)
            {
                // Construct the name of the properties get-method and backup the value before the set-method is invoked.
                this.myOldValue =
                    eventArgs.Instance.GetType().InvokeMember(
                        eventArgs.Method.Name.Substring(4),
                        BindingFlags.GetProperty,
                        null,
                        eventArgs.Instance,
                        null,
                        null,
                        null,
                        null);
                base.OnEntry(eventArgs);
            }

            /// <summary>
            /// Executed when the set accessor successfully completes. Raises the 
            /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
            /// </summary>
            /// <param name="eventArgs">Event arguments with information about the 
            /// current execution context.</param>
            public override void OnSuccess(MethodExecutionEventArgs eventArgs)
            {
                object newValue;
                newValue =
                    eventArgs.Instance.GetType().InvokeMember(
                        eventArgs.Method.Name.Substring(4),
                        BindingFlags.GetProperty,
                        null,
                        eventArgs.Instance,
                        null,
                        null,
                        null,
                        null);
                // Raises the PropertyChanged event, if necessary. We assume in this sample, that only value types were used.
                if (this.myOldValue != newValue)
                {
                    // Get the implementation of INotifyPropertyChanged. We have access to it through the IComposed interface,
                    // which is implemented at compile time.
                    NotifyPropertyChangedImplementation implementation =
                        (NotifyPropertyChangedImplementation)
                        ((IComposed<INotifyPropertyChanged>) eventArgs.Instance).GetImplementation(
                            eventArgs.InstanceCredentials);
                    implementation.OnPropertyChanged(this.myPropertyName);
                }
            }
        }

        #endregion
    }
}