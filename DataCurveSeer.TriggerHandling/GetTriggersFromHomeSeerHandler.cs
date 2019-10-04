using System;
using System.Linq;
using System.Threading;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurveSeer.TriggerHandling
{
    public interface IGetTriggersFromHomeSeerHandler
    {
        void DelayFetching(int? delayInSeconds = null);
        event TriggerDataReadyEventHandler TriggerDataReady;
        void StopWork();
        void StartWork();
        bool IsRunning { get; }
    }

    public class GetTriggersFromHomeSeerHandler : IGetTriggersFromHomeSeerHandler, IDisposable
    {
        private bool _disposed;
        private bool _stopRunning;
        private int _sleepTime = 300;
        private readonly IIniSettings _iniSettings;
        private readonly ILogging _logging;
        private DateTime _timeForFetching;
        private Thread _workThread;
        private readonly IAppCallbackAPI _callback;
        private readonly int _delayInSeconds = 60;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public GetTriggersFromHomeSeerHandler(IIniSettings iniSettings, ILogging logging, IAppCallbackAPI callback, int? delayInSeconds = null)
        {
            _iniSettings = iniSettings;
            _logging = logging;
            _callback = callback;
            if (delayInSeconds.HasValue)
            {
                _delayInSeconds = delayInSeconds.Value;
            }
        }
        //Should only exist as long as we have a Queue with refetch to be done
        public void DelayFetching(int? delayInSeconds = null)
        {
            if (delayInSeconds.HasValue)
            {
                _timeForFetching = SystemDateTime.Now().AddSeconds(delayInSeconds.Value);
            }
            else
            {
                _timeForFetching = SystemDateTime.Now().AddSeconds(_delayInSeconds);
            }
        }

        public event TriggerDataReadyEventHandler TriggerDataReady;

        private void OnTriggerDataReady(IPlugInAPI.strTrigActInfo[] triggersInPlugin)
        {
            //Only send if we have any subscribers
            TriggerDataReady?.Invoke(this, new TriggersInHomeSeerDataEventArgs(triggersInPlugin.ToList()));
        }

        public void StartWork()
        {
            _timeForFetching = SystemDateTime.Now().AddSeconds(_delayInSeconds);
            _workThread = new Thread(DoWork) { Name = $"RefetchThreadQueue_{SystemDateTime.Now().ToString("yyyyMMddHHmmss")}" };
            _workThread.Start();
        }

        private void DoWork()
        {
            _isRunning = true;
            do
            {
                Thread.Sleep(_sleepTime);
                if (SystemDateTime.Now() >= _timeForFetching)
                {
                    var triggersInPlugin = _callback.GetTriggers(Utility.PluginName);
                    if (triggersInPlugin != null)
                    {
                        OnTriggerDataReady(triggersInPlugin);
                    }
                    _stopRunning = true;
                }
            } while (!_stopRunning);

            _isRunning = false;
        }

        public void StopWork()
        {
            _stopRunning = true;
            _workThread?.Join(_sleepTime * 2);
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