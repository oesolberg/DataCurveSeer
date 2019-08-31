using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurve.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurve.TriggerHandling
{

	public class TriggerHandler : ITriggerHandler
	{
		private List<ITrigger> _triggers;

		public bool ContainsSubTriggerActionNumber(int subTriggerActionNumber)
		{
			throw new NotImplementedException();
		}

		public IPlugInAPI.strTrigActInfo GetTriggerActionInfo()
		{
			throw new NotImplementedException();
		}

		public string GetSubTriggerName(int triggerNumber, int subTriggerNumber)
		{
			return "";
		}

		public int SubTriggerCount(int triggerNumber)
		{
			return 0;
		}

		public bool GetHasConditions(int triggerNumber)
		{
			var trigger = FindTrigger(triggerNumber);
			return trigger.GetHasConditions();
		}

		private ITrigger FindTrigger(int triggerNumber)
		{
			return _triggers.SingleOrDefault(x => x.TriggerNumber == triggerNumber);
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

			var foundTrigger = _triggers.SingleOrDefault(x => x.TriggerNumber == actionInfo.TANumber
			                                                          && x.ContainsSubTriggerActionNumber(actionInfo.SubTANumber));
			return foundTrigger;
		}


		public int GetSubTriggerCount(int triggerNumber)
		{
			return 0;
		}

		public int GetTriggerCount()
		{
			return 1;
		}

		public bool GetHasTriggers()
		{
			return  true;
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
			return triggerToBuild.TriggerProcessPostUi(postData,trigActInfo);
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var triggerToBuild = FindTrigger(trigActInfo);
			if (triggerToBuild == null) return false;
			return triggerToBuild.TriggerTrue(trigActInfo);
		}

		string ITriggerHandler.GetTriggerName(int triggerNumber)
		{
			var triggerToBuild = FindTrigger(triggerNumber);
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
			triggerToBuild.SetCondition(trigActInfo,value);
		}

		public void AddSettingsFromTrigActionInfo(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			throw new NotImplementedException();
		}

		public bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId)
		{
			return false;
		}
	}
}
