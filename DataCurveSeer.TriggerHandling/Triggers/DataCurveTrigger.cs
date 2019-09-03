using System;
using System.Collections.Specialized;
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

		public DataCurveTrigger(IHSApplication hs,ILogging logging, IHsCollectionFactory collectionFactory, 
			IHomeSeerHandler homeSeerHandler, IReformatCopiedAction reformatCopiedAction = null, IDataCurveTriggerUi dataCurveUi = null)
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
				_dataCurveUi = new DataCurveTriggerUi(_homeSeerHandler,_hs);
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
			return false;
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo)
		{
			return false;
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData, IPlugInAPI.strTrigActInfo trigActInfo)
		{
			var returnData = new IPlugInAPI.strMultiReturn();
			returnData.DataOut = trigActInfo.DataIn;
			returnData.TrigActInfo = trigActInfo;
			returnData.sResult = string.Empty;

			if (postData == null || postData.Count < 1) return returnData;

			object action = new Classes.action();
			var formattedAction = _collectionFactory.GetActionsIfPossible(trigActInfo);

			formattedAction = _reformatCopiedAction.Run(formattedAction, trigActInfo.UID, trigActInfo.evRef);
			
			postData.Add(Constants.TriggerTypeKey, this.ToString());//this.GetType().Name);// typeof(this).Name;//this.ToString());

			postData.Add(Constants.IsConditionKey, _isCondition.ToString());

			foreach (string dataKey in postData)
			{
				if (string.IsNullOrEmpty(dataKey)) continue;

				//if (IsAction(dataKey, dataKey))
				//{
				//	var res = DoActionBasedOnDataKey(dataKey, postData, formattedAction);
				//	if (res != null && res.ShouldBeStored)
				//	{
				//		formattedAction.AddObject(res.DataKey, (object)res.ObjectToStore);
				//		formattedAction.AddObject(Constants.RegExpHandingErrorMessageParameter, (object)"");
				//	}
				//	else if (res != null && res.Success == false && !string.IsNullOrEmpty(res.ErrorMessage))
				//	{
				//		formattedAction.AddObject(Constants.RegExpHandingErrorMessageParameter, (object)res.ErrorMessage);
				//	}
				//	continue;
				//}

				//if (IsNotToBeStored(dataKey))
				//{
				//	continue;
				//}

				var strippedKey = StripKeyOfUidStuff(dataKey);
				
				formattedAction.AddObject(strippedKey, (object)postData[dataKey]);
			}
			object objAction = (object)formattedAction;
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
			if (!_isCondition)
				return "This can never be a trigger, only a condition";
			return "Trigger configuration going on";
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

			if (formattedAction != null)//&& formattedAction.Keys.Count > 0)
			{
				var uidAndEvRef = $"{triggerInfo.UID.ToString()}_{triggerInfo.evRef.ToString()}_";
				//if (TriggerShouldBeUpdatedToNewVersion(formattedAction))
				//{
				//	formattedAction = UpdateFormattedAction(formattedAction, trigActInfo.UID, trigActInfo.evRef);
				//}

				formattedAction = _reformatCopiedAction.Run(formattedAction, triggerInfo.UID, triggerInfo.evRef);
				var triggerSettings = new DataCurveTriggerSettings();
				foreach (var dataKey in formattedAction.Keys)
				{

					if (dataKey.Contains(Constants.DeviceDropdownKey))
					{
						triggerSettings.DeviceIdChosen = GetIntOrNullFromObject(formattedAction[dataKey]);
					}

					//if (dataKey.Contains(Constants.IsConditionKey))
					//{
					//	_isCondition = false;
					//	var boolAsString = (string)formattedAction[dataKey];
					//	if (boolAsString.ToUpper() == true.ToString().ToUpper())
					//	{
					//		_isCondition = true;
					//	}
					//}

					//if (dataKey.StartsWith(Constants.CalendarCheckboxKey))
					//{
					//	var calendarStatus = (string)formattedAction[dataKey];
					//	if (calendarStatus == Constants.CheckedCalendar)
					//	{
					//		triggerSettings.CheckedCalendars.Add(GetCalendarHashCode(dataKey, uidAndEvRef));
					//	}
					//}

					//if (dataKey.Contains(Constants.AdvancedMenuScrollingParameter))
					//{
					//	var actionString = (string)formattedAction[dataKey];
					//	if (actionString.Contains("_open"))
					//	{
					//		triggerSettings.ScrollerOpen = true;
					//	}

					//}
					//if (dataKey.Contains(Constants.TriggerDropdownParameter))
					//{
					//	var triggerChoiceString = (string)formattedAction[dataKey];
					//	if (int.TryParse(triggerChoiceString, out var triggerChoice))
					//	{
					//		triggerSettings.OccurenceType = triggerChoice;
					//	}
					//}
					//if (dataKey.Contains(Constants.TextOrRegDropdownId))
					//{
					//	var textOrRegDropdownChoiceString = (string)formattedAction[dataKey];
					//	if (int.TryParse(textOrRegDropdownChoiceString, out var triggerChoice))
					//	{
					//		triggerSettings.UseRegularExpression = (triggerChoice == 2);
					//	}
					//}

					//if (dataKey.Contains(Constants.WithinTimeUnitDropDownParameter))
					//{
					//	var triggerChoiceString = (string)formattedAction[dataKey];
					//	if (int.TryParse(triggerChoiceString, out var triggerChoice))
					//	{
					//		triggerSettings.DuringTimeUnit = triggerChoice;
					//	}
					//}

					//if (dataKey.Contains(Constants.WithinNumberTextParameter))
					//{
					//	var triggerChoiceString = (string)formattedAction[dataKey];
					//	if (int.TryParse(triggerChoiceString, out var triggerChoice))
					//	{
					//		triggerSettings.DuringNumber = triggerChoice;
					//	}
					//}
					//if (dataKey.Contains(Constants.WithingPeriodeFromNowUnitParameter))
					//{
					//	var triggerChoiceString = (string)formattedAction[dataKey];
					//	if (int.TryParse(triggerChoiceString, out var triggerChoice))
					//	{
					//		triggerSettings.DuringPeriodeFromNow = triggerChoice;
					//	}
					//}
					//if (dataKey.Contains(Constants.StartOfOccurenceParameter))
					//{
					//	var timespanStartString = (string)formattedAction[dataKey];
					//	if (TryParseTimespanFromHS(timespanStartString, out var timespanStart))
					//	{
					//		triggerSettings.BetweenStartOfOccurence = timespanStart;
					//	}
					//}
					//if (dataKey.Contains(Constants.EndOfOccurenceParameter))
					//{
					//	var timespanEndString = (string)formattedAction[dataKey];
					//	if (TryParseTimespanFromHS(timespanEndString, out var timespanEnd))
					//	{
					//		triggerSettings.BetweenEndOfOccurence = timespanEnd;
					//	}
					//}

					//if (dataKey.StartsWith(Constants.CheckIfUseOffsetParameter))
					//{
					//	triggerSettings.UseOffset = ParameterExtraction.GetBoolValue(dataKey, formattedAction);
					//}

					//if (dataKey.StartsWith(Constants.RegexpOrTagDropDown))
					//{
					//	triggerSettings.UnitDataSearchMethod = (SearchMethod)ParameterExtraction.GetIntValue(dataKey, formattedAction);
					//}

					//if (dataKey.StartsWith(Constants.CheckDoNotTriggerOffsetIfOverlappingParameter))
					//{
					//	triggerSettings.OverlappingOffset = ParameterExtraction.GetBoolValue(dataKey, formattedAction);
					//}

					//if (dataKey.Contains(Constants.BeforeAfterOffsetParameter))
					//{
					//	var triggerChoiceString = (string)formattedAction[dataKey];
					//	if (int.TryParse(triggerChoiceString, out var triggerChoice))
					//	{
					//		triggerSettings.OffsetBeforeOrAfter = triggerChoice;
					//	}
					//}

					//if (dataKey.Contains(Constants.OffsetTimespanParameter))
					//{
					//	var offsetTimespanString = (string)formattedAction[dataKey];
					//	if (TimeSpan.TryParse(offsetTimespanString, out var offsetTimeSpan))
					//	{
					//		triggerSettings.OffsetTimespan = offsetTimeSpan;
					//	}
					//}

					//if (dataKey.StartsWith(Constants.CheckIfUseHandlingParameter))
					//{
					//	triggerSettings.UseRegExpHandling = ParameterExtraction.GetBoolValue(dataKey, formattedAction);
					//}

					////if (dataKey.Contains(Constants.RegExpHandlingUnitDataListParameter))
					////{
					////	settings.RegExpHandlingUnits = (List<RegExpHandlingUnitData>)formattedAction[dataKey];
					////}

					//if (dataKey.Contains(EventUi.Constants.RegExpHandlingUnitDataListParameter))
					//{
					//	triggerSettings.RegExpHandlingUnits = (List<RegExpHandlingUnitData>)formattedAction[dataKey];
					//}

					//if (dataKey.Contains(Constants.RegExpHandingErrorMessageParameter))
					//{
					//	triggerSettings.ErrorMessage = (string)formattedAction[dataKey];
					//}
					//if (dataKey.Contains(Constants.IsEditingIdParameter))
					//{
					//	triggerSettings.IsEditingId = (int)formattedAction[dataKey];
					//}

					//if (dataKey.StartsWith(Constants.ControlDeviceParameter))
					//{
					//	triggerSettings.CurrentControlUnitEditing = ParameterExtraction.GetIntValue(dataKey, formattedAction);
					//}
				}
				_triggerSettings = triggerSettings;
				return triggerSettings;
			}
			return null;
		}

		private int? GetIntOrNullFromObject(object obj)
		{
			var intString = obj as string;
			if (intString != null)
			{
				int chosenDeviceId = -1;
				if (int.TryParse(intString, out chosenDeviceId))
				{
					return chosenDeviceId;
				}
			}

			return null;
		}
	}

	internal class DataCurveTriggerSettings
	{
		public string FloorChosen { get; set; }
		public string RoomChosen { get; set; }
		public AscDescEnum AscendingOrDescending { get; set; }
		public string Uid { get; set; }
		public string UniqueControllerId { get; set; }
		public int? DeviceIdChosen { get; set; }
		public TimeSpan? TimeSpanChosen { get; set; }
	}

	internal enum AscDescEnum
	{
		Ascending = 1,
		Descending = 2
	}
}