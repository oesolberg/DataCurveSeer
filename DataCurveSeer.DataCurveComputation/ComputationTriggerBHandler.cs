using System;
using System.Collections.Generic;
using System.Linq;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.Common.Repository;

namespace DataCurveSeer.DataCurveComputation
{
    public class ComputationTriggerBHandler : IDataCurveComputationHandlerB
    {

        private static Object _lock = new Object();
        private ILogging _logging;

        public ComputationTriggerBHandler(ILogging logging)
        {
            _logging = logging;
        }

        public bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc, double thresholdValue)
        {
            if (dataPoints == null)
            {
                _logging.LogDebug("Datapoints is null");
                return false;
            }

            if (dataPoints.Count < 10)
            {
                _logging.LogDebug($"Too few datapoints: {dataPoints.Count}");
                return false;
            }


            var lastDateTime = dataPoints.Max(x => x.DateTimeOfMeasurment);
            var lastDataPoint = dataPoints.First(x => x.DateTimeOfMeasurment == lastDateTime);

            var tenLastDataPoints = dataPoints.OrderBy(x => x.DateTimeOfMeasurment).Take(10).ToList();

            var fitLineHandler = new FitLineHandler();
            var resultSetComputedLinearData = fitLineHandler.ComputeLinearData(tenLastDataPoints);


            if (ascDesc == AscDescEnum.Descending && lastDataPoint.Value < thresholdValue &&
                resultSetComputedLinearData.IsDescending)
                return true;

            if (ascDesc == AscDescEnum.Ascending && lastDataPoint.Value > thresholdValue &&
                resultSetComputedLinearData.IsAscending)
                return true;


            return false;
        }
    }
}