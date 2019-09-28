using System;

namespace DataCurveSeer.Common.Interfaces
{
	public interface IIniSettings : IDisposable
	{
		void LoadSettingsFromIniFile();

		void SaveSettingsToIniFile();


		event IniSettingsChangedEventHandler IniSettingsChanged;

		LogLevel LogLevel { get; set; }
        int DaysOfDataStorage { get; set; }
    }
}