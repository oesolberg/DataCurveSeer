using System;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurveSeer.Common
{
    public class IniSettings : IIniSettings, IDisposable
    {
        private readonly string ConfigSection = "CONFIG";

        public const string LogLevelKey = "LOGLEVEL";
        public const string DaysOfDataStorageKey = "DAYSOFDATASTORAGE";

        private bool _disposed;
        private readonly IHSApplication _hs;
        private LogLevel _logLevel;
        private int _daysOfDataStorage;

        public IniSettings(IHSApplication hs)
        {
            _hs = hs;
        }

        public void LoadSettingsFromIniFile()
        {
            _logLevel = GetLogLevel();
            _daysOfDataStorage = GetNumberOfDaysOfDataStorage();
        }

        private int GetNumberOfDaysOfDataStorage()
        {

            var numberOfDaysToStoreDataString = _hs.GetINISetting(ConfigSection, DaysOfDataStorageKey, "2", Utility.IniFile);
            int numberOfDaysToStoreData;
            if (int.TryParse(numberOfDaysToStoreDataString, out numberOfDaysToStoreData))
            {
                return numberOfDaysToStoreData;
            }
            return 2;
        }

        public void SaveSettingsToIniFile()
        {
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

        public int DaysOfDataStorage
        {
            get => _daysOfDataStorage;
            set
            {
                _daysOfDataStorage = value;
                SaveDaysOfDataStorage();
            }
        }

        private void SaveDaysOfDataStorage()
        {
            _hs.SaveINISetting(ConfigSection, DaysOfDataStorageKey, _daysOfDataStorage.ToString(), Utility.IniFile);
            //OnIniSettingsChanged();
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