using System;
using System.Collections.Generic;
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
		public bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc, double? thresholdValue = null, TimeSpan? timeToReachThreshold = null)
		{
			if (dataPoints != null && dataPoints.Count > 1)
			{
				_logging.LogDebug($"Wait for lock in DataCurveComputation for device:{dataPoints.First().DeviceId}");

				//Do computations to find if we have ascending or descending curve. Start by locking the methode to avoid multiple results
				lock (_lock)
				{
					_logging.LogDebug($"computing the curve for device:{dataPoints.First().DeviceId}");
					var resultSet = ComputeLinearData(dataPoints);
					_logging.LogDebug($"result of computing IsDescending: {resultSet.IsDescending.ToString()} IsAscending:{resultSet.IsAscending.ToString()} Slope:{resultSet.Slope.ToString("#.###")}");

					if (thresholdValue.HasValue && timeToReachThreshold.HasValue)
					{
						//Hente ut siste verdi
						var lastValue = dataPoints.LastOrDefault();
						if (lastValue != null)
						{
							var totalNumberOfSecondsInTimeSpan = timeToReachThreshold.Value.TotalSeconds;
							var futureValueAtEndOfTimeSpan = totalNumberOfSecondsInTimeSpan * resultSet.Slope;
							_logging.LogDebug($"Result of future computation. With slope {resultSet.Slope} for {totalNumberOfSecondsInTimeSpan} seconds computed value is {futureValueAtEndOfTimeSpan} (threshold:{thresholdValue.Value}");
							resultSet.FutureValueAtEndOfTimeSpan = futureValueAtEndOfTimeSpan;
							if (ascDesc == AscDescEnum.Ascending && futureValueAtEndOfTimeSpan >= thresholdValue.Value)
								return true;
							if (ascDesc == AscDescEnum.Descending && futureValueAtEndOfTimeSpan <= thresholdValue.Value)
								return true;
						}
					}
					else
					{
						//Only test curves
						if (ascDesc == AscDescEnum.Ascending && resultSet.IsAscending)
							return true;
						if (ascDesc == AscDescEnum.Descending && resultSet.IsDescending)
							return true;
					}
				}
			}
			return false;
		}

		private ComputedResultSet ComputeLinearData(List<DeviceValue> dataPoints)
		{
			var resultSet = new ComputedResultSet();
			double[] xdata = CreateXDataFromDateTimeToTotalSeconds(dataPoints);
			double[] ydata = CreateYDataFromDoubleValues(dataPoints);

			Tuple<double, double> p = Fit.Line(xdata, ydata);
			double intercept = p.Item1; // == 10; intercept -- No clue what this is
			double slope = p.Item2; // == 0.5; slope
			resultSet.Slope = slope;
			resultSet.Intercept = intercept;
			if (slope > 0)
			{
				resultSet.IsAscending = true;
			}

			if (slope < 0)
			{
				resultSet.IsDescending = true;
			}

			return resultSet;
		}

		private double[] CreateXDataFromDateTimeToTotalSeconds(List<DeviceValue> dataPoints)
		{
			var firstMeasurement = dataPoints.FirstOrDefault();
			var firstDateTime = firstMeasurement?.DateTimeOfMeasurment ?? DateTime.MinValue;

			double[] xData = new double[dataPoints.Count];
			var i = 0;
			foreach (var deviceValue in dataPoints)
			{
				xData[i] = (deviceValue.DateTimeOfMeasurment - firstDateTime).TotalSeconds;
				i++;
			}
			return xData;
		}

		private double[] CreateYDataFromDoubleValues(List<DeviceValue> dataPoints)
		{
			double[] yData = new double[dataPoints.Count];
			var i = 0;
			foreach (var deviceValue in dataPoints)
			{
				yData[i] = deviceValue.Value;
				i++;
			}
			return yData;
		}
	}

	internal class ComputedResultSet
	{
		public bool IsDescending { get; set; }
		public bool IsAscending { get; set; }
		public double Intercept { get; set; }
		public double Slope { get; set; }
		public double FutureValueAtEndOfTimeSpan { get; set; }
	}
}
