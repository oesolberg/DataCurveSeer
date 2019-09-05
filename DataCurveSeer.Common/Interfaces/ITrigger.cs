using System.Collections.Specialized;
using HomeSeerAPI;

namespace DataCurveSeer.Common.Interfaces
{
	public interface ITrigger
	{
		int TriggerNumber { get; }

		int UID { get; set; }

		int EvRef { get; set; }
		bool IsCondition { get; }

		//bool ContainsSubTriggerActionNumber(int subTriggerActionNumber);
		void AddSettingsFromTrigActionInfo(IPlugInAPI.strTrigActInfo trigActInfo);
		IPlugInAPI.strTrigActInfo GetTriggerActionInfo();

		void SetCondition(IPlugInAPI.strTrigActInfo actionInfo, bool value);
		bool GetCondition(IPlugInAPI.strTrigActInfo actionInfo);

		string GetTriggerName();
		bool GetTriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo);


		string GetSubTriggerName(int subTriggerNumber);

		bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo);
		IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData, IPlugInAPI.strTrigActInfo trigActInfo);
		string TriggerFormatUi(IPlugInAPI.strTrigActInfo actionInfo);
		string TriggerBuildUi(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo);
		bool GetHasConditions();
		int GetSubTriggerCount();
		int? DeviceId { get; }
		bool ContainsSubTriggerActionNumber(int actionInfoSubTaNumber);
	}
}