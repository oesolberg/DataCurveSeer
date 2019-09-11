using System.Collections.Generic;
using DataCurveSeer.Common.Repository;

namespace DataCurveSeer.Common.Interfaces
{
	public interface IDataCurveComputationHandler
	{
		bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc);
	}

}