using System;
using System.Collections.Specialized;
using System.ComponentModel;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurveSeer.TriggerHandling.Triggers
{
	public class DataCurveTrigger : ITrigger
	{
		private bool _isCondition;
		private IPlugInAPI.strTrigActInfo _triggerInfo;
		private IHsCollectionFactory _collectionFactory;
		private ILogging _logging;
		private DataCurveTriggerSettings _triggerSettings;
		private ReformatCopiedAction _reformatCopiedAction;
		private DataCurveTriggerUi _dataCurveUi;
		private IHomeSeerHandler _homeSeerHandler;
		private IHSApplication _hs;

		public string GetTriggerName() => Utility.PluginName + ": A data curve of device values ...";
		public int TriggerNumber { get; } = 1;
		public int UID { get; set; }
		public int EvRef { get; set; }
		public bool IsCondition => _isCondition;

		public int? DeviceId => _triggerSettings.DeviceIdChosen;

		public DataCurveTrigger(IHSApplication hs, ILogging logging, IHsCollectionFactory collectionFactory,
			IHomeSeerHandler homeSeerHandler, IReformatCopiedAction reformatCopiedAction = null,
			IDataCurveTriggerUi dataCurveUi = null)
		{
			_collectionFactory = collectionFactory;
			_logging = logging;
			_homeSeerHandler = homeSeerHandler;
			_hs = hs;
			if (reformatCopiedAction == null)
			{
				_reformatCopiedAction = new ReformatCopiedAction(_logging);
			}

			if (dataCurveUi == null)
			{
				_dataCurveUi = new DataCurveTriggerUi(_homeSeerHandler, _hs);
			}

		}

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

		public void AddSettingsFromTrigActionInfo(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			GetSettingsFromTriggerInfo(trigActInfo);
		}

		public IPlugInAPI.strTrigActInfo GetTriggerActionInfo()
		{
			throw new System.NotImplementedException();
		}

		public void SetCondition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
		{
			_isCondition = value;
		}

		public bool GetCondition(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return _isCondition;
		}

		public bool GetTriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
		{
			_triggerSettings = GetSettingsFromTriggerInfo(actionInfo);
			return _triggerSettings.GetTriggerConfigured();
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return false;
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData,
			IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var returnData = new IPlugInAPI.strMultiReturn();
			returnData.DataOut = trigActInfo.DataIn;
			returnData.TrigActInfo = trigActInfo;
			returnData.sResult = string.Empty;

			if (postData == null || postData.Count < 1) return returnData;

			object action = new Classes.action();
			var formattedAction = _collectionFactory.GetActionsIfPossible(trigActInfo);

			formattedAction = _reformatCopiedAction.Run(formattedAction, trigActInfo.UID, trigActInfo.evRef);

			postData.Add(Constants.TriggerTypeKey,
				this.ToString()); //this.GetType().Name);// typeof(this).Name;//this.ToString());

			postData.Add(Constants.IsConditionKey, _isCondition.ToString());

			foreach (string dataKey in postData)
			{
				if (string.IsNullOrEmpty(dataKey)) continue;

				var strippedKey = StripKeyOfUidStuff(dataKey);

				formattedAction.AddObject(strippedKey, (object) postData[dataKey]);
			}

			object objAction = (object) formattedAction;
			Utility.SerializeObject(ref objAction, ref returnData.DataOut);
			returnData.sResult = string.Empty;

			return returnData;
		}

		private string StripKeyOfUidStuff(string dataKey)
		{
			var indexOfUnderScore = dataKey.IndexOf('_', 2);
			if (indexOfUnderScore > 0)
			{
				var maxLength = GetLengthToUse(dataKey, indexOfUnderScore);
				dataKey = dataKey.Substring(0, maxLength);
			}

			return dataKey;
		}

		private int GetLengthToUse(string currentKey, int indexOfUnderScore)
		{
			var indexOfNextUnderscore = currentKey.IndexOf('_', indexOfUnderScore + 1);
			while (indexOfNextUnderscore > 0 && indexOfNextUnderscore == indexOfUnderScore + 1)
			{
				indexOfUnderScore = indexOfNextUnderscore;
				indexOfNextUnderscore = currentKey.IndexOf('_', indexOfUnderScore + 1);
			}

			return indexOfUnderScore + 1;
		}




		//Summary of format info on collapsed trigger/condition
		public string TriggerFormatUi(IPlugInAPI.strTrigActInfo actionInfo)
		{
			var infoInHeader = string.Empty;
			if (!_isCondition)
				return "This can never be a trigger, only a condition";
			if (!ChangeValueTrigger(actionInfo.evRef))
			{
				infoInHeader=
					"The initial trigger needs to be of type 'This device just had its value set or changed' or 'This device has a value that just changed:' to collect data properly<br/>";
			}
			_triggerSettings = GetSettingsFromTriggerInfo(actionInfo);
			var deviceInfo = GetDeviceInfoString();
			var ascDescCurve = GetAscendingDescendingCurveInfoString();
			var timespan = GetTimespanInfoString();
			return $"{infoInHeader}A data curve of device values for the device {deviceInfo} has had {ascDescCurve} curve for {timespan}";
		}

		private bool ChangeValueTrigger(int evRef)
		{
			return true;
			//return _homeSeerHandler.IsEventOfChangeValueType(evRef);
		}

		private string GetAscendingDescendingCurveInfoString()
		{
			if (_triggerSettings.AscendingOrDescending == AscDescEnum.Ascending)
				return "an ascending";
			return "a descending";
		}

		private string GetTimespanInfoString()
		{
			if (_triggerSettings.TimeSpanChosen.HasValue)
			{
				var hoursString=string.Empty;
				var minutesString = string.Empty;
				var andString = string.Empty;
				var timeSpanChosen = _triggerSettings.TimeSpanChosen.Value;
				if (timeSpanChosen.Hours > 0)
				{

					hoursString = $"{timeSpanChosen.Hours.ToString()} hour";
					if (timeSpanChosen.Hours > 1)
					{
						hoursString += "s";
					}
				}

				if (timeSpanChosen.Hours > 0 && timeSpanChosen.Minutes > 0)
				{
					andString = " and ";
				}

				if (timeSpanChosen.Minutes > 0)
				{
					minutesString = $"{timeSpanChosen.Minutes} minute";
					if (timeSpanChosen.Minutes > 1)
					{
						minutesString += "s";
					}
				}
				return $"the last {hoursString}{andString}{minutesString}";
			}

			return "an unknown time span (this should not happen)";
			
		}

		private string GetDeviceInfoString()
		{
			if (_triggerSettings.DeviceIdChosen.HasValue)
				return _homeSeerHandler.GetDeviceInfoString(_triggerSettings.DeviceIdChosen.Value);
			return "no device set";
		}

		//Trigger settings
		public string TriggerBuildUi(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
		{
			var uid = triggerInfo.UID.ToString();
			_triggerSettings = GetSettingsFromTriggerInfo(triggerInfo);

			if (!_isCondition)
				return "This can never be a trigger, only a condition";
			_triggerSettings.Uid = triggerInfo.UID.ToString();

			_triggerSettings.UniqueControllerId = uniqueControlId;
			return _dataCurveUi.Build(_triggerSettings);
		}

		private DataCurveTriggerSettings GetSettingsFromTriggerInfo(IPlugInAPI.strTrigActInfo triggerInfo)
		{

			_triggerInfo = triggerInfo;
			var formattedAction = _collectionFactory.GetActionsIfPossible(triggerInfo);

			if (formattedAction != null) //&& formattedAction.Keys.Count > 0)
			{
				var uidAndEvRef = $"{triggerInfo.UID.ToString()}_{triggerInfo.evRef.ToString()}_";
				formattedAction = _reformatCopiedAction.Run(formattedAction, triggerInfo.UID, triggerInfo.evRef);
				var triggerSettings = new DataCurveTriggerSettings();
				foreach (var dataKey in formattedAction.Keys)
				{

					if (dataKey.Contains(Constants.DeviceDropdownKey))
					{
						triggerSettings.DeviceIdChosen =
							ParameterExtraction.GetIntOrNullFromObject(formattedAction[dataKey]);
					}

					if (dataKey.Contains(Constants.RoomKey))
					{
						triggerSettings.RoomChosen = (string) (formattedAction[dataKey]);
					}

					if (dataKey.Contains(Constants.FloorKey))
					{
						triggerSettings.FloorChosen = (string) (formattedAction[dataKey]);
					}

					if (dataKey.Contains(Constants.IsConditionKey))
					{
						triggerSettings.IsCondition = ParameterExtraction.GetBoolFromObject(formattedAction[dataKey]);
					}

					if (dataKey.Contains(Constants.TimeSpanKey))
					{
						triggerSettings.TimeSpanChosen = ParameterExtraction.GetTimeSpanFromObject(formattedAction[dataKey]);
					}

					if (dataKey.Contains(Constants.AscDescKey))
					{
						triggerSettings.AscendingOrDescending = ParameterExtraction.GetAscDescEnumFromObject(formattedAction[dataKey]);
					}
				}

				_triggerSettings = triggerSettings;
				return triggerSettings;
			}

			return null;
		}

		private TimeSpan? GetTimeSpanFromObject(object o)
		{
			var timespanAsString = o as string;
			if (o != null)
			{
				if (!string.IsNullOrEmpty(timespanAsString))
				{
					return ConvertTimespanStringToTimespanOrNull();
				}
			}

			return null;
		}

		private TimeSpan? ConvertTimespanStringToTimespanOrNull()
		{
			return null;
		}

		

	}
}