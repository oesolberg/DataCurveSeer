﻿using System.Collections.Specialized;
using HomeSeerAPI;

namespace DataCurveSeer.Common.Interfaces
{
	public interface ITriggerHandler
	{

		
		bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo actionInfo, int deviceId);

		void SetCondition(IPlugInAPI.strTrigActInfo actionInfo, bool value);
		bool GetCondition(IPlugInAPI.strTrigActInfo actionInfo);

		string GetTriggerName( int triggerNumber);
		bool GetTriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo);


		string GetSubTriggerName(int triggerNumber, int subTriggerNumber);

		bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo);
		IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData, IPlugInAPI.strTrigActInfo trigActInfo);
		string TriggerFormatUi(IPlugInAPI.strTrigActInfo actionInfo);
		string TriggerBuildUi(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo);
		bool GetHasConditions(int triggerNumber);
		int GetSubTriggerCount(int triggerNumber);
		int GetTriggerCount();
		bool GetHasTriggers();
		bool IsWatchedDeviceId(int deviceId);
	}
}