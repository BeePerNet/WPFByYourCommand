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

        private void Add(UIElement element, Action<UIElement> action, params string[] keys)
        {
            keys = keys.Select(T => T.ToUpperInvariant()).ToArray();
            lock (_lock)
            {
                Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>> item = list.SingleOrDefault(T => T.Item2.Target == element);
                if (item == null)
                {
                    item = new Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>>(new List<string>(), new TypedWeakReference<UIElement>(element), action);
                    list.Add(item);
                }
                foreach (string key in keys)
                    if (!item.Item1.Contains(key))
                        item.Item1.Add(key);
            }
        }

        private void Remove(UIElement element)
        {
            lock (_lock)
            {
                Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>>[] items = list.Where(T => T.Item2.Target == null || T.Item2.Target == element).ToArray();
                foreach (Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>> item in items)
                    list.Remove(item);
            }
        }


        private void Execute(params string[] keys)
        {
            keys = keys.Select(T => T.ToUpperInvariant()).ToArray();
            lock (_lock)
            {
                foreach (Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>> tuple in list.Where(T => T.Item1.Any(i => keys.Any(k => i == k))).ToArray())
                {
                    if (tuple.Item2.Target == null)
                        list.Remove(tuple);
                    else
                        tuple.Item3(tuple.Item2.Target);
                }
            }
        }

        private List<Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>>> list = new List<Tuple<List<string>, TypedWeakReference<UIElement>, Action<UIElement>>>();

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



        public static string GetUpdateGlobalization(UIElement element)
        {
            return (string)element.GetValue(UpdateGlobalizationProperty);
        }

        public static void SetUpdateGlobalization(UIElement element, string value)
        {
            element.SetValue(UpdateGlobalizationProperty, value);
        }


        public static readonly DependencyProperty UpdateGlobalizationProperty =
            DependencyProperty.RegisterAttached(
            "UpdateGlobalization",
            typeof(string),
            typeof(GlobalizationBehavior),
            new FrameworkPropertyMetadata(null, OnUpdateGlobalizationChanged));

        static void OnUpdateGlobalizationChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
                return;

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    Action<UIElement> action = GetAction(element);
                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                    Instance.Remove(element);
            }
        }

        private static Action<UIElement> GetAction(UIElement element)
        {
            if (element is Window window)
            {
                Action<UIElement> action = (w) => (w as Window).Language = XmlLanguage.GetLanguage(LocalizeDictionary.Instance.Culture.IetfLanguageTag);
                action(element);
                return action;
            }
            else if (element is ItemsControl itemsControl)
            {
                return (w) => CollectionViewSource.GetDefaultView((w as ItemsControl).Items).Refresh();
            }
            else
                throw new ArgumentOutOfRangeException(nameof(element), "Type not supported");
        }




    }
}
