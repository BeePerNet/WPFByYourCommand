using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace WPFByYourCommand.Observables
{

    public sealed class PropertyChangeNotifier : DependencyObject, IDisposable
    {
        #region Class level variables

        private readonly WeakReference mPropertySource;

        #endregion

        #region Constructors

        public PropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
                throw new ArgumentNullException(nameof(propertySource));
            this.mPropertySource = new WeakReference(propertySource);
            Binding binding = new Binding
            {
                Path = property ?? throw new ArgumentNullException(nameof(property)),
                Mode = BindingMode.OneWay,
                Source = propertySource
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        #endregion

        #region PropertySource

        public DependencyObject PropertySource
        {
            get
            {
                return this.mPropertySource != null && this.mPropertySource.IsAlive
                ? this.mPropertySource.Target as DependencyObject
                : null;
            }
        }

        #endregion

        #region Value

        /// <summary> 
        /// Identifies the <see cref="Value"/> dependency property 
        /// </summary> 
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(PropertyChangeNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyChangeNotifier notifier = d as PropertyChangeNotifier;
            notifier.ValueChanged?.Invoke(notifier, EventArgs.Empty);
        }

        /// <summary> 
        /// Returns/sets the value of the property 
        /// </summary> 
        /// <seealso cref="ValueProperty"/> 
        [Description("Returns/sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
#pragma warning disable CA1721 // Property names should not match get methods
        public object Value
#pragma warning restore CA1721 // Property names should not match get methods
        {
            get
            {
                return (object)this.GetValue(PropertyChangeNotifier.ValueProperty);
            }
            set
            {
                this.SetValue(PropertyChangeNotifier.ValueProperty, value);
            }
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
        }

        #endregion
    }
}