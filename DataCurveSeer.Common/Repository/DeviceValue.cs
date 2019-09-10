using System;

namespace DataCurveSeer.Common.Repository
{
	public class DeviceValue
	{
		public int Id { get; set; }
		public int DeviceId { get; set; }
		public double Value { get; set; }
		public DateTime DateTimeOfMesurment { get; set; }
	}
}