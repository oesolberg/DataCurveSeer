using System;
using System.Collections.Generic;
using System.Text;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;
using Scheduler;
using System.Web;


namespace DataCurveSeer.Config
{
	public class ConfigGeneral: PageBuilderAndMenu.clsPageBuilder, IConfigPage
	{
		private readonly string _pageName;
		private readonly string _pageNameText;
		private IHSApplication _hs;
		private readonly ILogging _logging;
		private readonly IIniSettings _iniSettings;
		private readonly IMainPlugin _mainPlugin;

		private readonly string _idKey = "id";
		private string _googleCode;
		private string _msAppId;
		private const string AppIdAuthKey = "AppIdKey";
		private const string DoAuthMsKey = "AuthMsInputKey";
		private const string LogLevelKey = "LogLevel";
		private const string CalendarCheckIntervalKey = "CalendarCheckInterval";
		private const string DaysOfDataStorageKey = "DaysOfDataStorage_";
		private const string UseCalendarKey = "checkUse_";
		private const string UseMsCalendarKey = "checkUseMsCal_";
		private const string CheckCalendarButtonKey = "RefreshCalendar";

		private const string CodeKey = "CodeInputKey";
		private const string DoAuthKey = "AuthInputKey";
		private const string IdKey = "id";
		private const string AuthResultDiv = "authResult";


		public ConfigGeneral(string pageName, IHSApplication hs, IIniSettings iniSettings,
			ILogging log, IMainPlugin mainPlugin) : base(pageName)
		{
			_pageName = pageName;
			_pageNameText = pageName.Replace("_", " ");
			_hs = hs;
			_iniSettings = iniSettings;
			_logging = log;
			_mainPlugin = mainPlugin;

		}


		public new string PageName => _pageName;

		public string GetPagePlugin(string page, string user, int userRights, string queryString)
		{
			this.reset();
			var returnString = new StringBuilder();


			returnString.Append("<title>" + _pageNameText + "</title>");
			returnString.Append(_hs.GetPageHeader(_pageName, Utility.PluginName, "", "", false, false));
			//' a message area for error messages from jquery ajax post back (optional, only needed if using AJAX calls to get data)
			returnString.Append(PageBuilderAndMenu.clsPageBuilder.DivStart("pluginpage", ""));
			returnString.Append(PageBuilderAndMenu.clsPageBuilder.DivStart("errormessage", "class='errormessage'"));
			//returnString.Append(ShowMissingCredentialsErrorIfCredentialsMissing());
			returnString.Append(PageBuilderAndMenu.clsPageBuilder.DivEnd());

			returnString.Append(BuildContent());

			returnString.Append(PageBuilderAndMenu.clsPageBuilder.DivEnd());
			this.AddFooter(_hs.GetPageFooter());
			this.suppressDefaultFooter = true;
			this.AddBody(returnString.ToString());



			return this.BuildPage();

		}

		private string BuildContent()
		{
			var returnString = new StringBuilder();

			returnString.Append("<strong><div id=\'message\'>&nbsp;</div></strong><br/>");
			returnString.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>");
			returnString.Append("  <tr class='tableheader'><td width='250'>" + _pageNameText + "</td><td width='750'>" +
								$"General settings for {Utility.PluginName}" + "</td></tr>");

			//Number of days to keep data
			returnString.Append("  <tr class='tableroweven'><td>Number of days to keep data for devices used in triggers:</td><td>" +
								SetDaysOfStorage() + "</td></tr>");

			//time between calendar checks
			//returnString.Append("  <tr class='tableroweven'><td>Time between checks of calendars:</td><td>" +
			//					SetTimeBetweenCalendarChecks() + "</td></tr>");

			//Set log level
			returnString.Append("  <tr class='tablerowodd'><td>Log level:</td><td>" + SetLogLevelUserInterface() +
								"</td></tr>");

			returnString.AppendLine(CreateInfoContent());
			returnString.Append("</td></tr>");
			returnString.Append(" </table>");

			returnString.Append("<br/><br/>");

			return returnString.ToString();

		}

		private string CreateInfoContent()
		{
			return string.Empty;
			//var returnString = new StringBuilder();
			//returnString.Append("  <tr class='tableheader'><td width='250'>Info</td><td></td></tr>");
			//returnString.Append("  <tr class='tablerowodd'><td class='tablecell'>Number of heat pumps to check:</td><td class='tablecell'><a href='" + ConfigConstants.DaikinSeerTcpIpConfig + "'>" + _iniSettings.GetHeatPumpAddresses.Count +
			//					"</a></td></tr>");
			//return returnString.ToString();
		}

        private string SetDaysOfStorage()
        {
            var textBoxDaysOfDataStorage= new clsJQuery.jqTextBox(DaysOfDataStorageKey,"text", "", _pageName,5, false);
            var currentDaysOfDataStorage = _iniSettings.DaysOfDataStorage;
            if(currentDaysOfDataStorage>0)
                textBoxDaysOfDataStorage.defaultText = currentDaysOfDataStorage.ToString();
            return textBoxDaysOfDataStorage.Build();
        }


        private string SetLogLevelUserInterface()
		{
			var logLevelDropdown = new clsJQuery.jqDropList(LogLevelKey, _pageName, false);
			logLevelDropdown.items = new List<Pair>()
			{
				new Pair() {Name = "None", Value = "0",},
				new Pair() {Name = "Normal", Value = "1"},
				new Pair() {Name = "Debug", Value = "2"},
				new Pair() {Name = "Debug to file", Value = "3"},
				new Pair() {Name = "Debug to file and log", Value = "4"}
			};
			var iniSettingsLogLevel = _iniSettings.LogLevel;
			logLevelDropdown.selectedItemIndex = iniSettingsLogLevel.ToInt();
			return logLevelDropdown.Build();
		}


