using System;
using System.Reflection;
using System.Text;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;
using Scheduler;

namespace DataCurveSeer.Config
{
	internal class ConfigAbout : PageBuilderAndMenu.clsPageBuilder, IConfigPage
	{
		private readonly string _pageName;
		private IHSApplication _hs;
		private string _pageNameText;

		public ConfigAbout(string pageName, IHSApplication hs, IIniSettings iniSettings, ILogging log):base(pageName)
		{
			_pageName = pageName;
			_hs = hs;
			_pageNameText = pageName.Replace("_", " ");
		}

		public new string PageName => _pageName;

		public string GetPagePlugin(string page, string user, int userRights, string queryString)
		{

			var returnString = new StringBuilder();
			reset();
			UsesJqAll = true;

			returnString.Append($"<title>About {Utility.PluginName} plugin</title>");
			returnString.Append(_hs.GetPageHeader(_pageName, Utility.PluginName, "", "", false, false));
			returnString.Append(DivStart("pluginpage", ""));

			returnString.Append("<br/>");
			returnString.Append("<br/>Big thank you to :");
			returnString.Append("<br/>");
			//returnString.Append("<br/>Kenneth at <a href=\"https://www.hjemmeautomasjon.no\" target=\"_blank\">www.hjemmeautomasjon.no</a>");

			returnString.Append("<br/>Moskus at <a href=\"https://www.hjemmeautomasjon.no\" target=\"_blank\">www.hjemmeautomasjon.no</a>");
			returnString.Append("<br/>");


			returnString.Append("<br/>");
			returnString.Append($"<br/>Guahtdim 2019 - {Utility.PluginName} version: " + Assembly.GetExecutingAssembly().GetName().Version);
			returnString.Append("<br/>");
			returnString.Append("<br/>Donations to paypal account oesolberg@hotmail.com ");
			returnString.Append("<br/>");
			returnString.Append("<br/>");
			returnString.Append("<br/>");
			var advancedText = new clsJQuery.jqSlidingTab("donations", PageName, false);
			advancedText.initiallyOpen = false;
			advancedText.tab.name = "Donations";
			advancedText.tab.tabName.Selected = "Donations";
			advancedText.tab.AddContent(CreateDonationsContent());
			returnString.Append(advancedText.Build());
			returnString.Append("<br/>");
			returnString.Append("<br/>");
			returnString.Append(DivEnd());
			AddFooter(_hs.GetPageFooter());
			suppressDefaultFooter = true;
			AddBody(returnString.ToString());
			return BuildPage();
		}

		private string CreateDonationsContent()
		{
			var returnString = new StringBuilder();
			returnString.Append("<p>Donations to paypal account oesolberg@hotmail.com</p> ");
			returnString.Append("<br/>");
			returnString.Append("<form action=\"https://www.paypal.com/cgi-bin/webscr\" method=\"post\" target=\"_blank\">");
			returnString.Append("<input type=\"hidden\" name=\"cmd\" value=\"_donations\" />");
			returnString.Append("<input type=\"hidden\" name=\"business\" value=\"AKZKJFMK3UEDC\" />");
			returnString.Append("<input type=\"hidden\" name=\"item_name\" value=\"Homeseer plugin development support\" />");
			returnString.Append("<input type=\"hidden\" name=\"currency_code\" value=\"NOK\" />");
			returnString.Append($"<input type=\"image\" src=\"/images/{Utility.PluginName}/btn_donateCC_LG.gif\" border=\"0\" name=\"submit\" title=\"PayPal - The safer, easier way to pay online!\" alt=\"Donate with PayPal button\" />");
			returnString.Append("&nbsp;&nbsp;&nbsp;&nbsp;Or scan QR &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
			returnString.Append($"<input type=\"image\" src=\"/images/{Utility.PluginName}/QR Code.png\" border=\"0\" name=\"submit\" title=\"PayPal - The safer, easier way to pay online!\" alt=\"Donate with PayPal button\" />");

			returnString.Append("<img alt=\"\" border=\"0\" src=\"https://www.paypal.com/en_NO/i/scr/pixel.gif\" width=\"1\" height=\"1\" />");
			returnString.Append("</form>");
			returnString.Append("<br/>");
			return returnString.ToString();
		}

		public string PostBackProc(string page, string data, string user, int userRights)
		{
			return string.Empty;
		}
	}
}