using Hspi;

namespace HSPI_DataCurve
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Connector.Connect<HSPI>(args);
		}
	}
}