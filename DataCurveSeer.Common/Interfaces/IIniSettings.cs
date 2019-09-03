using System;

namespace DataCurveSeer.Common.Interfaces
{
	public interface IIniSettings : IDisposable
	{
		void LoadSettingsFromIniFile();

		void SaveSettingsToIniFile();

		LogLevel LogLevel { get; set; }
	}
}