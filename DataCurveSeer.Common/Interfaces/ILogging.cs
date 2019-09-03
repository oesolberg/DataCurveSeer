using System;

namespace DataCurve.Common.Interfaces
{
	public interface ILogging : IDisposable
	{
		void LogToHsNoMatterSettings(string logMessage);
		void Log(string logMessage);
		void Log(string logMessage, LogLevel logLevel);
		void LogDebug(string logMessage);
		void LogRfLinkDataToFile(string eventArgsRfLinkData);
		LogLevel LogLevel { get; }
		void LogException(Exception ex);
		void LogError(string logMessage);
		void LogWarning(string logMessage);
		void LogIfLogToFileEnabled(string logMessage);
		void IniSettingHasChanged(object sender, EventArgs eventArgs);
	}
}