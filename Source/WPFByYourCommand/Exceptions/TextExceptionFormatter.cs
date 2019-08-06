using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace WPFByYourCommand.Exceptions
{
    /// <summary>
    /// Format an exception in a human readable string format
    /// </summary>
    public class TextExceptionFormatter
    {
        // Fields
        private int innerDepth;
        private readonly Exception exception;
        private NameValueCollection additionalInfo;
        private readonly StringBuilder stringBuilder = new StringBuilder(1024);
        private static readonly ArrayList IgnoredProperties = new ArrayList(new string[] { "Source", "Message", "HelpLink", "InnerException", "StackTrace" });


        public static Exception GetInnerException(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }

        public Exception GetInnerException()
        {
            return GetInnerException(exception);
        }


        // Methods
        public TextExceptionFormatter(Exception exception)
        {
            this.exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public string Format()
        {
            WriteDescription();
            WriteDateTime(DateTime.UtcNow);
            WriteException(exception, null);
            return stringBuilder.ToString();
        }

        private void WriteDescription()
        {
            stringBuilder.AppendLine(exception.GetType().FullName);
        }

        private void WriteDateTime(DateTime utcNow)
        {
            stringBuilder.AppendLine(utcNow.ToLocalTime().ToString("G", DateTimeFormatInfo.InvariantInfo));
        }

        private void Indent()
        {
            for (int i = 0; i < innerDepth; i++)
            {
                stringBuilder.Append("\t");
            }
        }

        private void WriteException(Exception exceptionToFormat, Exception outerException)
        {
            if (outerException != null)
            {
                innerDepth++;
                Indent();
                string innerException = "Inner Exception";
                stringBuilder.AppendLine(innerException);
                WriteException2(exceptionToFormat, outerException);
                innerDepth--;
            }
            else
            {
                WriteException2(exceptionToFormat, outerException);
            }
        }

        private void WriteException2(Exception exceptionToFormat, Exception outerException)
        {
            if (exceptionToFormat == null)
            {
                throw new ArgumentNullException(nameof(exceptionToFormat));
            }
            WriteExceptionType(exceptionToFormat.GetType());
            WriteMessage(exceptionToFormat.Message);
            WriteSource(exceptionToFormat.Source);
            WriteHelpLink(exceptionToFormat.HelpLink);
            WriteReflectionInfo(exceptionToFormat);
            WriteStackTrace(exceptionToFormat.StackTrace);
            if (outerException == null)
            {
                WriteAdditionalInfo(AdditionalInfo);
            }
            Exception innerException = exceptionToFormat.InnerException;
            if (innerException != null)
            {
                WriteException(innerException, exceptionToFormat);
            }
        }

        private void WriteExceptionType(Type exceptionType)
        {
            IndentAndWriteLine("Type: {0}", exceptionType.AssemblyQualifiedName);
        }

        private void WriteMessage(string message)
        {
            IndentAndWriteLine("Message: {0}", message);
        }

        private void WriteSource(string source)
        {
            IndentAndWriteLine("Source: {0}", source);
        }

        private void WriteHelpLink(string helpLink)
        {
            IndentAndWriteLine("HelpLink: {0}", helpLink);
        }

        private void WriteReflectionInfo(Exception exceptionToFormat)
        {
            object propertyAccessFailed;
            if (exceptionToFormat == null)
            {
                throw new ArgumentNullException(nameof(exceptionToFormat));
            }
            Type type = exceptionToFormat.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                if ((info.CanRead && (IgnoredProperties.IndexOf(info.Name) == -1)) && (info.GetIndexParameters().Length == 0))
                {
                    try
                    {
                        propertyAccessFailed = info.GetValue(exceptionToFormat, null);
                    }
                    catch (TargetInvocationException)
                    {
                        propertyAccessFailed = "Property Access Failed";
                    }
                    WritePropertyInfo(info, propertyAccessFailed);
                }
            }
            foreach (FieldInfo info2 in fields)
            {
                try
                {
                    propertyAccessFailed = info2.GetValue(exceptionToFormat);
                }
                catch (TargetInvocationException)
                {
                    propertyAccessFailed = "Field Access Failed";
                }
                WriteFieldInfo(info2, propertyAccessFailed);
            }
        }

        private void WritePropertyInfo(PropertyInfo propertyInfo, object value)
        {
            Indent();
            stringBuilder.Append(propertyInfo.Name);
            stringBuilder.Append(" : ");
            if (value == null)
            {
                stringBuilder.AppendLine("{null}");
            }
            else
            {
                stringBuilder.AppendLine(value.ToString());
            }
        }

        private void WriteFieldInfo(FieldInfo fieldInfo, object value)
        {
            Indent();
            stringBuilder.Append(fieldInfo.Name);
            stringBuilder.Append(" : ");
            if (value == null)
            {
                stringBuilder.AppendLine("{null}");
            }
            else
            {
                stringBuilder.AppendLine(value.ToString());
            }
        }

        private void WriteStackTrace(string stackTrace)
        {
            Indent();
            stringBuilder.Append("StackTrace: ");
            if ((stackTrace == null) || (stackTrace.Length == 0))
            {
                stringBuilder.AppendLine("Stack Trace Unavailable");
            }
            else
            {
                string str2 = stackTrace.Replace("\n", "\n" + new string('\t', innerDepth));
                stringBuilder.AppendLine(str2);
                stringBuilder.AppendLine();
            }
        }

        private void WriteAdditionalInfo(NameValueCollection additionalInformation)
        {
            stringBuilder.AppendLine("Additional Info:");
            stringBuilder.AppendLine();
            foreach (string str in additionalInformation.AllKeys)
            {
                stringBuilder.Append(str);
                stringBuilder.Append(" : ");
                stringBuilder.Append(additionalInformation[str]);
                stringBuilder.Append("\n");
            }
        }

        private void IndentAndWriteLine(string format, params object[] arg)
        {
            Indent();
            stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, format, arg));
        }

        public Exception Exception => exception;

        public NameValueCollection AdditionalInfo
        {
            get
            {
                if (additionalInfo == null)
                {
                    additionalInfo = new NameValueCollection
                    {
                        { "MachineName", GetMachineName() },
                        { "TimeStamp", DateTime.UtcNow.ToString(CultureInfo.CurrentCulture) },
                        { "FullName", Assembly.GetExecutingAssembly().FullName },
                        { "AppDomainName", AppDomain.CurrentDomain.FriendlyName },
                        { "ThreadIdentity", Thread.CurrentPrincipal.Identity.Name },
                        { "WindowsIdentity", GetWindowsIdentity() }
                    };
                }
                return additionalInfo;
            }
        }

        private static string GetMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (SecurityException)
            {
                return "Permission Denied";
            }
        }

        private static string GetWindowsIdentity()
        {
            try
            {
                return WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
                return "Permission Denied";
            }
        }





    }

}