		public string PostBackProc(string page, string data, string user, int userRights)
		{
			//ShowMissingCredentialsErrorIfCredentialsMissing();

			Dictionary<string, string> dicQueryString = SplitDataString(data);

			if (dicQueryString.ContainsKey(_idKey))
			{
				var configUnit = dicQueryString[_idKey];
				switch (configUnit)
				{
					case LogLevelKey:
						HandleLogLevelDropDown(configUnit, dicQueryString[configUnit]);
						break;
					case DoAuthKey:
						//HandleDoAuthentication(configUnit, dicQueryString[configUnit]);
						break;
					case DoAuthMsKey:
						//HandleDoMsAuthentication(configUnit, dicQueryString[configUnit]);
						break;

				}

				if (configUnit.Contains(UseCalendarKey))
				{
					//HandleChangeOfChosenGCalendars(dicQueryString);
				}

				if (configUnit.Contains(UseMsCalendarKey))
				{
					//HandleChangeOfChosenMsCalendars(dicQueryString);
				}

				if (configUnit.Contains(CheckCalendarButtonKey))
				{
					HandleUpdateOfCalendar();
				}
			}
			else if (dicQueryString.ContainsKey(CalendarCheckIntervalKey))
			{
				HandleCalendarCheckIntervalChange(dicQueryString);
			}
			else if (dicQueryString.ContainsKey(DaysOfDataStorageKey))
			{
                HandleDaysOfDataStorageChange(dicQueryString);
			}
			else if (dicQueryString.ContainsKey(CodeKey))
			{
				HandleInputOfCodeKey(dicQueryString);
			}
			else if (dicQueryString.ContainsKey(AppIdAuthKey))
			{
				HandleInputOfMsAppId(dicQueryString);
			}

			return base.postBackProc(page, data, user, userRights);

		}

        private void HandleDaysOfDataStorageChange(Dictionary<string, string> dicQueryString)
        {
            //Get and store the number of days to store data. Minimum=1
        }

        private void HandleInputOfCodeKey(Dictionary<string, string> dicQueryString)
		{
			_googleCode = HttpUtility.UrlDecode(dicQueryString[CodeKey]);
		}

		private void HandleInputOfMsAppId(Dictionary<string, string> dicQueryString)
		{
			_msAppId = HttpUtility.UrlDecode(dicQueryString[AppIdAuthKey]);

			//_iniSettings.MsAppId = _msAppId;
		}

		private void HandleUpdateOfCalendar()
		{
			this.pageCommands.Add("refresh", "true");
		}


		private bool IsChecked(string checkedString)
		{
			if (checkedString == "checked") return true;
			return false;
		}

		private void HandleCalendarCheckIntervalChange(Dictionary<string, string> dicQuerystring)
		{
			var timeString = dicQuerystring[CalendarCheckIntervalKey];
			var timespan = GetTimespanFromTimestring(timeString);
			//_iniSettings.CalendarCheckInterval = timespan;
		}

		//private void HandleTriggerCheckIntervalChange(Dictionary<string, string> dicQuerystring)
		//{
		//	var timeString = dicQuerystring[DaysOfDataStorageKey];
		//	var timespan = GetTimespanFromTimestring(timeString);
		//	_iniSettings.CheckHeatPumpTimerInterval = (int)timespan.TotalSeconds;
		//}


		private TimeSpan GetTimespanFromTimestring(string timeString)
		{
			var resultingTimeSpan = new TimeSpan(0, 0, 1, 0);
			//Only minutes and seconds
			if (!string.IsNullOrWhiteSpace(timeString) && timeString.Length >= 3)
			{
				var splitOnColon = timeString.Split(':');

				var numberOfMinutes = int.Parse(splitOnColon[0]);
				var numberOfSeconds = int.Parse(splitOnColon[1]);

				resultingTimeSpan = new TimeSpan(0, 0, numberOfMinutes, numberOfSeconds);
				if (resultingTimeSpan.TotalSeconds >= 3600)
				{
					resultingTimeSpan = new TimeSpan(0, 0, 59, 59);
				}
			}

			return resultingTimeSpan;
		}

		private void PostError(string message)
		{
			this.divToUpdate.Add("errormessage", message);
		}

		private void HandleLogLevelDropDown(string configUnit, string chosenNumber)
		{
			var newLoglevel = (LogLevel)Enum.Parse(typeof(LogLevel), chosenNumber);
			_iniSettings.LogLevel = newLoglevel;
		}

		private Dictionary<string, string> SplitDataString(string data)
		{
			var returnDictionary = new Dictionary<string, string>();
			var splitByAmper = data.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var nameValuePair in splitByAmper)
			{
				var splitByEqual = nameValuePair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				if (splitByEqual.Length > 1)
				{
					returnDictionary.Add(splitByEqual[0], splitByEqual[1]);
				}

				if (splitByEqual.Length == 1)
				{
					returnDictionary.Add(splitByEqual[0], string.Empty);
				}
			}

			return returnDictionary;
		}


		private void ShowAuthResult(string message)
		{
			this.divToUpdate.Add(AuthResultDiv, message);
		}

	}
}