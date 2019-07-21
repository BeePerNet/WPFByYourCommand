using System;
using System.Windows;
using System.Windows.Data;
using WPFByYourCommand.Observables;

namespace WPFByYourCommand.Bindings
{
    public class VisibleBinding : BindingDecoratorBase, IDisposable
    {
        #region Class level variables

        private FrameworkElement TargetObject;
        private DependencyProperty TargetProperty;
        private PropertyChangeNotifier Notifier;

        #endregion

        #region Constructor

        public VisibleBinding()
            : base()
        {
            Mode = BindingMode.OneWay;
        }

        public VisibleBinding(string path)
            : base(path)
        {
            Mode = BindingMode.OneWay;
        }

        private VisibleBinding(Binding binding, FrameworkElement targetObject, DependencyProperty targetProperty)
            : base(binding)
        {
            Mode = BindingMode.OneWay;

            Init(targetObject, targetProperty);
        }

        #endregion

        #region Methods

        public static void SetVisibleBinding(Binding binding, FrameworkElement targetObject, DependencyProperty targetProperty)
        {
            new VisibleBinding(binding, targetObject, targetProperty);
        }

        private void AttachTargetObject()
        {
            SetVisibleBinding(TargetObject, this);
        }

        private void DetachTargetObject()
        {
            SetVisibleBinding(TargetObject, null);
        }

        private void AttachTargetProperty()
        {

        }

        private void DetachTargetProperty()
        {

        }


        private void NotifierValueChanged(object sender, EventArgs e)
        {
            CheckBindings();
        }

        private void CheckBindings()
        {
            if (Notifier != null && Notifier.Value is bool)
            {
                if ((bool)Notifier.Value)
                {
                    SetBinding();
                }
                else
                {
                    ClearBinding();
                }
            }
        }

        private void ClearBinding()
        {
            if (TargetObject != null && TargetProperty != null)
            {
                TargetObject.SetValue(TargetProperty, null);
            }
        }

        private void SetBinding()
        {
            if (TargetObject != null && TargetProperty != null)
            {
                TargetObject.SetBinding(TargetProperty, this.Binding);
            }
        }

        private void Init(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            if (targetObject is FrameworkElement)
            {
                var element = targetObject as FrameworkElement;
                if (TargetObject == null)
                {
                    if (TargetObject != null)
                    {
                        DetachTargetObject();
                    }
                    TargetObject = element;
                    if (TargetObject != null)
                    {
                        AttachTargetObject();
                    }
                    if (TargetProperty != null)
                    {
                        DetachTargetProperty();
                    }
                    TargetProperty = targetProperty;
                    if (TargetProperty != null)
                    {
                        AttachTargetProperty();
                    }
                    if (Notifier != null)
                    {
                        Notifier.ValueChanged -= NotifierValueChanged;
                    }
                    Notifier = new PropertyChangeNotifier(element, "IsEnabled");
                    Notifier.ValueChanged += NotifierValueChanged;
                    CheckBindings();
                }
            }
        }

        #endregion

        #region Overrides

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //delegate binding creation etc. to the base class 
            var expression = base.ProvideValue(serviceProvider);

            DependencyObject targetObject = null;
            DependencyProperty targetProperty = null;

            if (serviceProvider != null)
            {
                TryGetTargetItems(serviceProvider, out targetObject, out targetProperty);
            }
            Init(targetObject, targetProperty);
            return expression;
        }

        #endregion

        #region DependencyProperties

        #region VisibleBinding

        /// <summary> 
        /// Gets the value of the VisibleBinding attached property for a specified UIElement. 
        /// </summary> 
        /// <param name="element">The UIElement from which the property value is read.</param> 
        /// <returns>The VisibleBinding property value for the UIElement.</returns> 
        public static VisibleBinding GetVisibleBinding(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (VisibleBinding)element.GetValue(VisibleBindingProperty);
        }

        /// <summary> 
        /// Sets the value of the VisibleBinding attached property to a specified UIElement. 
        /// </summary> 
        /// <param name="element">The UIElement to which the attached property is written.</param> 
        /// <param name="value">The needed VisibleBinding value.</param> 
        public static void SetVisibleBinding(UIElement element, VisibleBinding value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(VisibleBindingProperty, value);
        }

        /// <summary> 
        /// Identifies the VisibleBinding dependency property. 
        /// </summary> 
        public static readonly DependencyProperty VisibleBindingProperty =
            DependencyProperty.RegisterAttached(
                "VisibleBinding",
                typeof(VisibleBinding),
                typeof(VisibleBinding),
                new PropertyMetadata(null, OnVisibleBindingPropertyChanged));

        /// <summary> 
        /// VisibleBindingProperty property changed handler. 
        /// </summary> 
        /// <param name="d">VisibleBinding that changed its VisibleBinding.</param> 
        /// <param name="e">Event arguments.</param> 
        private static void OnVisibleBindingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //TODO:Problème ici à vérifier
            if (sender is FrameworkElement control)
            {
            }
        }

        #endregion

        #endregion




        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if (this.Notifier != null)
                    this.Notifier.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
