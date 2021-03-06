﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.Common.Repository;
using MathNet.Numerics;

namespace DataCurveSeer.DataCurveComputation
{

	public class DataCurveComputationHandler : IDataCurveComputationHandler
	{

		private static Object _lock = new Object();
		private ILogging _logging;

		public DataCurveComputationHandler(ILogging logging)
		{
			_logging = logging;
		}
		public bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc, double? threshold = null, TimeSpan? timeToReachThreshold = null)
		{
			if (dataPoints != null && dataPoints.Count > 1)
			{
				_logging.LogDebug($"Wait for lock in DataCurveComputation for device:{dataPoints.First().DeviceId}");

				//Do computations to find if we have ascending or descending curve. Start by locking the methode to avoid multiple results
				lock (_lock)
				{
					_logging.LogDebug($"computing the curve for device:{dataPoints.First().DeviceId}");

                    var fitLineHandler=new FitLineHandler();
					var resultSetComputedLinearData = fitLineHandler.ComputeLinearData(dataPoints);

					var culture = CultureInfo.CreateSpecificCulture("en-US");
					_logging.LogDebug($"result of computing IsDescending: {resultSetComputedLinearData.IsDescending.ToString()} IsAscending:{resultSetComputedLinearData.IsAscending.ToString()} Slope:{resultSetComputedLinearData.Slope.ToString("#.###", culture)}");

					var resultsetLastValues = ComputeLastValues(dataPoints, ascDesc);

					if (threshold.HasValue && timeToReachThreshold.HasValue)
					{
						//Hente ut siste verdi
						var resultSetComputedFutureLine = ComputeFutureValues(dataPoints, ascDesc, resultSetComputedLinearData, threshold.Value, timeToReachThreshold.Value);
						_logging.LogDebug($"ascDec Descending{ascDesc == AscDescEnum.Descending} resultSetComputedFutureLine.TriggerTrue: {resultSetComputedFutureLine.TriggerTrue} resultSetComputedLinearData.IsDescending: {resultSetComputedLinearData.IsDescending } resultSetComputedLinearData.IsAscending: {resultSetComputedLinearData.IsAscending }  resultsetLastValues.LastSlope: {resultsetLastValues.LastSlope} resultsetLastValues.LastValueHighest: {resultsetLastValues.LastValueHighest } resultsetLastValues.LastValueLowest: {resultsetLastValues.LastValueLowest} ");
						if (ascDesc==AscDescEnum.Ascending && resultSetComputedFutureLine.TriggerTrue && resultSetComputedLinearData.IsAscending && resultsetLastValues.LastSlope > 0 && resultsetLastValues.LastValueHighest)
						{
							return true;
						}
						if (ascDesc == AscDescEnum.Descending && resultSetComputedFutureLine.TriggerTrue && resultSetComputedLinearData.IsDescending && resultsetLastValues.LastSlope < 0 && resultsetLastValues.LastValueLowest)
						{
							return true;
						}
					}

					else
					{
						//Only test curves
						_logging.LogDebug($"ascDec Descending{ascDesc == AscDescEnum.Descending}  resultSetComputedLinearData.IsDescending: {resultSetComputedLinearData.IsDescending } resultSetComputedLinearData.IsAscending: {resultSetComputedLinearData.IsAscending }  resultsetLastValues.LastSlope: {resultsetLastValues.LastSlope} resultsetLastValues.LastValueHighest: {resultsetLastValues.LastValueHighest } resultsetLastValues.LastValueLowest: {resultsetLastValues.LastValueLowest} ");
						if (ascDesc == AscDescEnum.Ascending && resultSetComputedLinearData.IsAscending && resultsetLastValues.LastSlope>0 && resultsetLastValues.LastValueHighest)
							return true;
						if (ascDesc == AscDescEnum.Descending && resultSetComputedLinearData.IsDescending && resultsetLastValues.LastSlope<0 && resultsetLastValues.LastValueLowest)
							return true;
					}
				}
			}
			return false;
		}

		private ResultSetComputedFutureValue ComputeFutureValues(List<DeviceValue> dataPoints, AscDescEnum ascDesc, ComputedResultSetFitLine resultSetComputedLinearData, double threshold, TimeSpan timeToReachThreshold)
		{
			var resultSet = new ResultSetComputedFutureValue();
			var lastValue = dataPoints.LastOrDefault();
			if (lastValue != null)
			{
				
				var totalNumberOfSecondsInTimeSpan = timeToReachThreshold.TotalSeconds;
				var futureValueAtEndOfTimeSpan = totalNumberOfSecondsInTimeSpan * resultSetComputedLinearData.Slope;
				_logging.LogDebug($"Result of future computation. With slope {resultSetComputedLinearData.Slope} for {totalNumberOfSecondsInTimeSpan} seconds computed value is {futureValueAtEndOfTimeSpan} (threshold:{threshold})");
				resultSet.FutureValueAtEndOfTimeSpan = futureValueAtEndOfTimeSpan;

				if (ascDesc == AscDescEnum.Ascending && futureValueAtEndOfTimeSpan >= threshold)
					resultSet.TriggerTrue = true;
				if (ascDesc == AscDescEnum.Descending && futureValueAtEndOfTimeSpan <= threshold)
					resultSet.TriggerTrue = true;
			}

			return resultSet;
		}

		private ComputeLastValuesResultSet ComputeLastValues(List<DeviceValue> dataPoints, AscDescEnum ascDesc)
		{
			var resultSet = new ComputeLastValuesResultSet();
			var maxValue = dataPoints.Max(x => x.Value);
			var minValue = dataPoints.Min(x => x.Value);

			var lastRegisteredMeasurementTime = dataPoints.Max(x => x.DateTimeOfMeasurment);
			var lastDataPoint = dataPoints.FirstOrDefault(x => x.DateTimeOfMeasurment == lastRegisteredMeasurementTime);
			var secondlastRegisteredMeasurementTime = dataPoints.Where(x => x.DateTimeOfMeasurment < lastRegisteredMeasurementTime).Max(x => x.DateTimeOfMeasurment);
			var secondLastDataPoint =
				dataPoints.FirstOrDefault(x => x.DateTimeOfMeasurment == secondlastRegisteredMeasurementTime);

			resultSet.LastSlope = (lastDataPoint.Value - secondLastDataPoint.Value) /
							((lastDataPoint.DateTimeOfMeasurment - secondLastDataPoint.DateTimeOfMeasurment)
								.TotalSeconds);
			if (minValue == lastDataPoint.Value)
			{
				resultSet.LastValueLowest = true;
			}
			if (maxValue == lastDataPoint.Value)
			{
				resultSet.LastValueHighest = true;
			}

			return resultSet;
		}

		

		
	}

    internal class ResultSetComputedFutureValue
	{
		public bool TriggerTrue { get; set; }
		public double FutureValueAtEndOfTimeSpan { get; set; }
	}
}
