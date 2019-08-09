using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using WPFLocalizeExtension.Engine;

namespace WPFByYourCommand.Behaviors
{
    /// <summary>
    /// For Window and ItemsControl
    /// </summary>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<En attente>")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
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
                {
                    if (!item.Item1.Contains(key))
                    {
                        item.Item1.Add(key);
                    }
                }
            }
        }

        private void Remove(DependencyObject element)
        {
            lock (_lock)
            {
                Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>[] items = list.Where(T => T.Item2.Target == null || T.Item2.Target == element).ToArray();
                foreach (Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>> item in items)
                {
                    list.Remove(item);
                }
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
                    {
                        list.Remove(tuple);
                    }
                    else
                    {
                        tuple.Item3(tuple.Item2.Target);
                    }
                }
            }
        }

        private readonly List<Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>> list = new List<Tuple<List<string>, TypedWeakReference<DependencyObject>, Action<DependencyObject>>>();

        private readonly object _lock = new object();
        private static GlobalizationBehavior Instance { get; } = new GlobalizationBehavior();

        public static void CallUpdate(params string[] keys)
        {
            if (keys.Length == 0)
            {
                keys = new string[] { string.Empty };
            }

            Instance.Execute(keys);
        }

        public static void ChangeLanguage(string language = null)
        {
            if (string.IsNullOrEmpty(language))
            {
                ChangeLanguage(CultureInfo.CurrentUICulture);
            }
            else
            {
                ChangeLanguage(new CultureInfo(language));
            }
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
        public static IReadOnlyDictionary<string, string> Cultures
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

        public static readonly char[] DateTimeStandardFormats = { 'D', 'F', 'G', 'M', 'O', 'R', 'T', 'Y', 'd', 'f', 'g', 's', 't', 'u' };

        /// <summary>Retourne tous les modèles dans lesquels les valeurs date et heure peuvent être mises en forme en utilisant la chaîne de format standard.</summary>
        /// <param name="format">Chaîne de format standard.</param>
        /// <returns>Tableau contenant les modèles standard selon lesquels les valeurs de date et d'heure peuvent être appliquées aux valeurs en utilisant la chaîne de format spécifiée.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="format" /> n'est pas une chaîne de format standard valide.</exception>
        public static string GetDateTimePattern(char format)
        {
            switch (format)
            {
                case 'D':
                    return DateTimeFormatInfo.CurrentInfo.LongDatePattern;
                case 'F':
                case 'U':
                    return string.Concat(DateTimeFormatInfo.CurrentInfo.LongDatePattern, " ", DateTimeFormatInfo.CurrentInfo.LongTimePattern);
                case 'G':
                    return string.Concat(DateTimeFormatInfo.CurrentInfo.ShortDatePattern, " ", DateTimeFormatInfo.CurrentInfo.LongTimePattern);
                case 'M':
                case 'm':
                    return DateTimeFormatInfo.CurrentInfo.MonthDayPattern;
                case 'O':
                case 'o':
                    return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";
                case 'R':
                case 'r':
                    return "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                case 'T':
                    return DateTimeFormatInfo.CurrentInfo.LongTimePattern;
                case 'Y':
                case 'y':
                    return DateTimeFormatInfo.CurrentInfo.YearMonthPattern;
                case 'd':
                    return DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                case 'f':
                    return string.Concat(DateTimeFormatInfo.CurrentInfo.LongDatePattern, " ", DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
                case 'g':
                    return string.Concat(DateTimeFormatInfo.CurrentInfo.ShortDatePattern, " ", DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
                case 's':
                    return "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
                case 't':
                    return DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
                case 'u':
                    return DateTimeFormatInfo.CurrentInfo.UniversalSortableDateTimePattern;
                default:
                    throw new ArgumentException("Format_BadFormatSpecifier", nameof(format));
            }
        }



        public static IEnumerable<Tuple<char, string, string>> DateTimeFormats
        {
            get
            {
                DateTime now = DateTime.Now;
                List<Tuple<char, string, string>> result = new List<Tuple<char, string, string>>();

                string pattern;
                foreach (char format in DateTimeStandardFormats)
                {
                    pattern = GetDateTimePattern(format);
                    result.Add(new Tuple<char, string, string>(format, pattern, now.ToString(pattern, CultureInfo.CurrentCulture)));
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

        private static void OnUpdateWindowChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
            {
                return;
            }

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    void action(DependencyObject w)
                    {
                        (w as Window).Language = XmlLanguage.GetLanguage(LocalizeDictionary.Instance.Culture.IetfLanguageTag);
                    }

                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                    action(element);
                }
                else
                {
                    Instance.Remove(element);
                }
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

        private static void OnUpdateItemsDefaultViewChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
            {
                return;
            }

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    void action(DependencyObject w)
                    {
                        CollectionViewSource.GetDefaultView((w as ItemsControl).Items).Refresh();
                    }

                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                {
                    Instance.Remove(element);
                }
            }
        }

        private static void UpdateBindings(ItemsControl itemsControl)
        {
            foreach (DependencyObject control in itemsControl.Items.OfType<DependencyObject>())
            {
                UpdateBindings(control);
            }
        }

        private static void UpdateBindings(DependencyObject obj)
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

        private static void OnUpdateItemsBindingsChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is ItemsControl element))
            {
                return;
            }

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    void action(DependencyObject w)
                    {
                        UpdateBindings(w as ItemsControl);
                    }

                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                {
                    Instance.Remove(element);
                }
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

        private static void OnUpdateBindingsChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is DependencyObject element))
            {
                return;
            }

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                if (e.NewValue != null)
                {
                    void action(DependencyObject w)
                    {
                        UpdateBindings(w);
                    }

                    Instance.Add(element, action, e.NewValue.ToString().Split(','));
                }
                else
                {
                    Instance.Remove(element);
                }
            }
        }















        public static string GetUpdateChildsBindings(DependencyObject element)
        {
            return (string)element.GetValue(UpdateChildsBindingsProperty);
        }

        public static void SetUpdateChildsBindings(DependencyObject element, string value)
        {
            element.SetValue(UpdateChildsBindingsProperty, value);
        }


        public static readonly DependencyProperty UpdateChildsBindingsProperty =
            DependencyProperty.RegisterAttached(
            "UpdateChildsBindings",
            typeof(string),
            typeof(GlobalizationBehavior),
            new FrameworkPropertyMetadata(null, OnUpdateChildsBindingsChanged));

        private static void OnUpdateChildsBindingsChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(depObj))
            {
                if (e.NewValue != null)
                {
                    void action(DependencyObject w)
                    {
                        int count = VisualTreeHelper.GetChildrenCount(w);
                        DependencyObject child;
                        for (int i = 0; i < count; i++)
                        {
                            child = VisualTreeHelper.GetChild(w, i);
                            UpdateBindings(child);
                        }
                    }

                    Instance.Add(depObj, action, e.NewValue.ToString().Split(','));
                }
                else
                {
                    Instance.Remove(depObj);
                }
            }
        }




    }
}
