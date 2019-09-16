using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.TriggerHandling.Triggers;
using HomeSeerAPI;

namespace DataCurveSeer.TriggerHandling
{

	public class TriggerHandler : ITriggerHandler, IDisposable
	{
		private IHSApplication _hs;
		private readonly IAppCallbackAPI _callback;
		private IIniSettings _iniSettings;
		private ILogging _logging;
		private readonly IHsCollectionFactory _collectionFactory;
		private bool _disposed;
		private readonly List<ITrigger> _triggerTypes = new List<ITrigger>();
		private List<ITrigger> _triggers = new List<ITrigger>();
		private IHomeSeerHandler _homeSeerHandler;
		private List<int> _watchedEventDeviceIds = new List<int>();
		private IStorageHandler _storageHandler;

		protected internal const string TriggerTypeKey = "TriggerType";

		public TriggerHandler(IHSApplication hs, IAppCallbackAPI callback, IIniSettings iniSettings, 
			ILogging logging, IHsCollectionFactory collectionFactory,IHomeSeerHandler homeSeerHandler, IStorageHandler storageHandler)
		{
			_hs = hs;
			_callback = callback;
			_iniSettings = iniSettings;
			_logging = logging;
			_collectionFactory = collectionFactory;
			_homeSeerHandler = homeSeerHandler;
			_storageHandler = storageHandler;
			_logging.LogDebug("Creating trigger types");
			_triggerTypes = CreateTriggerTypes();

			_logging.LogDebug("Starting thread to fetch triggers");
			GetPluginTriggersFromHomeSeerInNewThread();

			_logging.LogDebug("Done init TriggerHandler");
		}

		private void GetPluginTriggersFromHomeSeerInNewThread()
		{
			var getTriggerThread=new Thread(GetPluginTriggersFromHomeSeer);
			getTriggerThread.Start();
		}

		public List<int> WatchedEventDeviceIdIds => _watchedEventDeviceIds;

		private List<ITrigger> CreateTriggerTypes()
		{
			var triggers = new List<ITrigger>();
			triggers.Add(new DataCurveTrigger(_hs,_logging,_collectionFactory,_homeSeerHandler));
			return triggers;
		}


		private void GetPluginTriggersFromHomeSeer()
		{
			var triggersInPlugin = _callback.GetTriggers(Utility.PluginName);
			if (triggersInPlugin != null & triggersInPlugin.Length > 0)
				CreateExistingTriggers(triggersInPlugin);
		}

		private void CreateExistingTriggers(IPlugInAPI.strTrigActInfo[] triggersInPlugin)
		{
			foreach (var trigActInfo in triggersInPlugin)
			{
				var actionInfo = _collectionFactory.GetActionsIfPossible(trigActInfo);
				if (actionInfo != null && actionInfo.ContainsKey(TriggerTypeKey))
				{
					CreateTriggerAndAddToCollection(actionInfo, trigActInfo);
				}
			}
		}

		private void CreateTriggerAndAddToCollection(Classes.action settings, IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerType = (string)settings[TriggerTypeKey];
			object[] argObjects = new object[] { _logging, this, _callback };
			var triggerToAdd = TriggerFactory.Get(_hs,triggerType, _logging, this, _callback, _collectionFactory, _homeSeerHandler);
			if (triggerToAdd != null)
			{
				triggerToAdd.AddSettingsFromTrigActionInfo(trigActInfo);
				AddOrUpdatedToRunningTriggers(triggerToAdd);
				AddDeviceId(triggerToAdd.DeviceId);
			}
		}

		private void AddDeviceId(int? deviceId)
		{
			if (deviceId.HasValue && !_watchedEventDeviceIds.Exists(x => x == deviceId.Value))
			{
				_watchedEventDeviceIds.Add(deviceId.Value);
			}
		}

		private void AddOrUpdatedToRunningTriggers(ITrigger foundTrigger)
		{
			var test = _triggers.Count();
			if (foundTrigger != null)
			{
				if (_triggers.Exists(x => x.GetTriggerActionInfo().UID == foundTrigger.GetTriggerActionInfo().UID
													 && x.GetTriggerActionInfo().evRef == foundTrigger.GetTriggerActionInfo().evRef))
				{
					//Remove the old
					_triggers.RemoveAll(x => x.GetTriggerActionInfo().UID == foundTrigger.GetTriggerActionInfo().UID
														&& x.GetTriggerActionInfo().evRef == foundTrigger.GetTriggerActionInfo().evRef);
				}
				_triggers.Add(foundTrigger);

			}
		}



		public string GetSubTriggerName(int triggerNumber, int subTriggerNumber)
		{
			var foundTrigger = FindCorrectTriggerTypeInfo(triggerNumber);
			if (foundTrigger == null) return "";
			return foundTrigger.GetSubTriggerName(subTriggerNumber);
		}

