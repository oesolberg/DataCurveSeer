using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.TriggerHandling.Triggers;
using HomeSeerAPI;

namespace DataCurveSeer.TriggerHandling
{

	internal static class TriggerFactory
	{
		public static ITrigger Get(IHSApplication hs,string triggerType, ILogging logging, ITriggerHandler triggerHandler, IAppCallbackAPI callback, IHsCollectionFactory collectionFactory,IHomeSeerHandler homeSeerHandler)
		{
			switch (triggerType)
			{
				case "DataCurveSeer.TriggerHandling.Triggers.DataCurveTrigger":
					return new DataCurveTrigger(hs,logging, collectionFactory,homeSeerHandler);
                case "DataCurveSeer.TriggerHandling.Triggers.DataCurveTriggerB.DataCurveTriggerB":
                    return new DataCurveTrigger(hs, logging, collectionFactory, homeSeerHandler);

                default:
					logging.Log($"Could not find any match for {triggerType}. Alert the creator and tell him to check for errors.");
					break;
			}

			return null;
		}
	}

}