using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.Config;
using DataCurveSeer.HomeSeerHandling;
using DataCurveSeer.Storage;
using DataCurveSeer.TriggerHandling;
using HomeSeerAPI;

namespace DataCurveSeer.MainPlugin
{
    public class MainPlugin:IMainPlugin,IDisposable 
    {
	    private bool _disposed;
	    private readonly IHSApplication _hs;
	    private IMainConfig _config;
	    private readonly ILogging _logging;
	    private readonly IIniSettings _iniSettings;
	    private readonly IAppCallbackAPI _callback;
	    private ITriggerHandler _triggerHandler;
	    private HsCollectionFactory _collectionFactory;
	    private IHomeSeerHandler _homeSeerHandler;
	    private IStorageHandler _storageHandler;
	    private long _numberOfEventsReceived;
	    private long _numberOfEventsStored;
	    private long _numberOfValueChanngeEventsReceived;


	    public MainPlugin(IHSApplication hs,ILogging logging,IIniSettings iniSettings, IAppCallbackAPI callback,HsCollectionFactory collectionFactory)
	    {
		    _hs = hs;
		    _logging = logging;
		    _iniSettings = iniSettings;
		    _callback = callback;
		    _collectionFactory = collectionFactory;

	    }
	    public string InitIO(string port)
	    {
		    var initLogString = $"{Utility.PluginName} MainPlugin InitIo started";
			Console.WriteLine(initLogString);
			_logging.Log(initLogString);
			_config = new MainConfig(_logging, _hs,  _iniSettings, _callback, this);
		    _config.RegisterConfigs();

		    _callback.RegisterEventCB(Enums.HSEvent.VALUE_CHANGE, Utility.PluginName, "");

		    _logging.Log($"Done creating configs");
			_iniSettings.IniSettingsChanged += _logging.IniSettingHasChanged;
			_homeSeerHandler = new HomeSeerHandler(_hs, _logging);
			_logging.Log($"Done creating HomeSeerHandler");

			_storageHandler = new StorageHandler(_logging);
			_logging.Log($"Done creating StorageHandler");

			_triggerHandler = new TriggerHandler(_hs, _callback, _iniSettings, _logging, _collectionFactory, _homeSeerHandler,_storageHandler);
			_logging.Log($"Done creating _triggerHandler ");

			//_callback.RegisterEventCB(Enums.HSEvent.CONFIG_CHANGE, Utility.PluginName, "");
			//Register callback on every event of value change. This is the method to find if this is a value of a device we are following 
			
			_logging.Log($"Done registering callback");
			_logging.Log($"{Utility.PluginName} MainPlugin InitIo Complete");
		    return "";
	    }

	    public void ShutDownIO()
	    {
		    //throw new NotImplementedException();
	    }

	    public string GetPagePlugin(string page, string user, int userRights, string queryString)
	    {
		    return _config.GetPagePlugin(page, user, userRights, queryString);
	    }

	    public string PostBackProc(string page, string data, string user, int userRights)
	    {
		    return _config.PostBackProc(page, data, user, userRights);
	    }

	    public void HsEvent(Enums.HSEvent eventType, object[] parameters)
	    {
			//Catch changes to values and store them for the devices we are watching
			_numberOfEventsReceived++;
		    if (eventType == Enums.HSEvent.VALUE_CHANGE)
		    {
				//_logging.LogDebug("Got an value changed event. Trying to check if we should store it");
			    _numberOfValueChanngeEventsReceived++;
			    if (parameters!=null && parameters.Length > 4 && parameters[2] != null && parameters[4] != null &&
			        parameters[2] is double && parameters[4] is int)
			    {
				    var newValue = (double) parameters[2];
				    var deviceId = (int) parameters[4];
				    //Console.WriteLine($"Something happened to a value for deviceId {deviceId} (value: {newValue.ToString()})");
				    if (DeviceIsWatched(deviceId))
				    {

					    _logging.LogDebug($"logging an event with data deviceId:{deviceId} value: {newValue}");
						_numberOfEventsStored++;
						_storageHandler.AddDeviceValueToDatabase(newValue,DateTime.Now,deviceId);
				    }
				}
			}
	    }

	    private bool DeviceIsWatched(int deviceId)
	    {
		    return _triggerHandler.IsWatchedDeviceId(deviceId);
	    }

	    public bool GetHasTriggers()
		{
			return _triggerHandler.GetHasTriggers();
		}

		public int GetTriggerCount()
		{
			return _triggerHandler.GetTriggerCount();
		}

		public string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return _triggerHandler.TriggerBuildUi(uniqueControlId, triggerInfo);

		}

		public string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _triggerHandler.TriggerFormatUi(actionInfo);
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData, IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return _triggerHandler.TriggerProcessPostUi(postData, triggerInfo);
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return _triggerHandler.TriggerTrue(triggerInfo);
		}

		public int get_SubTriggerCount(int triggerNumber)
		{
			return _triggerHandler.GetSubTriggerCount(triggerNumber);
		}

		public string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
		{
			return _triggerHandler.GetSubTriggerName(triggerNumber, subTriggerNumber);
		}

		public bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return _triggerHandler.GetTriggerConfigured(triggerInfo);
		}

		public string get_TriggerName(int triggerNumber)
		{
			return _triggerHandler.GetTriggerName(triggerNumber);
		}

		public bool get_Condition(IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return _triggerHandler.GetCondition(triggerInfo);
		}

		public bool get_HasConditions(int triggerNumber)
		{
			return _triggerHandler.GetHasConditions(triggerNumber);
		}

		public void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
		{
			var eventRef = actionInfo.evRef;
			var uid = actionInfo.UID;
			Console.WriteLine($"setting condition to {value.ToString()} for evRef {eventRef} and UID {uid}");
			_triggerHandler.SetCondition(actionInfo,value);
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
