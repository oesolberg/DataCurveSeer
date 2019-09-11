using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.Common.Repository;

namespace DataCurveSeer.DataCurveComputation
{
	
	public class DataCurveComputationHandler: IDataCurveComputationHandler
	{
		public DataCurveComputationHandler(ILogging logging)
		{
			
		}
		private static Object _lock=new Object();
		public bool TriggerTrue(List<DeviceValue> dataPoints, AscDescEnum ascDesc)
		{
			if (dataPoints != null && dataPoints.Count > 1)
			{
				//Do computations to find if we have ascending or descending curve. Start by locking the methode to avoid multiple results
				lock (_lock)
				{
					return true;
				}
				
			}
			return false;
		}
    }
}
