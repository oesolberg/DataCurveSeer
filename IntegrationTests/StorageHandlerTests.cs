using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common.Interfaces;
using NUnit.Framework;
using DataCurveSeer.Storage;
using Should;

namespace IntegrationTests
{
	[TestFixture]
    public class StorageHandlerTests
    {
	    private ILogging _logging;

		[SetUp]
	    public void SetUp()
	    {
		    _logging = NSubstitute.Substitute.For<ILogging>();

	    }
	    [Test]
	    public void GetValuesForDevice_ShouldReturnCreatedValues()
	    {
		    var deviceId = 666;
		    var sut=new StorageHandler(_logging);
		    CreateACoupleOfPoints(sut, deviceId);

		    var result = sut.GetValuesForDevice(deviceId, new DateTime(2000, 1, 1), new DateTime(2000, 1, 1, 1, 0, 0));
			//Clean up
		    DeleteThePoints(sut, deviceId);

			result.ShouldNotBeNull();
			result.Count.ShouldBeGreaterThan(0);
		    
	    }

	    private void CreateACoupleOfPoints(StorageHandler storeHandler,int deviceId)
		{
			storeHandler.AddDeviceValueToDatabase(1,new DateTime(2000,1,1,0,0,1), deviceId);

			storeHandler.AddDeviceValueToDatabase(2, new DateTime(2000, 1, 1, 0, 0, 2), deviceId);

			storeHandler.AddDeviceValueToDatabase(3, new DateTime(2000, 1, 1, 0, 0, 3), deviceId);
		}

	    private void DeleteThePoints(StorageHandler sut,int deviceId)
		{
		    sut.RemoveFromDatabase(deviceId);
	    }
    }
}
