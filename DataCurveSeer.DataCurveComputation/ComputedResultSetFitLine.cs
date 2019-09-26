namespace DataCurveSeer.DataCurveComputation
{
    public class ComputedResultSetFitLine
    {
        public bool IsDescending { get; set; }
        public bool IsAscending { get; set; }
        public double Intercept { get; set; }
        public double Slope { get; set; }
        public double FutureValueAtEndOfTimeSpan { get; set; }
    }
}