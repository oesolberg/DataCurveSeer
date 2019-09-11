using System;
using System.Collections.Generic;
using DataCurveSeer.Common.Repository;

namespace DataCurveSeer.Common.Interfaces
{
	public interface IStorageHandler
	{
		void AddDeviceValueToDatabase(double value, DateTime dateTimeOfMeasurement, int deviceId);
		void RemoveFromDatabase(int deviceId);
		List<DeviceValue> GetValuesForDevice(int deviceId, DateTime? fromDateTime, DateTime? toDateTime);
	}
}