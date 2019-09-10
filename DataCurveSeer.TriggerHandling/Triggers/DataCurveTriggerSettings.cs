using System;
using DataCurveSeer.Common;

namespace DataCurveSeer.TriggerHandling.Triggers
{
	internal class DataCurveTriggerSettings
	{
		public string FloorChosen { get; set; }
		public string RoomChosen { get; set; }
		public AscDescEnum AscendingOrDescending { get; set; }
		public string UidString { get; set; }

		public int UID { get; set; }
		public int EvRef { get; set; }

		public string UniqueControllerId { get; set; }
		public int? DeviceIdChosen { get; set; }
		public TimeSpan? TimeSpanChosen { get; set; }
		public bool IsCondition { get; set; }

		public bool GetTriggerConfigured()
		{
			if (TimeSpanChosen.HasValue && TimeSpanChosen.Value > new TimeSpan(0, 1, 0) &&
			    DeviceIdChosen.HasValue && DeviceIdChosen.Value > 0 && IsCondition)
				return true;
			return false;
		}
	}
}