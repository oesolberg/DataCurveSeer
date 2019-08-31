using System;

namespace DataCurve.Common.Interfaces
{
	public interface IIniSettings : IDisposable
	{
		void LoadSettingsFromIniFile();

		void SaveSettingsToIniFile();

		LogLevel LogLevel { get; set; }
	}
}