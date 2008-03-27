using System;
using log4net;
using log4net.Config;

#if DEBUG

namespace xmpp.logging
{
	/// <summary>
	/// 
	/// </summary>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDebugEnabled(object o)
		{
			return IsDebugEnabled(o.GetType());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDebugEnabled(Type t)
		{
			return IsDebugEnabled(t.FullName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsDebugEnabled(string name)
		{
			return LogManager.GetLogger(name).IsDebugEnabled;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsInfoEnabled(object o)
		{
			return IsInfoEnabled(o.GetType());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsInfoEnabled(Type t)
		{
			return IsInfoEnabled(t.FullName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsInfoEnabled(string name)
		{
			return LogManager.GetLogger(name).IsInfoEnabled;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsWarnEnabled(object o)
		{
			return IsWarnEnabled(o.GetType());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsWarnEnabled(Type t)
		{
			return IsWarnEnabled(t.FullName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsWarnEnabled(string name)
		{
			return LogManager.GetLogger(name).IsWarnEnabled;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsErrorEnabled(object o)
		{
			return IsErrorEnabled(o.GetType());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsErrorEnabled(Type t)
		{
			return IsErrorEnabled(t.FullName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsErrorEnabled(string name)
		{
			return LogManager.GetLogger(name).IsErrorEnabled;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsFatalEnabled(object o)
		{
			return IsFatalEnabled(o.GetType());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsFatalEnabled(Type t)
		{
			return IsFatalEnabled(t.FullName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool IsFatalEnabled(string name)
		{
			return LogManager.GetLogger(name).IsFatalEnabled;
		}

      #endregion

      #region .: Debug :.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Debug(object o, object message)
		{
			Debug(o.GetType(), message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Debug(Type t, object message)
		{
			Debug(t.FullName, message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Debug(string name, object message)
		{
			LogManager.GetLogger(name).Debug(message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void DebugFormat(object o, string format, params object[] parameters)
		{
			DebugFormat(o.GetType(), format, parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void DebugFormat(Type t, string format, params object[] parameters)
		{
			DebugFormat(t.FullName, format, parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void DebugFormat(string name, string format, params object[] parameters)
		{
			LogManager.GetLogger(name).DebugFormat(format, parameters);
		}

      #endregion

      #region .: Info :.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Info(object o, object message)
		{
			Info(o.GetType(), message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Info(Type t, object message)
		{
			Info(t.FullName, message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Info(string name, object message)
		{
			LogManager.GetLogger(name).Info(message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void InfoFormat(object o, string format, params object[] parameters)
		{
			InfoFormat(o.GetType(), format, parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void InfoFormat(Type t, string format, params object[] parameters)
		{
			InfoFormat(t.FullName, format, parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void InfoFormat(string name, string format, params object[] parameters)
		{
			LogManager.GetLogger(name).InfoFormat(format, parameters);
		}

      #endregion

      #region .: Warn :.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Warn(object o, object message)
		{
			Warn(o.GetType(), message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Warn(Type t, object message)
		{
			Warn(t.FullName, message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
		public static void Warn(string name, object message)
		{
			LogManager.GetLogger(name).Warn(message);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void WarnFormat(object o, string format, params object[] parameters)
      {
         WarnFormat(o.GetType(), format, parameters);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void WarnFormat(Type t, string format, params object[] parameters)
      {
         WarnFormat(t.FullName, format, parameters);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void WarnFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).WarnFormat(format, parameters);
      }

      #endregion

      #region .: Error :.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void Error(object o, object message)
      {
         Error(o.GetType(), message);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void Error(Type t, object message)
      {
         Error(t.FullName, message);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void Error(string name, object message)
      {
         LogManager.GetLogger(name).Error(message);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void ErrorFormat(object o, string format, params object[] parameters)
      {
         ErrorFormat(o.GetType(), format, parameters);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void ErrorFormat(Type t, string format, params object[] parameters)
      {
         ErrorFormat(t.FullName, format, parameters);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void ErrorFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).ErrorFormat(format, parameters);
      }

      #endregion

      #region .: Fatal :.

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void Fatal(object o, object message)
      {
         Fatal(o.GetType(), message);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void Fatal(Type t, object message)
      {
         Fatal(t.FullName, message);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void Fatal(string name, object message)
      {
         LogManager.GetLogger(name).Fatal(message);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void FatalFormat(object o, string format, params object[] parameters)
      {
         FatalFormat(o.GetType(), format, parameters);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t">
		/// A <see cref="Type"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void FatalFormat(Type t, string format, params object[] parameters)
      {
         FatalFormat(t.FullName, format, parameters);
      }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="parameters">
		/// A <see cref="System.Object"/>
		/// </param>
      public static void FatalFormat(string name, string format, params object[] parameters)
      {
         LogManager.GetLogger(name).FatalFormat(format, parameters);
      }

      #endregion
   }
}

#endif