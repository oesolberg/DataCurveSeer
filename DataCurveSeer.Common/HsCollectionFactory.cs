
using HomeSeerAPI;

namespace DataCurveSeer.Common
{
	public class HsCollectionFactory : IHsCollectionFactory
	{

		public HsCollectionFactory()
		{

		}

		public Classes.action GetActionsIfPossible(IPlugInAPI.strTrigActInfo trigActInfo)
		{
			object objAction = new Classes.action();
			if (trigActInfo.DataIn != null && trigActInfo.DataIn.Length > 0)
			{
				Utility.DeSerializeObject(ref trigActInfo.DataIn,
					ref objAction);
				Classes.action formattedAction = (Classes.action)objAction;
				if (formattedAction != null)
				{
					return formattedAction;
				}
			}
			return (Classes.action)objAction;
		}

	}
}
