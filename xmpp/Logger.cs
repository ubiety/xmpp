using System;
using log4net;
using log4net.Config;

namespace xmpp.logging
{
   public class Logger
   {
      private Logger()
      {
      }

      static Logger()
      {
         XmlConfigurator.Configure();
      }

      #region .: Enabled Checks :.

      public static bool IsDebugEnabled(object o)
      {
         return IsDebugEnabled(o.GetType());
      }

      public static bool IsDebugEnabled(Type t)
      {
         return IsDebugEnabled(t.FullName);
      }

      public static bool IsDebugEnabled(string name)
      {
         return LogManager.GetLogger(name).IsDebugEnabled;
      }

      public static bool IsInfoEnabled(object o)
      {
         return IsInfoEnabled(o.GetType());
      }

      public static bool IsInfoEnabled(Type t)
      {
         return IsInfoEnabled(t.FullName);
      }

      public static bool IsInfoEnabled(string name)
      {
         return LogManager.GetLogger(name).IsInfoEnabled;
      }

      public static bool IsWarnEnabled(object o)
      {
         return IsWarnEnabled(o.GetType());
      }

      public static bool IsWarnEnabled(Type t)
      {
         return IsWarnEnabled(t.FullName);
      }

      public static bool IsWarnEnabled(string name)
      {
         return LogManager.GetLogger(name).IsWarnEnabled;
      }

      public static bool IsErrorEnabled(object o)
      {
         return IsErrorEnabled(o.GetType());
      }

      public static bool IsErrorEnabled(Type t)
      {
         return IsErrorEnabled(t.FullName);
      }

      public static bool IsErrorEnabled(string name)
      {
         return LogManager.GetLogger(name).IsErrorEnabled;
      }

      public static bool IsFatalEnabled(object o)
      {
         return IsFatalEnabled(o.GetType());
      }

      public static bool IsFatalEnabled(Type t)
      {
         return IsFatalEnabled(t.FullName);
      }

      public static bool IsFatalEnabled(string name)
      {
         return LogManager.GetLogger(name).IsFatalEnabled;
      }

      #endregion

      #region .: Debug :.

      public static void Debug(object o, object message)
      {
         Debug(o.GetType(), message);
      }

      public static void Debug(Type t, object message)
      {
         Debug(t.FullName, message);
      }

      public static void Debug(string name, object message)
      {
         LogManager.GetLogger(name).Debug(message);
      }

      public static void DebugFormat(object o, string format, params object[] parameters)
      {
         DebugFormat(o.GetType(), format, parameters);
      }

      public static void DebugFormat(Type t, string format, params object[] parameters)
      {
         DebugFormat(t.FullName, format, parameters);
      }

      public static void DebugFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).DebugFormat(format, parameters);
      }

      #endregion

      #region .: Info :.

      public static void Info(object o, object message)
      {
         Info(o.GetType(), message);
      }

      public static void Info(Type t, object message)
      {
         Info(t.FullName, message);
      }

      public static void Info(string name, object message)
      {
         LogManager.GetLogger(name).Info(message);
      }

      public static void InfoFormat(object o, string format, params object[] parameters)
      {
         InfoFormat(o.GetType(), format, parameters);
      }

      public static void InfoFormat(Type t, string format, params object[] parameters)
      {
         InfoFormat(t.FullName, format, parameters);
      }

      public static void InfoFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).InfoFormat(format, parameters);
      }

      #endregion

      #region .: Warn :.

      public static void Warn(object o, object message)
      {
         Warn(o.GetType(), message);
      }

      public static void Warn(Type t, object message)
      {
         Warn(t.FullName, message);
      }

      public static void Warn(string name, object message)
      {
         LogManager.GetLogger(name).Warn(message);
      }

      public static void WarnFormat(object o, string format, params object[] parameters)
      {
         WarnFormat(o.GetType(), format, parameters);
      }

      public static void WarnFormat(Type t, string format, params object[] parameters)
      {
         WarnFormat(t.FullName, format, parameters);
      }

      public static void WarnFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).WarnFormat(format, parameters);
      }

      #endregion

      #region .: Error :.

      public static void Error(object o, object message)
      {
         Error(o.GetType(), message);
      }

      public static void Error(Type t, object message)
      {
         Error(t.FullName, message);
      }

      public static void Error(string name, object message)
      {
         LogManager.GetLogger(name).Error(message);
      }

      public static void ErrorFormat(object o, string format, params object[] parameters)
      {
         ErrorFormat(o.GetType(), format, parameters);
      }

      public static void ErrorFormat(Type t, string format, params object[] parameters)
      {
         ErrorFormat(t.FullName, format, parameters);
      }

      public static void ErrorFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).ErrorFormat(format, parameters);
      }

      #endregion

      #region .: Fatal :.

      public static void Fatal(object o, object message)
      {
         Fatal(o.GetType(), message);
      }

      public static void Fatal(Type t, object message)
      {
         Fatal(t.FullName, message);
      }

      public static void Fatal(string name, object message)
      {
         LogManager.GetLogger(name).Fatal(message);
      }

      public static void FatalFormat(object o, string format, params object[] parameters)
      {
         FatalFormat(o.GetType(), format, parameters);
      }

      public static void FatalFormat(Type t, string format, params object[] parameters)
      {
         FatalFormat(t.FullName, format, parameters);
      }

      public static void FatalFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).FatalFormat(format, parameters);
      }

      #endregion
   }
}