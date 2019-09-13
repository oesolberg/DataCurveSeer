using System;
using DataCurveSeer.Common;
using Hspi;

namespace HSPI_DataCurveSeer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine(Utility.PluginName);
			Connector.Connect<HSPI>(args);
		}
	}
}