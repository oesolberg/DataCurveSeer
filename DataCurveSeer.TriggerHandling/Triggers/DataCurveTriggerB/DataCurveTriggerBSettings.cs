using System;
using DataCurveSeer.Common;

namespace DataCurveSeer.TriggerHandling.Triggers.DataCurveTriggerB
{
	internal class DataCurveTriggerBSettings
	{
		public string FloorChosen { get; set; }
		public string RoomChosen { get; set; }
		public AscDescEnum AscendingOrDescending { get; set; }
		public string UidString { get; set; }

		public int UID { get; set; }
		public int EvRef { get; set; }

		public string UniqueControllerId { get; set; }
		public int? DeviceIdChosen { get; set; }
		public bool IsCondition { get; set; }
		public double? ThresholdValue { get; set; }
        public int? NumberOfLastMeasurements { get; set; }

		public bool GetTriggerConfigured()
		{
			if (ThresholdValue.HasValue &&
                NumberOfLastMeasurements.HasValue && NumberOfLastMeasurements.Value>1 &&
				DeviceIdChosen.HasValue && DeviceIdChosen.Value > 0 && IsCondition)
				return true;
			return false;
		}
	}
}