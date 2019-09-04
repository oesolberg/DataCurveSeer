using System;

namespace DataCurveSeer.TriggerHandling
{
	internal class ParameterExtraction
	{
		public static bool? GetBoolOrNullFromObject(object o)
		{
			var boolAsString = o as string;
			if (!string.IsNullOrEmpty(boolAsString) && (boolAsString.ToLower() == "true" || boolAsString.ToLower() == "false"))
			{
				if (boolAsString.ToLower() == "true")
					return true;
				return false;
			}
			return null;
		}

		public static bool GetBoolFromObject(object o)
		{
			var result = GetBoolOrNullFromObject(o);
			if (result.HasValue && result.Value)
				return true;
			return false;
		}

		public static int? GetIntOrNullFromObject(object obj)
		{
			var intString = obj as string;
			if (intString != null)
			{
				int chosenDeviceId;
				if (int.TryParse(intString, out chosenDeviceId))
				{
					return chosenDeviceId;
				}
			}
			return null;
		}

		public static AscDescEnum GetAscDescEnumFromObject(object o)
		{
			var ascDescEnumAsString = o as string;
			if (o != null)
			{
				AscDescEnum ascDescEnumResult;
				if (Enum.TryParse(ascDescEnumAsString, true, out ascDescEnumResult))
					return ascDescEnumResult;
			}

			return AscDescEnum.Ascending;
		}

		public static TimeSpan? GetTimeSpanFromObject(object o)
		{
			//expected format for timespan dd.hh:mm:ss (0.1:1:0=>1 hour 1 minute)
			var timespanAsString = o as string;
			if (!string.IsNullOrEmpty(timespanAsString))
			{
				var daysAndTimeSplit = timespanAsString.Split('.');
				if (daysAndTimeSplit.Length == 2)
				{
					var hoursMinutesAndSecondsSplit = daysAndTimeSplit[1].Split(':');
					if (hoursMinutesAndSecondsSplit.Length == 3)
					{
						var daysAsString = daysAndTimeSplit[0];
						var hoursAsString = hoursMinutesAndSecondsSplit[0];
						var minutesAsString = hoursMinutesAndSecondsSplit[1];
						var secondsAsString = hoursMinutesAndSecondsSplit[2];
						int days;
						int hours;
						int minutes;
						int seconds;
						if (!int.TryParse(daysAsString, out days))
						{
							return null;
						}
						if (!int.TryParse(hoursAsString, out hours))
						{
							return null;
						}
						if (!int.TryParse(minutesAsString, out minutes))
						{
							return null;
						}
						if (!int.TryParse(secondsAsString, out seconds))
						{
							return null;
						}
						return new TimeSpan(days,hours,minutes,seconds);
					}
				}
			}

			return null;
		}
	}
}