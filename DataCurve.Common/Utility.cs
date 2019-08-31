using System.IO;

namespace DataCurve.Common
{
	public class Utility
	{
		public const string PluginName = "DataCurve";
		public const string IniFile = PluginName+".ini";
		public static string ExePath = Directory.GetCurrentDirectory();
		public static string InstanceFriendlyName => "";

		public static bool StringToBool(string stringToCheck)
		{
			bool returnValue = !string.IsNullOrEmpty(stringToCheck) && stringToCheck.Trim().ToUpper() == "TRUE";
			return returnValue;
		}
	}
}