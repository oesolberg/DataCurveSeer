using System.Collections.Generic;
using Scheduler.Classes;

namespace DataCurveSeer.Common.Interfaces
{
	public interface IHomeSeerHandler
	{
		FloorsRoomsAndDevices GetFloorsRoomsAndDevices();
		string GetDeviceInfoString(int triggerSettingsDeviceIdChosen);
		bool IsEventOfChangeValueType(int evRef);
	}


}