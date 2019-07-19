using System;
using System.Windows;
using System.Windows.Data;
using WPFByYourCommand.Observables;

namespace WPFByYourCommand.Bindings
{
    public class VisibleBinding : BindingDecoratorBase
    {
        #region Class level variables

        private FrameworkElement mTargetObject;
        private DependencyProperty mTargetProperty;
        private PropertyChangeNotifier mNotifier;

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
            SetVisibleBinding(mTargetObject, this);
        }

        private void DetachTargetObject()
        {
            SetVisibleBinding(mTargetObject, null);
        }

        private void AttachTargetProperty()
        {

        }

        private void DetachTargetProperty()
        {

        }

        private void AttachExpression()
        {

        }

        private void DetachExpression()
        {

        }

        private void mNotifier_ValueChanged(object sender, EventArgs e)
        {
            CheckBindings();
        }

        private void CheckBindings()
        {
            if (mNotifier != null && mNotifier.Value is bool)
            {
                if ((bool)mNotifier.Value)
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
            if (mTargetObject != null && mTargetProperty != null)
            {
                mTargetObject.SetValue(mTargetProperty, null);
            }
        }

        private void SetBinding()
        {
            if (mTargetObject != null && mTargetProperty != null)
            {
                mTargetObject.SetBinding(mTargetProperty, this.Binding);
            }
        }

        private void Init(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            if (targetObject is FrameworkElement)
            {
                var element = targetObject as FrameworkElement;
                if (mTargetObject == null)
                {
                    if (mTargetObject != null)
                    {
                        DetachTargetObject();
                    }
                    mTargetObject = element;
                    if (mTargetObject != null)
                    {
                        AttachTargetObject();
                    }
                    if (mTargetProperty != null)
                    {
                        DetachTargetProperty();
                    }
                    mTargetProperty = targetProperty;
                    if (mTargetProperty != null)
                    {
                        AttachTargetProperty();
                    }
                    if (mNotifier != null)
                    {
                        mNotifier.ValueChanged -= mNotifier_ValueChanged;
                    }
                    mNotifier = new PropertyChangeNotifier(element, "IsEnabled");
                    mNotifier.ValueChanged += mNotifier_ValueChanged;
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
            var control = sender as FrameworkElement;
            if (control != null)
            {
            }
        }

        #endregion

        #endregion 
    }
}
