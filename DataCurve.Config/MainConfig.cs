using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurve.Common;
using DataCurve.Common.Interfaces;
using HomeSeerAPI;

namespace DataCurve.Config
{
	public interface IMainConfig
	{
		void RegisterConfigs();
		string PostBackProc(string page, string data, string user, int userRights);
		string GetPagePlugin(string page, string user, int userRights, string queryString);
	}

	public class MainConfig : IMainConfig
	{
		private readonly ILogging _logging;
		private readonly IIniSettings _iniSettings;
		private readonly IAppCallbackAPI _callback;
		private readonly IMainPlugin _mainPlugin;
		private readonly IHSApplication _hs;
		private List<IConfigPage> _configPages;

		private const string DataCurveGeneralConfig = "DataCurve_General_Config";

		public MainConfig(ILogging logging, IHSApplication hs, IIniSettings iniSettings, IAppCallbackAPI callback, IMainPlugin mainPlugin)
		{
			_hs = hs;
			_logging = logging;
			_iniSettings = iniSettings;
			_callback = callback;
			_mainPlugin = mainPlugin;

		}
		public void RegisterConfigs()
		{
			var wpd = new WebPageDesc
			{
				link = DataCurveGeneralConfig,
				plugInName = Utility.PluginName
			};
			_callback.RegisterConfigLink(wpd);

			if (_configPages == null) _configPages = new List<IConfigPage>();
			_configPages.Add(CreateConfigPage(DataCurveGeneralConfig));
		}

		public string PostBackProc(string page, string data, string user, int userRights)
		{
			var selectedPage = FindSelectedPage(page);
			if (selectedPage != null)
				return selectedPage.PostBackProc(page, data, user, userRights);
			return "SOMETHING IS MISSING!!!";
		}

		private IConfigPage FindSelectedPage(string page)
		{
			IConfigPage toBeReturned = _configPages.FirstOrDefault(x => x.PageName == page);
			if (toBeReturned == null) Console.WriteLine($"Got a page name that does not exist '{page}'");

			return toBeReturned;
		}

		public string GetPagePlugin(string page, string user, int userRights, string queryString)
		{
			Console.WriteLine($"got call for page {page} from user {user} with user rights {userRights} and querystring {queryString}");
			var webPage = FindSelectedPage(page);
			return webPage.GetPagePlugin(page, user, userRights, queryString);
		}

		private IConfigPage CreateConfigPage(string pageName)
		{
			Scheduler.PageBuilderAndMenu.clsPageBuilder pageToRegister;
			switch (pageName)
			{
				case DataCurveGeneralConfig:
					pageToRegister = new ConfigGeneral(pageName, _hs, _iniSettings, _logging, _mainPlugin);
					break;
				//case GCalSeerAboutPage:
				//	pageToRegister = new ConfigAbout(pageName, _hs, _iniSettings, _log);
				//	break;
				default: throw new NotImplementedException($"Page {pageName} is not implemented");
			}

			_hs.RegisterPage(pageName, Utility.PluginName, Utility.InstanceFriendlyName);

			var linkText = pageName;
			linkText = linkText.Replace("GCalSeer_", "").Replace("_", " ").Replace(Utility.PluginName, "");
			var pageTitle = linkText;

			var webPageDescription = new WebPageDesc
			{
				plugInName = Utility.PluginName,
				link = pageName + Utility.InstanceFriendlyName,
				linktext = pageTitle,
				page_title = pageTitle + Utility.InstanceFriendlyName
			};

			_callback.RegisterLink(webPageDescription);
			return (IConfigPage)pageToRegister;
		}
	}
}
