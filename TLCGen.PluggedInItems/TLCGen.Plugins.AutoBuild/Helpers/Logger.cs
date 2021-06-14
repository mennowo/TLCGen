using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TLCGen.Plugins.AutoBuild
{
    public static class Logger
    {
        public static Dispatcher MyDispatcher;

        private static ObservableQueue<LogMessage> _ConsoleOutput;
        public static ObservableQueue<LogMessage> ConsoleOutput
        {
            get { return _ConsoleOutput; }
        }

        public static void AddMessage(LogMessage msg)
        {
            MyDispatcher.Invoke(() => ConsoleOutput.Enqueue(msg));
        }
        public static void AddMessage(string msg)
        {
            LogMessage m = new LogMessage(msg);
            MyDispatcher.Invoke(() => ConsoleOutput.Enqueue(m));
        }

        public static void AddErrorMessage(ErrorLogMessage msg)
        {
            MyDispatcher.Invoke(() => ConsoleOutput.Enqueue(msg));
        }
        public static void AddErrorMessage(string msg)
        {
            ErrorLogMessage m = new ErrorLogMessage(msg);
            MyDispatcher.Invoke(() => ConsoleOutput.Enqueue(m));
        }

        public static void AddWarningMessage(WarningLogMessage msg)
        {
            MyDispatcher.Invoke(() => ConsoleOutput.Enqueue(msg));
        }
        public static void AddWarningMessage(string msg)
        {
            WarningLogMessage m = new WarningLogMessage(msg);
            MyDispatcher.Invoke(() => ConsoleOutput.Enqueue(m));
        }

        static Logger()
        {
            _ConsoleOutput = new ObservableQueue<LogMessage>();
        }
    }

    public class LogMessage
    {
        private DateTime _TimeStamp;
        public DateTime TimeStamp
        {
            get { return _TimeStamp; }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
        }

        public LogMessage(string msg)
        {
            _Message = msg;
            _TimeStamp = DateTime.Now;
        }

        public override string ToString()
        {
            return _TimeStamp.ToLongTimeString() + " - " + _Message;
        }
    }

    public class ErrorLogMessage : LogMessage
    {
        public ErrorLogMessage(string msg) : base(msg)
        {
        }
    }

    public class WarningLogMessage : LogMessage
    {
        public WarningLogMessage(string msg) : base(msg)
        {
        }
    }
}
