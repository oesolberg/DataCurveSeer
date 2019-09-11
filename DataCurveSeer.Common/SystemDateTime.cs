using System;

namespace DataCurveSeer.Common
{
	public class SystemDateTime
	{
		public static Func<DateTime> Now = () => DateTime.Now;

		public static void ResetDateTime()
		{
			Now = () => DateTime.Now;
		}

	}
}