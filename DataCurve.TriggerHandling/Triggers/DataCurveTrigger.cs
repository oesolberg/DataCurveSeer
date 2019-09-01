using System.Collections.Specialized;
using DataCurve.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurve.TriggerHandling.Triggers
{
	public class DataCurveTrigger:ITrigger
	{
		public string GetTriggerName()=> "DataCurveCondition";
		public int TriggerNumber { get; } = 1;
		public int UID { get; set; }
		public int EvRef { get; set; }

		public string GetSubTriggerName(int subTriggerNumber)
		{
			return "";
		}

		public bool GetHasConditions()
		{
			return true;
		}

		public int GetSubTriggerCount()
		{
			return 0;
		}

		public bool ContainsSubTriggerActionNumber(int subTriggerNumber)
		{
			if (subTriggerNumber == -1) return true;
			return false;
		}

		//public IPlugInAPI.strTrigActInfo GetTriggerActionInfo()
		//{
		//	throw new System.NotImplementedException();
		//}

		public void AddSettingsFromTrigActionInfo(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			throw new System.NotImplementedException();
		}

		public IPlugInAPI.strTrigActInfo GetTriggerActionInfo()
		{
			throw new System.NotImplementedException();
		}

		public void SetCondition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
		{
			return;
		}

		public bool GetCondition(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return true;
		}


		public bool GetTriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return false;
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return false;
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData, IPlugInAPI.strTrigActInfo trigActInfo)
		{
			return new IPlugInAPI.strMultiReturn();
		}

		public string TriggerFormatUi(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return "This can never be a trigger, only a condition";
		}

		public string TriggerBuildUi(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
		{
			return "This can never be a trigger, only a condition";
		}

	
	}
}