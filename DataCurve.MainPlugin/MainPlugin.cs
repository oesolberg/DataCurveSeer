using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurve.Common;
using DataCurve.Common.Interfaces;
using DataCurve.Config;
using DataCurve.TriggerHandling;
using HomeSeerAPI;

namespace DataCurve.MainPlugin
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

	    public MainPlugin(IHSApplication hs,ILogging logging,IIniSettings iniSettings, IAppCallbackAPI callback)
	    {
		    _hs = hs;
		    _logging = logging;
		    _iniSettings = iniSettings;
		    _callback = callback;

	    }
	    public string InitIO(string port)
	    {

		    _logging.Log($"{Utility.PluginName} MainPlugin InitIo started");
			_config = new MainConfig(_logging, _hs,  _iniSettings, _callback, this);
		    _config.RegisterConfigs();

		    _logging.Log($"{Utility.PluginName} MainPlugin InitIo Complete");
		    _triggerHandler = new TriggerHandler();
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

		public void Dispose()
		{

			Dispose(true);

			// Use SupressFinalize in case a subclass 
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
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

	



		public bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return true;
		}

		public bool get_HasConditions(int triggerNumber)
		{
			return true;
		}

		public void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
		{
			var eventRef = actionInfo.evRef;
			var uid = actionInfo.UID;
			Console.WriteLine($"setting condition to {value.ToString()} for evRef {eventRef} and UID {uid}");
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
