using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurve.Common;
using DataCurve.Common.Interfaces;
using HomeSeerAPI;
using Scheduler.Classes;

namespace DataCurve.HomeSeerHandling
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
