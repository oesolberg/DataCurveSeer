using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Text;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using HomeSeerAPI;
using Scheduler;

namespace DataCurveSeer.TriggerHandling.Triggers
{
	public interface IDataCurveTriggerUi
	{

	}

	internal sealed class DataCurveTriggerUi : IDataCurveTriggerUi
	{

		private const string EventsPage = "events";
		private IHomeSeerHandler _homeSeerHandler;
		private FloorsRoomsAndDevices _floorsRomsAndDevices;
		private IHSApplication _hs;

		public DataCurveTriggerUi(IHomeSeerHandler homeSeerHandler,IHSApplication hs)
		{
			_homeSeerHandler = homeSeerHandler;
			_hs = hs;
			PopulateWithHomeSeerData();
		}

		private void PopulateWithHomeSeerData()
		{
			_floorsRomsAndDevices = _homeSeerHandler.GetFloorsRoomsAndDevices();
		}

		public string Build(DataCurveTriggerSettings triggerSettings, IHomeSeerHandler homeSeerHandler = null)
		{
			var sb = new StringBuilder();
			sb.AppendLine("<table>");
			
			var floorDropDown = CreateFloorDropdown(triggerSettings.FloorChosen, triggerSettings.Uid, triggerSettings.UniqueControllerId);
			var roomDropdown = CreateRoomDropdown(triggerSettings.RoomChosen, triggerSettings.Uid, triggerSettings.UniqueControllerId);
			if (!_floorsRomsAndDevices.RoomExists(triggerSettings.RoomChosen))
			{
				triggerSettings.RoomChosen = "";
			}
			if (!_floorsRomsAndDevices.FloorExists(triggerSettings.FloorChosen))
			{
				triggerSettings.FloorChosen = "";
			}
			var deviceDropdown = CreateDeviceDropdown(triggerSettings.FloorChosen, triggerSettings.RoomChosen,triggerSettings.DeviceIdChosen, triggerSettings.Uid, triggerSettings.UniqueControllerId);
			var timePicker = CreateTimePicker(triggerSettings.TimeSpanChosen,triggerSettings.Uid,triggerSettings.UniqueControllerId);
			var ascendingDescendingDropdown = CreateAscendingDescendingDropdown(triggerSettings.AscendingOrDescending,triggerSettings.Uid,triggerSettings.UniqueControllerId);
			sb.AppendLine($"<tr><td>A data curve of device values for the device {floorDropDown} {roomDropdown}  {deviceDropdown} has had {ascendingDescendingDropdown} curve for the last {timePicker} minutes</td></tr>");
			sb.AppendLine("</table>");
			return sb.ToString();
		}

		private string CreateAscendingDescendingDropdown(AscDescEnum ascDescChosen,string uid, string uniqueControllerId)
		{
			var ascDescValues = CreateAscDescValues();
			var dropdown =CreateDropDownFromNameValueCollection(Constants.AscDescKey, Enum.GetName(typeof(AscDescEnum), ascDescChosen),
																	ascDescValues,uid,uniqueControllerId,noDefaultBlank :true);
			return dropdown.Build();
		}

		private NameValueCollection CreateAscDescValues()
		{
			var returnList=new NameValueCollection();
			returnList.Add(Enum.GetName(typeof(AscDescEnum), AscDescEnum.Ascending), "an ascending");
			returnList.Add(Enum.GetName(typeof(AscDescEnum), AscDescEnum.Descending), "a descending");
			return returnList;
		}

		private string CreateFloorDropdown(string chosenFloor, string uid, string uniqueControllerId)
		{
			var dropDown = CreateDropDown(Constants.FloorKey, chosenFloor, _floorsRomsAndDevices.Floors, uid,
				uniqueControllerId);
			return dropDown.Build();
		}

		private string CreateRoomDropdown(string chosenRoom, string uid, string uniqueControllerId)
		{
			var dropDown = CreateDropDown(Constants.RoomKey, chosenRoom, _floorsRomsAndDevices.Rooms, uid,
				uniqueControllerId);
			return dropDown.Build();
		}

		private string CreateDeviceDropdown(string floorChosen, string roomChosen, int? deviceId, string uid, string uniqueControllerId)
		{
			string deviceDropDownParameter = Constants.DeviceDropdownKey;
			var devicesFromFloorAndRoom = _floorsRomsAndDevices.GetDevices(floorChosen, roomChosen);
			var chosenDevice = -1;
			var noSelectionMade= false;
			if (deviceId.HasValue)
			{
				chosenDevice = deviceId.Value;
			}
			else
			{
				noSelectionMade = true;
			}
			var listToReturn = new clsJQuery.jqDropList(deviceDropDownParameter + uid + uniqueControllerId, EventsPage, true);
			listToReturn.AddItem("", "-1", noSelectionMade);
			foreach (var hsDevice in devicesFromFloorAndRoom)
			{
				var currentDeviceId = hsDevice.DeviceId;
				listToReturn.AddItem(hsDevice.DeviceName, currentDeviceId.ToString(), currentDeviceId == chosenDevice);
			}
			return listToReturn.Build();

		}
		private clsJQuery.jqDropList CreateDropDownFromNameValueCollection(string dropDownParameter, string chosenValue, NameValueCollection unitList, string uid, string uniqueControllerId,bool noDefaultBlank=false)
		{
			var dropList = new clsJQuery.jqDropList(dropDownParameter + uid + uniqueControllerId, EventsPage, true);
			if(!noDefaultBlank)
				dropList.AddItem("", "", false);
			foreach (var unit in unitList.AllKeys)
			{
				dropList.AddItem(unitList[unit], unit, unit == chosenValue);
			}
			return dropList;
		}

		private clsJQuery.jqDropList CreateDropDown(string dropDownParameter, string chosenValue, List<string> unitList, string uid, string uniqueControllerId)
		{
			var dropList = new clsJQuery.jqDropList(dropDownParameter + uid + uniqueControllerId, EventsPage, true);
			dropList.AddItem("", "", false);
			foreach (var unit in unitList)
			{
				dropList.AddItem(unit, unit, unit == chosenValue);
			}
			return dropList;
		}


		private string CreateTimePicker(TimeSpan? timeSpanChosen, string uid, string uniqueControllerId)
		{
			var timePicker=new clsJQuery.jqTimeSpanPicker(Constants.TimeSpanKey,"",EventsPage,true);
			timePicker.showDays = false;
			timePicker.showSeconds = false;
			timePicker.defaultTimeSpan=new TimeSpan(1,0,0);
			if (timeSpanChosen.HasValue)
			{
				timePicker.defaultTimeSpan = timeSpanChosen.Value;
			}

			return timePicker.Build();
		}
	}
}