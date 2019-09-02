using System.Collections.Specialized;
using DataCurve.Common;
using DataCurve.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurve.TriggerHandling.Triggers
{
	public class DataCurveTrigger : ITrigger
	{
		private bool _isCondition;
		private IPlugInAPI.strTrigActInfo _triggerInfo;
		private IHsCollectionFactory _collectionFactory;
		private ILogging _logging;
		private DataCurveTriggerSettings _triggerSettings;
		private ReformatCopiedAction _reformatCopiedAction;

		public string GetTriggerName() => "DataCurveCondition";
		public int TriggerNumber { get; } = 1;
		public int UID { get; set; }
		public int EvRef { get; set; }
		public bool IsCondition => _isCondition;

		public DataCurveTrigger(ILogging logging,IHsCollectionFactory collectionFactory, IReformatCopiedAction reformatCopiedAction = null)
		{
			_collectionFactory = collectionFactory;
			_logging = logging;
			if (reformatCopiedAction == null)
			{
				_reformatCopiedAction = new ReformatCopiedAction(_logging);
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
			throw new System.NotImplementedException();
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
			return "jabba jabba";
		}

		private DataCurveTriggerSettings GetSettingsFromTriggerInfo(IPlugInAPI.strTrigActInfo triggerInfo)
		{
			
			_triggerInfo= triggerInfo;
			var formattedAction = _collectionFactory.GetActionsIfPossible(triggerInfo);

			if (formattedAction != null && formattedAction.Keys.Count > 0)
			{
				var uidAndEvRef = $"{triggerInfo.UID.ToString()}_{triggerInfo.evRef.ToString()}_"; ;
				//if (TriggerShouldBeUpdatedToNewVersion(formattedAction))
				//{
				//	formattedAction = UpdateFormattedAction(formattedAction, trigActInfo.UID, trigActInfo.evRef);
				//}

				formattedAction = _reformatCopiedAction.Run(formattedAction, triggerInfo.UID, triggerInfo.evRef);
				var triggerSettings = new DataCurveTriggerSettings();
				foreach (var dataKey in formattedAction.Keys)
				{

					//if (dataKey.Contains(Constants.GetSummaryTextParameter))
					//{
					//	triggerSettings.SearchText = (string)formattedAction[dataKey];
					//}

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
	}

	internal class DataCurveTriggerSettings
	{
	}
}