using System;

namespace DataCurveSeer.Common
{
	
		public delegate void DeviceIdSetEventHandler(Object sender, EventArgs eventArgs);

		public class DelegateDummy
		{

		}

		public class DeviceIdEventArgs : EventArgs
		{
			public int DeviceId { get; set; }
		}
	
}