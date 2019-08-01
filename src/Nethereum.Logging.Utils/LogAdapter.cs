using System;
using Common.Logging;
using Microsoft.Extensions.Logging;
using cl = Common.Logging;
using ms = Microsoft.Extensions.Logging;

namespace Nethereum.Microsoft.Logging.Utils
{
    /// <summary>
    /// Transates from Microsoft.Extensions.Logging.ILogger to Common.Logging.ILog
    /// </summary>
    public class LogAdapter : ILog
    {
        private readonly ms.ILogger _logger;
        private readonly ILogPreformatter _preformatter;

        public LogAdapter(ms.ILogger logger, ILogPreformatter preformatter = null)
        {
            _preformatter = preformatter ?? new LogPreformatter();
            _logger = logger;
        }

        public bool IsDebugEnabled => _logger.IsEnabled(cl.LogLevel.Debug.ToMsLogLevel());

        public bool IsErrorEnabled => _logger.IsEnabled(cl.LogLevel.Error.ToMsLogLevel());

        public bool IsFatalEnabled => _logger.IsEnabled(cl.LogLevel.Fatal.ToMsLogLevel());

        public bool IsInfoEnabled => _logger.IsEnabled(cl.LogLevel.Info.ToMsLogLevel());

        public bool IsTraceEnabled => _logger.IsEnabled(cl.LogLevel.Trace.ToMsLogLevel());

        public bool IsWarnEnabled => _logger.IsEnabled(cl.LogLevel.Warn.ToMsLogLevel());

        public virtual void Trace(object message) => Write(ms.LogLevel.Trace, message);

        public virtual void Trace(object message, Exception exception)
        {
            if (!IsTraceEnabled)
                return;

            Write(ms.LogLevel.Trace, exception, message);
        }

        public virtual void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsTraceEnabled)
                return;

