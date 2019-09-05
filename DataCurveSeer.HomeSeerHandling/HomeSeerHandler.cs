using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;
using Scheduler.Classes;

namespace DataCurveSeer.HomeSeerHandling
{
    public class HomeSeerHandler: IHomeSeerHandler
	{
		private IHSApplication _hs;
		private ILogging _logging;

		public HomeSeerHandler(IHSApplication hs, ILogging logging)
		{
			_hs = hs;
			_logging = logging;
		}

		public FloorsRoomsAndDevices GetFloorsRoomsAndDevices()
		{
			
			return GetDevicesFloorsAndRooms();
			
		}

		public string GetDeviceInfoString(int deviceRef)
		{
			var foundDevice = (DeviceClass)_hs.GetDeviceByRef(deviceRef);
			if (foundDevice != null)
			{
				var deviceName = foundDevice.get_Name(_hs);
				var floor = foundDevice.get_Location2(_hs);
				var room = foundDevice.get_Location(_hs);
				return $"{floor} {room} {deviceName}";
			}
			return "";
		}

		public bool IsEventOfChangeValueType(int evRef)
		{

			var eventData = _hs.Event_Info(4);
			var eventTypeTrigger = eventData.Trigger_Groups[0].Triggers[0];
			if (eventTypeTrigger.StartsWith("TYPE_VALUE_CHANGE : A Device's Value is..."))
				return true;
			return false;
		}

		public void GetEventInfo()
		{
		}

		public FloorsRoomsAndDevices  GetDevicesFloorsAndRooms()
		{
			var deviceList = new List<DeviceFromHomeSeer>();
			var deviceEnumerator = (clsDeviceEnumeration)_hs.GetDeviceEnumerator();
			while (!deviceEnumerator.Finished)
			{
				var foundDevice = deviceEnumerator.GetNext();
				deviceList.Add(new DeviceFromHomeSeer(){DeviceName =foundDevice.get_Name(_hs) ,DeviceId = foundDevice.get_Ref(_hs),FloorName = foundDevice.get_Location2(_hs),RoomName = foundDevice.get_Location(_hs)});
			}

			var floors = deviceList.Select(x => x.FloorName).Distinct().ToList();
			var rooms = deviceList.Select(x => x.RoomName).Distinct().ToList();
			return new FloorsRoomsAndDevices()
			{
				Devices = deviceList,
				Floors = floors,
				Rooms = rooms
			};
		}
	}
}