		public bool GetHasConditions(int triggerNumber)
		{
			var trigger = FindCorrectTriggerTypeInfo(triggerNumber);
			if (trigger == null) return false;
			return trigger.GetHasConditions();
		}

		private ITrigger FindCorrectTriggerTypeInfo(int triggerNumber)
		{
			return _triggerTypes.SingleOrDefault(x => x.TriggerNumber == triggerNumber);
		}

		private ITrigger FindTrigger(IPlugInAPI.strTrigActInfo actionInfo)
		{
			if (_triggers.Count > 0)
			{
				var foundRunningTrigger = _triggers.SingleOrDefault(x =>
					x.TriggerNumber == actionInfo.TANumber && x.UID == actionInfo.UID
																 && x.EvRef == actionInfo.evRef
																 && x.ContainsSubTriggerActionNumber(actionInfo
																	 .SubTANumber));
				if (foundRunningTrigger != null) return foundRunningTrigger;
			}

			var foundTrigger = _triggerTypes.SingleOrDefault(x => x.TriggerNumber == actionInfo.TANumber
																	  && x.ContainsSubTriggerActionNumber(actionInfo.SubTANumber));
			return foundTrigger;
		}


		public int GetSubTriggerCount(int triggerNumber)
		{
			var foundTrigger = FindCorrectTriggerTypeInfo(triggerNumber);
			if (foundTrigger == null) return 0;
			return foundTrigger.GetSubTriggerCount();
		}

		public int GetTriggerCount()
		{
			return _triggerTypes.Count;
		}

		public bool GetHasTriggers()
		{
			return true;
		}

		public bool IsWatchedDeviceId(int deviceId)
		{
			if (_watchedEventDeviceIds == null || _watchedEventDeviceIds.Count == 0) return false;
			return _watchedEventDeviceIds.Exists(x => x == deviceId);
		}

		public string TriggerBuildUi(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
		{
			var triggerToBuild = FindTrigger(triggerInfo);
			if (triggerToBuild == null) return "";
			return triggerToBuild.TriggerBuildUi(uniqueControlId, triggerInfo);
		}

		public string TriggerFormatUi(IPlugInAPI.strTrigActInfo triggerInfo)
		{
			var triggerToBuild = FindTrigger(triggerInfo);
			if (triggerToBuild == null) return "";
			return triggerToBuild.TriggerFormatUi(triggerInfo);
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData, IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return new IPlugInAPI.strMultiReturn();
			var result= triggerToBuild.TriggerProcessPostUi(postData, trigActInfo);
			StartTriggerUpdateIn500MilliSeconds();
			return result;
		}

		private void StartTriggerUpdateIn500MilliSeconds()
		{
			Console.WriteLine("Here be changes that needs addressing, start a thread in 10 seconds to update number of devices to follow by getting all events?");
			//Spawn a thread for waiting 250 ms, then update the triggerlist
			var t = new Thread(WaitAndRefetchTriggers);
			t.Start();
		}

		private void WaitAndRefetchTriggers()
		{
			//Wait 0.5 seconds to do updates. Hope it is enough time for HomeSeer to get everything done.
			Thread.Sleep(500);
			ReFetchTriggers();
		}


		private void ReFetchTriggers()
		{
			var triggersInPlugin = _callback.GetTriggers(Utility.PluginName);
			//Remove all watched event device Ids
			if (_watchedEventDeviceIds.Count > 0)
			{
				_watchedEventDeviceIds.Clear();
			}
			if (triggersInPlugin != null & triggersInPlugin.Length > 0)
			{
				CreateExistingTriggers(triggersInPlugin);
			}
		}


		public bool TriggerTrue(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return false;
			return triggerToBuild.TriggerTrue(trigActInfo, _storageHandler);
		}

		string ITriggerHandler.GetTriggerName(int triggerNumber)
		{
			var triggerToBuild = FindCorrectTriggerTypeInfo(triggerNumber);
			if (triggerToBuild == null) return "";
			return triggerToBuild.GetTriggerName();
		}

		public bool GetTriggerConfigured(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return false;
			//UpdateDevicesWatchedList(triggerToBuild);
			return triggerToBuild.GetTriggerConfigured(trigActInfo);
		}

		//private void UpdateDevicesWatchedList(ITrigger trigger)
		//{
		//	if (trigger.DeviceId.HasValue && trigger.DeviceId.Value > 0)
		//	{
		//		if (!_watchedEventDeviceIds.Contains(trigger.DeviceId.Value))
		//		{
		//			_watchedEventDeviceIds.Add(trigger.DeviceId.Value);
		//		}
		//	}
		//}

		public bool GetCondition(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return false;
			return triggerToBuild.GetCondition(trigActInfo);
		}

		public void SetCondition(IPlugInAPI.strTrigActInfo trigActInfo, bool value)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return;
			triggerToBuild.SetCondition(trigActInfo, value);
		}

		public bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
		{
			return false;
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
