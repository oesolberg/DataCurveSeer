using System;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurveSeer.Common
{
	public class IniSettings : IIniSettings, IDisposable
	{
		private readonly string ConfigSection = "CONFIG";

		public const string LogLevelKey = "LOGLEVEL";


		private bool _disposed;
		private readonly IHSApplication _hs;
		private LogLevel _logLevel;

		public IniSettings(IHSApplication hs)
		{
			_hs = hs;
		}

		public void LoadSettingsFromIniFile()
		{
			_logLevel = GetLogLevel();


			//_gcalendarHashItems = new Dictionary<int, string>();
			//var tempGcalendarItems = _hs.GetINISectionEx(GCalendarItemsSection, Utility.Inifile).ToList();
			//CreateCalendarItemHashList(tempGcalendarItems, _gcalendarHashItems);

			//_gcalendarIdItems = new Dictionary<int, string>();
			//tempGcalendarItems = _hs.GetINISectionEx(GCalendarIdItemsSection, Utility.Inifile).ToList();
			//CreateCalendarItemIdDictionary(tempGcalendarItems, _gcalendarIdItems);

			//_mscalendarHashItems = new Dictionary<int, string>();
			//var tempMsCalendarItems = _hs.GetINISectionEx(MsCalendarItemsSection, Utility.Inifile).ToList();
			//CreateCalendarItemHashList(tempMsCalendarItems, _mscalendarHashItems);

			//_msCalendarIdItems = new Dictionary<int, string>();
			//tempMsCalendarItems = _hs.GetINISectionEx(MsCalendarIdItemsSection, Utility.Inifile).ToList();
			//CreateCalendarItemIdDictionary(tempMsCalendarItems, _msCalendarIdItems);

			
			//_msAppId = GetMsAppId();
			//_msAppPassword = GetMsAppPassword();
			//_msRedirectUri = GetMsRedirectUri();
			//_calendarCheckInterval = GetCalendarCheckInterval();
			//_checkTriggerTimerInterval = GetCheckTriggerTimerInterval();
		}

		public void SaveSettingsToIniFile()
		{
			//_hs.ClearINISection(MsCalendarItemsSection, Utility.Inifile);
			//_mscalendarHashItems = new Dictionary<int, string>();
			//foreach (var msCalendarItem in calendarListToSave)
			//{
			//	if (msCalendarItem.IsChecked)
			//	{
			//		_hs.SaveINISetting(MsCalendarItemsSection, msCalendarItem.Calendar.CalendarId.GetHashCode().ToString(), msCalendarItem.Calendar.CalendarName, Utility.Inifile);
			//		_mscalendarHashItems.Add(msCalendarItem.Calendar.CalendarId.GetHashCode(), msCalendarItem.Calendar.CalendarName);
			//	}
			//}
			////Store the calendarIds as well

			//_hs.ClearINISection(MsCalendarIdItemsSection, Utility.Inifile);
			//_msCalendarIdItems = new Dictionary<int, string>();
			//foreach (var msCalendarItem in calendarListToSave)
			//{
			//	if (msCalendarItem.IsChecked)
			//	{
			//		_hs.SaveINISetting(MsCalendarIdItemsSection, msCalendarItem.Calendar.CalendarId.GetHashCode().ToString(), msCalendarItem.Calendar.CalendarId, Utility.Inifile);
			//		_msCalendarIdItems.Add(msCalendarItem.Calendar.CalendarId.GetHashCode(), msCalendarItem.Calendar.CalendarId);
			//	}
			//}
			////Warn about changed inisettings
			//OnIniSettingsChanged();

		}

		private LogLevel GetLogLevel()
		{
			var debugLevelAsString = _hs.GetINISetting(ConfigSection, LogLevelKey, "NONE", Utility.IniFile);
			LogLevel logLevelToReturn;
			if (!Enum.TryParse(debugLevelAsString, true, out logLevelToReturn))
			{
				logLevelToReturn = LogLevel.Normal;
			}
			return logLevelToReturn;
		}

		public LogLevel LogLevel
		{
			get => _logLevel;
			set
			{
				_logLevel = value;
				SaveLoglevel();
				OnIniSettingsChanged();
			}
		}

		private void SaveLoglevel()
		{

			var loglevelToSave = Enum.GetName(typeof(LogLevel), _logLevel);

			_hs.SaveINISetting(ConfigSection, LogLevelKey, loglevelToSave, Utility.IniFile);
			//OnIniSettingsChanged();
		}

		public event IniSettingsChangedEventHandler IniSettingsChanged;


		protected virtual void OnIniSettingsChanged()
		{
			IniSettingsChanged?.Invoke(this, EventArgs.Empty);
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