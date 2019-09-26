using System;
using System.Collections.Generic;
using System.Linq;
using DataCurveSeer.Common.Repository;
using MathNet.Numerics;

namespace DataCurveSeer.DataCurveComputation
{
    public class FitLineHandler
    {

        public ComputedResultSetFitLine ComputeLinearData(List<DeviceValue> dataPoints)
        {
            var resultSet = new ComputedResultSetFitLine();
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
}