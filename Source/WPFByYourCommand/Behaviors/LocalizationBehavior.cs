using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using WPFLocalizeExtension.Engine;

namespace WPFByYourCommand.Behaviors
{
    /// <summary>
    /// For Window and ItemsControl
    /// </summary>
    public class LocalizationBehavior
    {
        private LocalizationBehavior()
        {
            LocalizeDictionary.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LocalizeDictionary.Culture))
            {
                lock (Instance._lock)
                {
                    foreach (TypedWeakReference<Window> tWindow in list.ToArray())
                    {
                        Window window = tWindow.Target;
                        if (window == null)
                        {
                            list.Remove(tWindow);
                        }
                        else
                        {
                            window.Language = XmlLanguage.GetLanguage(LocalizeDictionary.Instance.Culture.IetfLanguageTag);
                        }
                    }
                }
            }
        }

        private List<TypedWeakReference<Window>> list = new List<TypedWeakReference<Window>>();

        private object _lock = new object();

        private static LocalizationBehavior Instance { get; } = new LocalizationBehavior();


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




        public static bool GetUpdateWindowLanguage(Window element)
        {
            return (bool)element.GetValue(UpdateWindowLanguageProperty);
        }

        public static void SetUpdateWindowLanguage(Window element, bool value)
        {
            element.SetValue(UpdateWindowLanguageProperty, value);
        }


        public static readonly DependencyProperty UpdateWindowLanguageProperty =
            DependencyProperty.RegisterAttached(
            "UpdateWindowLanguage",
            typeof(bool),
            typeof(LocalizationBehavior),
            new FrameworkPropertyMetadata(false, OnUpdateWindowLanguagesChanged));

        static void OnUpdateWindowLanguagesChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is Window element))
                return;

            if (!DesignerProperties.GetIsInDesignMode(element))
            {
                lock (Instance._lock)
                {
                    if ((bool)e.NewValue)
                    {
                        Instance.list.Add(new TypedWeakReference<Window>(element));
                    }
                    else
                    {
                        TypedWeakReference<Window>[] wrefs = Instance.list.Where(T => T.Target == element || T.Target == null).ToArray();
                        foreach (TypedWeakReference<Window> wref in Instance.list)
                            Instance.list.Remove(wref);
                    }
                }
            }
        }

    }
}
