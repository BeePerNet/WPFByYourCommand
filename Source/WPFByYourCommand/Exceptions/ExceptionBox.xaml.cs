using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;

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

        private ExceptionBox(string textblock, string textbox)
        {
            InitializeComponent();

            DataContext = new Context() { Textblock = textblock, Textbox = textbox };
        }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<En attente>")]
        public static void ShowException(Exception ex, Window owner = null)
        {
            ExceptionBox.ShowException(TextExceptionFormatter.GetInnerException(ex).Message, new TextExceptionFormatter(ex).Format(), owner);
        }

        public static void ShowException(string startText, Exception ex, Window owner = null)
        {
            ExceptionBox.ShowException(startText, new TextExceptionFormatter(ex).Format(), owner);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<En attente>")]
        private static void InternalShowException(string textblock, string textbox, Window owner = null)
        {
            try
            {
                ExceptionBox window = new ExceptionBox(textblock, textbox)
                {
                    Owner = owner
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Trace.Write(new TextExceptionFormatter(ex).Format());
            }
        }

        public static void ShowException(string textblock, string textbox, Window owner = null)
        {
            if (owner == null && Application.Current != null)
            {
                owner = Application.Current.MainWindow;
            }

            if (owner == null)
            {
                InternalShowException(textblock, textbox);
            }
            else if (owner.Dispatcher.Thread == Thread.CurrentThread)
            {
                InternalShowException(textblock, textbox, owner);
            }
            else
            {
                owner.Dispatcher.BeginInvoke(new Action(() =>
                {
                    InternalShowException(textblock, textbox, owner);
                }));
            }
        }

    }
}
