using System;
using System.Collections.Generic;
using DataCurveSeer.Common.Repository;

namespace DataCurveSeer.Common.Interfaces
{
	public interface IDataCurveComputationHandler
	{
		//bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc);
		bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc, double? thresholdValue = null,
			TimeSpan? timeToReachThreshold = null);
	}

    public interface IDataCurveComputationHandlerB
    {
        //bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc);
        bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc, double thresholdValue,int numberOfLastDataPoints);
    }
}