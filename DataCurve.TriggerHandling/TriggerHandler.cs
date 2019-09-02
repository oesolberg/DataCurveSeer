using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurve.Common;
using DataCurve.Common.Interfaces;
using DataCurve.TriggerHandling.Triggers;
using HomeSeerAPI;

namespace DataCurve.TriggerHandling
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

		protected internal const string TriggerTypeKey = "TriggerType";

		public TriggerHandler(IHSApplication hs, IAppCallbackAPI callback, IIniSettings iniSettings, ILogging logging, IHsCollectionFactory collectionFactory)
		{
			_hs = hs;
			_callback = callback;
			_iniSettings = iniSettings;
			_logging = logging;
			_collectionFactory = collectionFactory;
			_triggerTypes = CreateTriggerTypes();
			GetPluginTriggersFromHomeSeer();


		}

		private List<ITrigger> CreateTriggerTypes()
		{
			var triggers = new List<ITrigger>();
			triggers.Add(new DataCurveTrigger(_logging,_collectionFactory));
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
			var triggerToAdd = TriggerFactory.Get(triggerType, _logging, this, _callback, _collectionFactory);
			if (triggerToAdd != null)
			{
				triggerToAdd.AddSettingsFromTrigActionInfo(trigActInfo);
				AddOrUpdatedToRunningTriggers(triggerToAdd);
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
			return triggerToBuild.TriggerProcessPostUi(postData, trigActInfo);
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return false;
			return triggerToBuild.TriggerTrue(trigActInfo);
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
			return triggerToBuild.GetTriggerConfigured(trigActInfo);
		}

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
