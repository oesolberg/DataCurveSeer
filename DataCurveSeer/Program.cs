using Hspi;

namespace HSPI_DataCurveSeer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Connector.Connect<HSPI>(args);
		}
	}
}