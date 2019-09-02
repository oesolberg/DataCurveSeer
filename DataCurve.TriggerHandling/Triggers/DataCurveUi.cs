using System.Data.SqlTypes;
using System.Text;

namespace DataCurve.TriggerHandling.Triggers
{
	public interface IDataCurveUi
	{

	}

	internal sealed class DataCurveUi: IDataCurveUi
	{
		public string Build(DataCurveTriggerSettings triggerSettings)
		{
			var sb = new StringBuilder();
			sb.AppendLine("<table>");
			var floorDropDown = CreateFloorDropdown();
			var roomDropdown = CreateRoomDropdown();
			var deviceDropdown = CreateDeviceDropdown();
			var timePicker = CreateTimePicker();
			sb.AppendLine($"<tr><td>A data curve of device values for the device {floorDropDown} -> {roomDropdown} -> {deviceDropdown} has had a ascending/descending curve for the last {timePicker} minutes</td></tr>");
			sb.AppendLine("</table>");
			return sb.ToString();
		}

		private string CreateFloorDropdown()
		{
			return "floor";
		}

		private string CreateRoomDropdown()
		{
			return "room";
		}

		private string CreateDeviceDropdown()
		{
			return "dev";
		}

		private string CreateTimePicker()
		{
			return "10";
		}
	}
}