using System;

namespace DataCurveSeer.TriggerHandling.Triggers
{
	internal class DataCurveTriggerSettings
	{
		public string FloorChosen { get; set; }
		public string RoomChosen { get; set; }
		public AscDescEnum AscendingOrDescending { get; set; }
		public string Uid { get; set; }
		public string UniqueControllerId { get; set; }
		public int? DeviceIdChosen { get; set; }
		public TimeSpan? TimeSpanChosen { get; set; }
		public bool IsCondition { get; set; }
	}
}