            WriteFormat(ms.LogLevel.Trace, formatProvider, format, args);
        }

        public virtual void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            if (!IsTraceEnabled)
                return;

            WriteFormat(ms.LogLevel.Trace, exception, formatProvider, format, args);
        }

        public virtual void TraceFormat(string format, params object[] args)
        {
            if (!IsTraceEnabled)
                return;

            WriteFormat(ms.LogLevel.Trace, format, args);
        }

        public virtual void TraceFormat(string format, Exception exception, params object[] args)
        {
            if (!IsTraceEnabled)
                return;

            WriteFormat(ms.LogLevel.Trace, exception, format, args);
        }

        public virtual void Trace(Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsTraceEnabled)
                return;

            WriteCallback(ms.LogLevel.Trace, formatMessageCallback);
        }

        public virtual void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsTraceEnabled)
                return;

            WriteCallback(ms.LogLevel.Trace, formatMessageCallback, exception);
        }

        public virtual void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsTraceEnabled)
                return;

            WriteCallback(ms.LogLevel.Trace, formatProvider, formatMessageCallback);
        }

        public virtual void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsTraceEnabled)
                return;

            WriteCallback(ms.LogLevel.Trace, formatProvider, formatMessageCallback, exception);
        }

        public virtual void Debug(object message)
        {
            if (!IsDebugEnabled)
                return;

            Write(ms.LogLevel.Debug, message);
        }

        public virtual void Debug(object message, Exception exception)
        {
            if (!IsDebugEnabled)
                return;

            Write(ms.LogLevel.Debug, exception, message);
        }

        public virtual void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;

            WriteFormat(ms.LogLevel.Debug, formatProvider, format, args);
        }

        public virtual void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            if (!IsDebugEnabled)
                return;

            WriteFormat(ms.LogLevel.Debug, exception, formatProvider, format, args);
        }

        public virtual void DebugFormat(string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;

            WriteFormat(ms.LogLevel.Debug, format, args);
        }

        public virtual void DebugFormat(string format, Exception exception, params object[] args)
        {
            if (!IsDebugEnabled)
                return;

            WriteFormat(ms.LogLevel.Debug, exception, format, args);
        }

        public virtual void Debug(Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsDebugEnabled)
                return;

            WriteCallback(ms.LogLevel.Debug, formatMessageCallback);
        }

        public virtual void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsDebugEnabled)
                return;

            WriteCallback(ms.LogLevel.Debug, formatMessageCallback, exception);
        }

        public virtual void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsDebugEnabled)
                return;

            WriteCallback(ms.LogLevel.Debug, formatProvider, formatMessageCallback);
        }

        public virtual void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsDebugEnabled)
                return;

            WriteCallback(ms.LogLevel.Debug, formatProvider, formatMessageCallback, exception);
        }

        public virtual void Info(object message)
        {
            if (!IsInfoEnabled)
                return;

            Write(ms.LogLevel.Information, message);
        }

        public virtual void Info(object message, Exception exception)
        {
            if (!IsInfoEnabled)
                return;

            Write(ms.LogLevel.Information, exception, message);
        }

        public virtual void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;

            WriteFormat(ms.LogLevel.Information, formatProvider, format, args);
        }

        public virtual void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            if (!IsInfoEnabled)
                return;

            WriteFormat(ms.LogLevel.Information, exception, formatProvider, format, args);
        }

        public virtual void InfoFormat(string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;

            WriteFormat(ms.LogLevel.Information, format, args);
        }

        public virtual void InfoFormat(string format, Exception exception, params object[] args)
        {
            if (!IsInfoEnabled)
                return;

            WriteFormat(ms.LogLevel.Information, exception, format, args);
        }

        public virtual void Info(Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsInfoEnabled)
                return;

            WriteCallback(ms.LogLevel.Information, formatMessageCallback);
        }

        public virtual void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsInfoEnabled)
                return;

            WriteCallback(ms.LogLevel.Information, formatMessageCallback, exception);
        }

        public virtual void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsInfoEnabled)
                return;

            WriteCallback(ms.LogLevel.Information, formatProvider, formatMessageCallback);
        }

        public virtual void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsInfoEnabled)
                return;

            WriteCallback(ms.LogLevel.Information, formatProvider, formatMessageCallback, exception);
        }

        public virtual void Warn(object message)
        {
            if (!IsWarnEnabled)
                return;

            Write(ms.LogLevel.Warning, message);
        }

        public virtual void Warn(object message, Exception exception)
        {
            if (!IsWarnEnabled)
                return;

            Write(ms.LogLevel.Warning, exception, message);
        }

        public virtual void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;

            WriteFormat(ms.LogLevel.Warning, formatProvider, format, args);
        }

        public virtual void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            if (!IsWarnEnabled)
                return;

            WriteFormat(ms.LogLevel.Warning, exception, formatProvider, format, args);
        }

        public virtual void WarnFormat(string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;

            WriteFormat(ms.LogLevel.Warning, format, args);
        }

        public virtual void WarnFormat(string format, Exception exception, params object[] args)
        {
            if (!IsWarnEnabled)
                return;

            WriteFormat(ms.LogLevel.Warning, exception, format, args);
        }

        public virtual void Warn(Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsWarnEnabled)
                return;

            WriteCallback(ms.LogLevel.Warning, formatMessageCallback);
        }

        public virtual void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsWarnEnabled)
                return;

            WriteCallback(ms.LogLevel.Warning, formatMessageCallback, exception);
        }

        public virtual void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsWarnEnabled)
                return;

            WriteCallback(ms.LogLevel.Warning, formatProvider, formatMessageCallback);
        }

        public virtual void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsWarnEnabled)
                return;

            WriteCallback(ms.LogLevel.Warning, formatProvider, formatMessageCallback, exception);
        }

        public virtual void Error(object message)
        {
            if (!IsErrorEnabled)
                return;

            Write(ms.LogLevel.Error, message);
        }

        public virtual void Error(object message, Exception exception)
        {
            if (!IsErrorEnabled)
                return;

            Write(ms.LogLevel.Error, exception, message);
        }

        public virtual void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;

            WriteFormat(ms.LogLevel.Error, formatProvider, format, args);
        }

        public virtual void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            if (!IsErrorEnabled)
                return;

            WriteFormat(ms.LogLevel.Error, exception, formatProvider, format, args);
        }

        public virtual void ErrorFormat(string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;

            WriteFormat(ms.LogLevel.Error, format, args);
        }

        public virtual void ErrorFormat(string format, Exception exception, params object[] args)
        {
            if (!IsErrorEnabled)
                return;

            WriteFormat(ms.LogLevel.Error, exception, format, args);
        }

        public virtual void Error(Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsErrorEnabled)
                return;

            WriteCallback(ms.LogLevel.Error, formatMessageCallback);
        }

        public virtual void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsErrorEnabled)
                return;

            WriteCallback(ms.LogLevel.Error, formatMessageCallback, exception);
        }

        public virtual void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsErrorEnabled)
                return;

            WriteCallback(ms.LogLevel.Error, formatProvider, formatMessageCallback);
        }

        public virtual void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsErrorEnabled)
                return;

            WriteCallback(ms.LogLevel.Error, formatProvider, formatMessageCallback, exception);
        }

        public virtual void Fatal(object message)
        {
            if (!IsFatalEnabled)
                return;

            Write(ms.LogLevel.Critical, message);
        }

        public virtual void Fatal(object message, Exception exception)
        {
            if (!IsFatalEnabled)
                return;

            Write(ms.LogLevel.Critical, exception, message);
        }

        public virtual void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;

            WriteFormat(ms.LogLevel.Critical, formatProvider, format, args);
        }

        public virtual void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            if (!IsFatalEnabled)
                return;

            WriteFormat(ms.LogLevel.Critical, exception, formatProvider, format, args);
        }

        public virtual void FatalFormat(string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;

            WriteFormat(ms.LogLevel.Critical, format, args);
        }

        public virtual void FatalFormat(string format, Exception exception, params object[] args)
        {
            if (!IsFatalEnabled)
                return;

            WriteFormat(ms.LogLevel.Critical, exception, format, args);
        }

        public virtual void Fatal(Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsFatalEnabled)
                return;

            WriteCallback(ms.LogLevel.Critical, formatMessageCallback);
        }

        public virtual void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsFatalEnabled)
                return;

            WriteCallback(ms.LogLevel.Critical, formatMessageCallback, exception);
        }

        public virtual void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
        {
            if (!IsFatalEnabled)
                return;

            WriteCallback(ms.LogLevel.Critical, formatProvider, formatMessageCallback);
        }

        public virtual void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
        {
            if (!IsFatalEnabled)
                return;

            WriteCallback(ms.LogLevel.Critical, formatProvider, formatMessageCallback, exception);
        }

        public virtual IVariablesContext GlobalVariablesContext => LogVariablesContext.GlobalVariablesContext;

        public virtual IVariablesContext ThreadVariablesContext => LogVariablesContext.ThreadLocal.Value;

        public virtual INestedVariablesContext NestedThreadVariablesContext => LogVariablesContext.NestedThreadVariablesContext.Value;

        protected void Write(ms.LogLevel level, object message)
        {
            Write(level, null, message);
        }

        protected void Write(ms.LogLevel level, Exception exception, object message)
        {
            if (message is string msg)
                _logger.Log(level, exception, "{Message:l}", msg);
            else
                _logger.Log(level, exception, "{@Message}", message);
        }

        protected void WriteCallback(ms.LogLevel level, Action<FormatMessageHandler> formatMessageCallback, Exception exception = null)
        {
            WriteCallback(level, null, formatMessageCallback, exception);
        }

        protected void WriteCallback(
            ms.LogLevel level,
            IFormatProvider formatProvider,
            Action<FormatMessageHandler> formatMessageCallback,
            Exception exception = null)
        {
            formatMessageCallback(MakeFormatted(level, formatProvider, exception));
        }

        protected FormatMessageHandler MakeFormatted(ms.LogLevel level, IFormatProvider formatProvider, Exception exception)
        {
            var messageHandler = new FormatMessageHandler(
                delegate (string message, object[] parameters)
                {
                    string formatted = string.Format(formatProvider, message, parameters);

                    if (formatProvider == null)
                        WriteFormat(level, exception, message, parameters);
                    else
                        Write(level, exception, formatted);

                    return formatted;
                });

            return messageHandler;
        }

        protected void WriteFormat(ms.LogLevel level, Exception exception, string message, object[] parameters)
        {
            WriteFormat(level, exception, null, message, parameters);
        }

        protected void WriteFormat(ms.LogLevel level, IFormatProvider formatProvider, string message, object[] parameters)
        {
            WriteFormat(level, null, formatProvider, message, parameters);
        }

        protected void WriteFormat(ms.LogLevel level, string message, object[] parameters)
        {
            WriteFormat(level, null, null, message, parameters);
        }

        protected void WriteFormat(ms.LogLevel level, Exception exception, IFormatProvider formatProvider, string message, object[] parameters)
        {
            if (formatProvider == null)
            {
                string template;
                object[] args;
                _preformatter.TryPreformat(message, parameters, out template, out args);
                _logger.Log(level, exception, template, args);
            }
            else
                Write(level, exception, string.Format(formatProvider, message, parameters));
        }


    }
}
