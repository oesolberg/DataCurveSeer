using HomeSeerAPI;

namespace DataCurve.Common
{
	public interface IHsCollectionFactory
	{
		Classes.action GetActionsIfPossible(IPlugInAPI.strTrigActInfo trigActInfo);
	}
}