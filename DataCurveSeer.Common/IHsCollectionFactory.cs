using HomeSeerAPI;

namespace DataCurveSeer.Common
{
	public interface IHsCollectionFactory
	{
		Classes.action GetActionsIfPossible(IPlugInAPI.strTrigActInfo trigActInfo);
	}
}