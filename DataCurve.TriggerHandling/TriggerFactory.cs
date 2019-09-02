using DataCurve.Common;
using DataCurve.Common.Interfaces;
using DataCurve.TriggerHandling.Triggers;
using HomeSeerAPI;

namespace DataCurve.TriggerHandling
{

	internal static class TriggerFactory
	{
		public static ITrigger Get(string triggerType, ILogging logging, ITriggerHandler triggerHandler, IAppCallbackAPI callback, IHsCollectionFactory collectionFactory)
		{
			switch (triggerType)
			{
				//case "GCalSeer.MainPlugin.Triggers.GetSummary": return new GetSummary(log, mainPlugin, callback);
				case "DataCurve.TriggerHandling.Triggers.DataCurveTrigger":
					return new DataCurveTrigger(logging,collectionFactory);

				default:
					logging.Log($"Could not find any match for {triggerType}. Alert the creator and tell him to check for errors.");
					break;
			}

			return null;
		}
	}

}