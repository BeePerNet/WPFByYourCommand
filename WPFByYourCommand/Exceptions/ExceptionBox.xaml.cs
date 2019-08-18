using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WPFByYourCommand.Exceptions
{
    /// <summary>
    /// Interaction logic for ExceptionBox.xaml
    /// </summary>
    [SuppressMessage("Design", "CA1501")]
    public partial class ExceptionBox : Window
    {
        private class Context
        {
            public string Textblock { get; set; }
            public string Textbox { get; set; }
        }

        private ExceptionBox(string textblock, string textbox, string title)
        {
            InitializeComponent();
            if (title != null)
                this.Title = title;
            DataContext = new Context() { Textblock = textblock, Textbox = textbox };
        }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<En attente>")]
        public static void ShowException(Exception ex, string title = null, Window owner = null)
        {
            ExceptionBox.ShowException(TextExceptionFormatter.GetInnerException(ex).Message, new TextExceptionFormatter(ex).Format(), title, owner);
        }

        public static void ShowException(string startText, Exception ex, string title = null, Window owner = null)
        {
            ExceptionBox.ShowException(startText, new TextExceptionFormatter(ex).Format(), title, owner);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<En attente>")]
        private static void InternalShowException(string textblock, string textbox, string title, Window owner)
        {
            try
            {
                ExceptionBox window = new ExceptionBox(textblock, textbox, title);
                if (owner != null)
                {
                    window.Owner = owner;
                }

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.Write(new TextExceptionFormatter(ex).Format());
            }
        }

        public static void ShowException(string textblock, string textbox, string title = null, Window owner = null)
        {
            Dispatcher dispatcher = owner?.Dispatcher;
            if (dispatcher == null && Application.Current != null)
            {
                dispatcher = Application.Current.Dispatcher;
            }
            if (dispatcher.Thread == Thread.CurrentThread)
            {
                InternalShowException(textblock, textbox, title, owner ?? Application.Current?.MainWindow);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                {
                    InternalShowException(textblock, textbox, title, owner ?? Application.Current?.MainWindow);
                }));
            }
        }

    }
}
