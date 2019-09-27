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

        public bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc, double thresholdValue, int numberOfLastMeasurements)
        {
            if (dataPoints == null)
            {
                _logging.LogDebug("Datapoints is null");
                return false;
            }

            if (dataPoints.Count < numberOfLastMeasurements)
            {
                _logging.LogDebug($"Too few datapoints: {dataPoints.Count}");
                return false;
            }

            //Get last data point
            var lastDataPoint = dataPoints.OrderBy(x=>x.DateTimeOfMeasurment).Last();

            //Check if we have reached threshold. If not return false
            if (!ThresholdReached(ascDesc, lastDataPoint.Value, thresholdValue))
                return false;

            var resultSetComputedLinearData = CreateLinearDataSet(dataPoints,numberOfLastMeasurements);

            return CheckResultData(ascDesc, resultSetComputedLinearData);

        }

        private bool CheckResultData(AscDescEnum ascDesc, ComputedResultSetFitLine resultSetComputedLinearData)
        {
            if (ascDesc == AscDescEnum.Descending && resultSetComputedLinearData.IsDescending)
                return true;

            if (ascDesc == AscDescEnum.Ascending && resultSetComputedLinearData.IsAscending)
                return true;

            return false;
        }

        private ComputedResultSetFitLine CreateLinearDataSet(List<DeviceValue> dataPoints,int numberOfLastMeasurements)
        {
            var lastDataPoints = dataPoints.OrderBy(x => x.DateTimeOfMeasurment).Take(numberOfLastMeasurements).ToList();
            var fitLineHandler = new FitLineHandler();
                return fitLineHandler.ComputeLinearData(lastDataPoints);
        }

        private bool ThresholdReached(AscDescEnum ascDesc, double currentValue, double thresholdValue)
        {
            if (ascDesc == AscDescEnum.Descending && currentValue > thresholdValue)
                return false;

            if (ascDesc == AscDescEnum.Ascending && currentValue < thresholdValue)
                return false;

            return true;
        }
    }
}