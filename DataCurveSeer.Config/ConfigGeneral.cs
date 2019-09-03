using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;
using Scheduler;

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
			throw new System.NotImplementedException();
		}

		public string PostBackProc(string page, string data, string user, int userRights)
		{
			throw new System.NotImplementedException();
		}
	}
}