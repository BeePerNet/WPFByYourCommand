using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using WPFLocalizeExtension.Engine;

namespace WPFByYourCommand.Behaviors
{
    /// <summary>
    /// For Window and ItemsControl
    /// </summary>
    public class GlobalizationBehavior
    {
        private GlobalizationBehavior()
        {
            LocalizeDictionary.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LocalizeDictionary.Culture))
            {
                Instance.Execute(string.Empty);
            }
        }

        private void Add(DependencyObject element, Action<DependencyObject> action, params string[] keys)
        {
            keys = keys.Select(T => T.ToUpperInvariant()).ToArray();
            lock (_lock)
            {
                Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>> item = list.SingleOrDefault(T => T.Item2.Target == element);
                if (item == null)
                {
                    item = new Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>(new List<string>(), new TypedWeakReference<DependencyObject>(element), action);
                    list.Add(item);
                }
                foreach (string key in keys)
                    if (!item.Item1.Contains(key))
                        item.Item1.Add(key);
            }
        }

        private void Remove(DependencyObject element)
        {
            lock (_lock)
            {
                Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>[] items = list.Where(T => T.Item2.Target == null || T.Item2.Target == element).ToArray();
                foreach (Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>> item in items)
                    list.Remove(item);
            }
        }


        private void Execute(params string[] keys)
        {
            keys = keys.Select(T => T.ToUpperInvariant()).ToArray();
            lock (_lock)
            {
                foreach (Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>> tuple in list.Where(T => T.Item1.Any(i => keys.Any(k => i == k))).ToArray())
                {
                    if (tuple.Item2.Target == null)
                        list.Remove(tuple);
                    else
                        tuple.Item3(tuple.Item2.Target);
                }
            }
        }

        private List<Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>> list = new List<Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>>();

        private object _lock = new object();
        private static GlobalizationBehavior Instance { get; } = new GlobalizationBehavior();

        public static void CallUpdate(params string[] keys)
        {
            if (keys.Length == 0)
                keys = new string[] { string.Empty };
            Instance.Execute(keys);
        }

        public static void ChangeLanguage(string language = null)
        {
            if (string.IsNullOrEmpty(language))
                ChangeLanguage(CultureInfo.CurrentUICulture);
            else
                ChangeLanguage(new CultureInfo(language));
        }

        public static void ChangeLanguage(CultureInfo cultureInfo)
        {
            LocalizeDictionary.Instance.Culture = cultureInfo;
        }


        /// <summary>
        /// List of specific cultures.
        /// It seem that xaml cannot parse a static function with parameters.
        /// To delete if howto was found and xaml fixed.
        /// </summary>
        /// <returns>List of specific cultures</returns>
        public static Dictionary<string, string> Cultures
        {
            get
            {
                Dictionary<string, string> result = CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(T => T.NativeName).ToDictionary(T => T.Name, T => T.NativeName);
                string[] languagelist = Directory.GetDirectories(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                List<string> installedlanguages = languagelist.Select(T => Path.GetFileName(T)).ToList();
                installedlanguages.Add("en");
                foreach (string language in installedlanguages)
                {
                    foreach (KeyValuePair<string, string> pair in result.Where(T => T.Key.StartsWith(language, StringComparison.OrdinalIgnoreCase)).ToList())
                    {
                        result[pair.Key] = string.Concat(pair.Value, " *");
                    }
                }
                return result;
            }
        }







        public static string GetUpdateWindow(Window element)
        {
            return (string)element.GetValue(UpdateWindowProperty);
        }

        public static void SetUpdateWindow(Window element, string value)
        {
            element.SetValue(UpdateWindowProperty, value);
        }


        public static readonly DependencyProperty UpdateWindowProperty =
            DependencyProperty.RegisterAttached(
            "UpdateWindow",
            typeof(string),
            typeof(GlobalizationBehavior),
            new FrameworkPropertyMetadata(null, OnUpdateWindowChanged));

        static void OnUpdateWindowChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
                return;

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    Action<DependencyObject> action = (w) => (w as Window).Language = XmlLanguage.GetLanguage(LocalizeDictionary.Instance.Culture.IetfLanguageTag);
                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                    action(element);
                }
                else
                    Instance.Remove(element);
            }
        }






        public static string GetUpdateItemsDefaultView(ItemsControl element)
        {
            return (string)element.GetValue(UpdateItemsDefaultViewProperty);
        }

        public static void SetUpdateItemsDefaultView(ItemsControl element, string value)
        {
            element.SetValue(UpdateItemsDefaultViewProperty, value);
        }


        public static readonly DependencyProperty UpdateItemsDefaultViewProperty =
            DependencyProperty.RegisterAttached(
            "UpdateItemsDefaultView",
            typeof(string),
            typeof(GlobalizationBehavior),
            new FrameworkPropertyMetadata(null, OnUpdateItemsDefaultViewChanged));

        static void OnUpdateItemsDefaultViewChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
                return;

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    Action<DependencyObject> action = (w) => CollectionViewSource.GetDefaultView((w as ItemsControl).Items).Refresh();
                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                    Instance.Remove(element);
            }
        }




        static void UpdateBindings(ItemsControl itemsControl)
        {
            foreach(DependencyObject control in itemsControl.Items.OfType<DependencyObject>())
            {
                UpdateBindings(control);
            }
        }


        static void UpdateBindings(DependencyObject obj)
        {
            LocalValueEnumerator lve = obj.GetLocalValueEnumerator();
            while (lve.MoveNext())
            {
                LocalValueEntry entry = lve.Current;

                if (BindingOperations.IsDataBound(obj, entry.Property))
                {
                    (entry.Value as BindingExpression).UpdateTarget();
                }
            }
            if (obj is ContentControl contentControl && contentControl.Content is DependencyObject dependencyObject)
            {
                UpdateBindings(dependencyObject);
            }
        }




        public static string GetUpdateItemsBindings(ItemsControl element)
        {
            return (string)element.GetValue(UpdateItemsBindingsProperty);
        }

        public static void SetUpdateItemsBindings(ItemsControl element, string value)
        {
            element.SetValue(UpdateItemsBindingsProperty, value);
        }


        public static readonly DependencyProperty UpdateItemsBindingsProperty =
            DependencyProperty.RegisterAttached(
            "UpdateItemsBindings",
            typeof(string),
            typeof(GlobalizationBehavior),
            new FrameworkPropertyMetadata(null, OnUpdateItemsBindingsChanged));


        static void OnUpdateItemsBindingsChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is ItemsControl element))
                return;

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    Action<DependencyObject> action = (w) => UpdateBindings(w as ItemsControl);
                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                    Instance.Remove(element);
            }
        }











        public static string GetUpdateBindings(DependencyObject element)
        {
            return (string)element.GetValue(UpdateBindingsProperty);
        }

        public static void SetUpdateBindings(DependencyObject element, string value)
        {
            element.SetValue(UpdateBindingsProperty, value);
        }


        public static readonly DependencyProperty UpdateBindingsProperty =
            DependencyProperty.RegisterAttached(
            "UpdateBindings",
            typeof(string),
            typeof(GlobalizationBehavior),
            new FrameworkPropertyMetadata(null, OnUpdateBindingsChanged));


        static void OnUpdateBindingsChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is DependencyObject element))
                return;

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    Action<DependencyObject> action = (w) => UpdateBindings(w);
                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                    Instance.Remove(element);
            }
        }

    }
}
