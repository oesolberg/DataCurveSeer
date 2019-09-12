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
	
	public class DataCurveComputationHandler: IDataCurveComputationHandler
	{
		
		private static Object _lock=new Object();
		private ILogging _logging;

		public DataCurveComputationHandler(ILogging logging)
		{
			_logging = logging;
		}
		public bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc)
		{
			if (dataPoints != null && dataPoints.Count > 1)
			{
				//Do computations to find if we have ascending or descending curve. Start by locking the methode to avoid multiple results
				lock (_lock)
				{
					_logging.LogDebug($"computing the curve for device:{dataPoints.First().DeviceId}");
					var resultSet = ComputeLinearData(dataPoints);
					_logging.LogDebug($"result of computing IsDescending: {resultSet.IsDescending.ToString()} IsAscending:{resultSet.IsAscending.ToString()} Slope:{resultSet.Slope.ToString("#.###")}");
					if (ascDesc == AscDescEnum.Ascending && resultSet.IsAscending)
						return true;
					if (ascDesc == AscDescEnum.Descending && resultSet.IsDescending)
						return true;
				}
			}
			return false;
		}

		private ComputedResultSet ComputeLinearData(List<DeviceValue> dataPoints)
		{
			var resultSet=new ComputedResultSet();
			double[] xdata = CreateXDataFromDateTime(dataPoints);
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

		private double[] CreateXDataFromDateTime(List<DeviceValue> dataPoints)
		{
			double[] xData = new double[dataPoints.Count];
			var i = 0;
			foreach (var deviceValue in dataPoints)
			{
				xData[i] = CreateDoubleValueFromDateTime(deviceValue.DateTimeOfMeasurment);
			}
			return xData;
		}

		private double CreateDoubleValueFromDateTime(DateTime deviceValue)
		{

			double returnValue = deviceValue.Ticks;
			return returnValue;
		}

		private double[] CreateYDataFromDoubleValues(List<DeviceValue> dataPoints)
		{
			double[] yData = new double[dataPoints.Count];
			var i = 0;
			foreach (var deviceValue in dataPoints)
			{
				yData[i] = deviceValue.Value;
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
	}
}
