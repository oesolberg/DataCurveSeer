using System.Collections.Generic;
using System.Collections.Specialized;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.Logging;
using DataCurveSeer.MainPlugin;
using HomeSeerAPI;
using Hspi;
using Scheduler;

namespace HSPI_DataCurveSeer
{
	// ReSharper disable once InconsistentNaming
	public class HSPI : HspiBase
	{
		private IMainPlugin _mainPlugin;
		private IniSettings _iniSettings;
		private Logging _logging;

		public override string InitIO(string port)
		{
			// debug
			HS.WriteLog(Utility.PluginName, "Entering InitIO");

			// initialise everything here, return a blank string only if successful, or an error message
			// initialise everything here, return a blank string only if successful, or an error message
			_iniSettings = new IniSettings(HS);
			_iniSettings.LoadSettingsFromIniFile();
			_logging = new Logging(_iniSettings, HS);
			_mainPlugin = new MainPlugin(HS,_logging,_iniSettings,Callback, new HsCollectionFactory());
			_mainPlugin.InitIO(port);
			// debug

			HS.WriteLog(Name, "Completed InitIO");
			return "";
		}

		public override void ShutdownIO()
		{
			// debug
			HS.WriteLog(Name, "Entering ShutdownIO");

			// shut everything down here

			_mainPlugin.ShutDownIO();
			_mainPlugin.Dispose();
			// let our console wrapper know we are finished
			Shutdown = true;

			// debug
			HS.WriteLog(Name, "Completed ShutdownIO");
		}

		protected override bool GetHasTriggers()
		{
			return _mainPlugin.GetHasTriggers();
		}

		protected override int GetTriggerCount()
		{
			return _mainPlugin.GetTriggerCount();
		}

		public override string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return _mainPlugin.TriggerBuildUI(uniqueControlId, triggerInfo);
		}

		//Shows a summary of the settings for the trigger when it is set correct and collapsed
		public override string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _mainPlugin.TriggerFormatUI(actionInfo);
			
		}

		//processes the post from changing the trigger UI
		public override IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
			IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _mainPlugin.TriggerProcessPostUI(postData, actionInfo);
		}

		//Not in use - should be some kind of search thing
		public override bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
		{
			return false;
		}

		public override bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _mainPlugin.TriggerTrue(actionInfo);
		}

		public override int get_SubTriggerCount(int triggerNumber)
		{
			return _mainPlugin.get_SubTriggerCount(triggerNumber);
		}

		public override string get_SubTriggerName(int triggerNumber, int subTriggerNumber)
		{
			return _mainPlugin.get_SubTriggerName(triggerNumber, subTriggerNumber);
		}

		public override bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _mainPlugin.get_TriggerConfigured(actionInfo);
		}

		public override string get_TriggerName(int triggerNumber)
		{
			return _mainPlugin.get_TriggerName(triggerNumber);
		}

		public override bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _mainPlugin.get_Condition(actionInfo);
		}

		public override bool get_HasConditions(int triggerNumber)
		{
			return _mainPlugin.get_HasConditions(triggerNumber);
		}

		public override void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
		{
			_mainPlugin.set_Condition(actionInfo,value);
		}


		public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)
		{
			return "";
		}

		public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return true;
		}

		public override int ActionCount()
		{
			return 0;
		}

		public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return "";
		}

		public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData,
			IPlugInAPI.strTrigActInfo actionInfo)
		{
			return new IPlugInAPI.strMultiReturn();
		}

		public override bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
		{
			return false;
		}

		public override string get_ActionName(int actionNumber)
		{
			return "";
		}

		

		public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return false;
		}

		
		public override string InstanceFriendlyName()
		{
			return string.Empty;
		}

		public override IPlugInAPI.strInterfaceStatus InterfaceStatus()
		{
			return new IPlugInAPI.strInterfaceStatus { intStatus = IPlugInAPI.enumInterfaceStatus.OK };
		}

		public override int Capabilities()
		{
			return (int)Enums.eCapabilities.CA_IO;
		}

		public override int AccessLevel()
		{
			return 1;
		}

		public override bool SupportsAddDevice()
		{
			return false;
		}

		public override bool SupportsConfigDevice()
		{
			return false;
		}

		public override bool SupportsConfigDeviceAll()
		{
			return false;
		}

		public override bool SupportsMultipleInstances()
		{
			return false;
		}

		public override bool SupportsMultipleInstancesSingleEXE()
		{
			return false;
		}

		public override bool RaisesGenericCallbacks()
		{
			return false;
		}

		public override void HSEvent(Enums.HSEvent eventType, object[] parameters)
		{
			_mainPlugin.HsEvent(eventType, parameters);
		}

	
		public override IPlugInAPI.PollResultInfo PollDevice(int deviceId)
		{
			// return the value of a device on demand

			return new IPlugInAPI.PollResultInfo
			{
				Result = IPlugInAPI.enumPollResult.Device_Not_Found,
				Value = 0
			};
		}

		public override void SetIOMulti(List<CAPI.CAPIControl> colSend)
		{
			// HomeSeer will inform us when the one of our devices has changed.  Push that change through to the field.
		}

		public override SearchReturn[] Search(string searchString, bool regEx)
		{
			return null;
		}
		

		public override void SpeakIn(int deviceId, string txt, bool w, string host)
		{
		}

		public override string GenPage(string link)
		{
			return "";
		}

		public override string PagePut(string data)
		{
			return "";
		}

		public override string GetPagePlugin(string page, string user, int userRights, string queryString)
		{
			return _mainPlugin.GetPagePlugin(page, user, userRights, queryString);
		}

		public override string PostBackProc(string page, string data, string user, int userRights)
		{
			return "";
		}

		public override string ConfigDevice(int deviceId, string user, int userRights, bool newDevice)
		{
			return "";
		}

		public override Enums.ConfigDevicePostReturn ConfigDevicePost(int deviceId, string data, string user, int userRights)
		{
			return Enums.ConfigDevicePostReturn.DoneAndCancel;
		}

		public override object PluginFunction(string functionName, object[] parameters)
		{
			return null;
		}

		public override object PluginPropertyGet(string propertyName, object[] parameters)
		{
			return null;
		}

		public override void PluginPropertySet(string propertyName, object value)
		{
		}

		

		protected override string GetName()
		{
			return Utility.PluginName;
		}

		protected override bool GetHscomPort()
		{
			return true;
		}

		public override void SetDeviceValue(int deviceId, double value, bool trigger = true)
		{
		}
	}
}