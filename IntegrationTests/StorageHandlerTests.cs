using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.DataCurveComputation;
using NUnit.Framework;
using DataCurveSeer.Storage;
using NSubstitute;
using Should;

namespace IntegrationTests
{
	[TestFixture]
    public class StorageHandlerTests
    {
	    private ILogging _logging;
        private IIniSettings _iniSettings;
        [SetUp]
	    public void SetUp()
	    {
		    _logging = NSubstitute.Substitute.For<ILogging>();
            _iniSettings = NSubstitute.Substitute.For<IIniSettings>();

        }
	    [Test]
	    public void GetValuesForDevice_ShouldReturnCreatedValues()
	    {
		    var deviceId = 666;
		    var sut=new StorageHandler(_logging,_iniSettings);
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

	    [Test]
	    public void GetValuesForDevice_ShouldComputeSomething()
	    {
		    var deviceId = 530;
		    var path = System.IO.Path.GetDirectoryName(
			    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
		    if (path.StartsWith("file:\\"))
		    {
			    path = path.Substring(6);
		    }
			var dbPath= Path.Combine(path, "DataCurveSeer.db");
			var storageHandler = new StorageHandler(_logging,_iniSettings,dbPath);

			var result = storageHandler.GetValuesForDevice(deviceId, new DateTime(2019, 9, 12), new DateTime(2019, 9, 12, 1, 0, 0));
			var sut = new DataCurveComputationHandler(_logging);
			var finalResult = sut.TriggerTrue(result, ascDesc: DataCurveSeer.Common.AscDescEnum.Ascending,threshold: 10.5d,timeToReachThreshold: new TimeSpan(0,3,0,0));
			finalResult.ShouldBeFalse();

	    }

        [Test]
        public void Maintenance_ShouldDeleteSomething()
        {

            var deviceId = 530;
            _iniSettings.DaysOfDataStorage.Returns(5);
            var dbPath = CreateCopyOfDbAndReturnPath("DataCurveSeer_okt2019.db");

            var storageHandler = new StorageHandler(_logging, _iniSettings, dbPath);
            SystemDateTime.Now=()=>new DateTime(2019, 10, 16, 0, 0, 0);

            storageHandler.AddDeviceValueToDatabase(99,SystemDateTime.Now(),deviceId);

            storageHandler.AddDeviceValueToDatabase(99, SystemDateTime.Now(), deviceId);


        }

        private string CreateCopyOfDbAndReturnPath(string fileToFind)
        {
            var path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (path.StartsWith("file:\\"))
            {
                path = path.Substring(6);
            }

            if (string.IsNullOrEmpty(fileToFind))
                fileToFind = "DataCurveSeer.db";
            var existingDbPath = Path.Combine(path, fileToFind);
            var newDbPath = Path.Combine(path, "DataCurveSeerEdit.db");
            File.Copy(existingDbPath,newDbPath,overwrite:true);
            return newDbPath;
        }
    }
}

