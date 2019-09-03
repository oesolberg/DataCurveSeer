using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurve.Common;
using DataCurve.Common.Interfaces;
using HomeSeerAPI;
using Serilog;
using Serilog.Core;

namespace DataCurve.Logging
{
	public class Logging : ILogging
	{
		private const string OrangeColor = "#FFA500";
		private const string RedColor = "#FF0000";
		private static object _lockObject = new object();
		private readonly IIniSettings _iniSettings;
		private readonly IHSApplication _hs;
		private bool _disposed;
		private Logger _seriLogger = null;



		public Logging(IIniSettings iniSettings, IHSApplication hs)
		{
			_iniSettings = iniSettings;
			_hs = hs;

			var logPath = Path.Combine(Utility.ExePath, "Logs");
			if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
			var logFile = Path.Combine(logPath, Utility.PluginName + "Debug.log");
			if (_iniSettings.LogLevel == LogLevel.DebugToFile || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)
			{
				CreateLogFile();
			}
		}

		public void LogToHsNoMatterSettings(string logMessage)
		{
			DoLog(logMessage, LogLevel.IgnoreSettings);
		}

		public void LogError(string message)
		{
			_hs.WriteLogEx(Utility.PluginName + "-Error", message, RedColor);
		}

		public void LogWarning(string message)
		{
			_hs.WriteLogEx(Utility.PluginName + "-Warn", message, OrangeColor);
		}

		public void Log(string message)
		{
			DoLog(message, LogLevel.Normal);
		}

		public void Log(string message, LogLevel logLevel)
		{
			if (logLevel == _iniSettings.LogLevel)
				DoLog(message, logLevel);
		}

		public void LogDebug(string message)
		{
			DoLog(message, LogLevel.Debug);
		}

		public void LogRfLinkDataToFile(string eventArgsRfLinkData)
		{
			throw new NotImplementedException();
		}

		public void LogIfLogToFileEnabled(string message)
		{
			if (_iniSettings.LogLevel == LogLevel.DebugToFile || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)
			{
				lock (_lockObject)
				{
					_seriLogger.Debug($"{message}");
					Serilog.Log.CloseAndFlush();
				}
			}
		}

		public LogLevel LogLevel => _iniSettings.LogLevel;

		public void LogException(Exception ex)
		{
			if (ex != null)
			{
				_hs.WriteLog(Utility.PluginName, "Error: " + ex.Message);
				_hs.WriteLog(Utility.PluginName, ex.StackTrace);
			}
		}

		private void DoLog(string message, LogLevel logLevel = LogLevel.Normal)
		{
			if (_iniSettings.LogLevel == LogLevel.None && logLevel != LogLevel.IgnoreSettings) return; // Logging = off in config
																									   //Write to Homseseer log

			if ((logLevel == LogLevel.IgnoreSettings) || //Log that will be written no matter what ini setting
				(logLevel == LogLevel.Normal && _iniSettings.LogLevel == LogLevel.Normal) || //Log that will be written when ini setting is set to normal
				(logLevel == LogLevel.Debug && (_iniSettings.LogLevel == LogLevel.Debug || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)))
			{
				_hs.WriteLog(Utility.PluginName, message);
			}

			//Write to logfile
			if (_iniSettings.LogLevel == LogLevel.DebugToFile || _iniSettings.LogLevel == LogLevel.DebugToFileAndLog)
			{
				LogIfLogToFileEnabled(message);
			}
		}

		public void IniSettingHasChanged(object sender, EventArgs eventargs)
		{
			if (_iniSettings.LogLevel != LogLevel.DebugToFileAndLog && _iniSettings.LogLevel != LogLevel.DebugToFile)
			{
				CloseLogFile();
			}

			if (_iniSettings.LogLevel == LogLevel.DebugToFileAndLog || _iniSettings.LogLevel == LogLevel.DebugToFile)
			{
				CreateLogFile();
			}
		}

		private void CreateLogFile()
		{
			if (_seriLogger != null) return;
			var logPath = Path.Combine(Utility.ExePath, "Logs");
			if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
			var logFile = Path.Combine(logPath, Utility.PluginName + "Debug.log");
			lock (_lockObject)
			{
				_seriLogger = new LoggerConfiguration()
					.MinimumLevel.Debug()

					.WriteTo.File(path: logFile, rollingInterval: RollingInterval.Day, shared: true)
					.CreateLogger();
			}
		}

		private void CloseLogFile()
		{
			if (_seriLogger != null)
			{
				lock (_lockObject)
				{
					_seriLogger.Dispose();
					_seriLogger = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);

			// Use SupressFinalize in case a subclass 
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// Indicate that the instance has been disposed.
				_disposed = true;
			}
		}
	}
}
