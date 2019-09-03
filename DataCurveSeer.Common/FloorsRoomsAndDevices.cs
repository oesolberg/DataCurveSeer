using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HomeSeerAPI;
using Scheduler.Classes;

namespace DataCurveSeer.Common
{
	public class FloorsRoomsAndDevices
	{
		public List<string> Floors { get; set; }
		public List<string> Rooms { get; set; }
		public List<DeviceFromHomeSeer> Devices { get; set; }

		public bool RoomExists(string roomChosen)
		{
			if (Rooms != null && Rooms.Contains(roomChosen))
				return true;
			return false;
		}

		public bool FloorExists(string floorChosen)
		{

			if (Floors != null && Floors.Contains(floorChosen))
				return true;
			return false;
		}

		public IEnumerable<DeviceFromHomeSeer> GetDevices(string floorChosen, string roomChosen)
		{
			var deviceList = Devices;
			if (!string.IsNullOrEmpty(floorChosen))
			{
				deviceList = Devices.Where(x => x.FloorName == floorChosen).ToList();
			}
			if (!string.IsNullOrEmpty(roomChosen))
			{
				deviceList = Devices.Where(x => x.RoomName == roomChosen).ToList();
			}
			return deviceList;
		}
	}

	public class DeviceFromHomeSeer
	{
		public int DeviceId { get; set; }
		public string DeviceName { get; set; }
		public string RoomName { get; set; }
		public string FloorName{get;set;}
	}
}