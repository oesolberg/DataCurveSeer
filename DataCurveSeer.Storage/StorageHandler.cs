using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Repository;
using DataCurveSeer.Common.Interfaces;
using LiteDB;

namespace DataCurveSeer.Storage
{


	public class StorageHandler:IStorageHandler,IDisposable
    {

	    private const string DeviceValuesTable = "DeviceValues";

		private ILogging _logging;
	    private bool _disposed;
	    private static Object _lockObject = new Object();
	    private readonly string _repoFilename = $"Data/{Utility.PluginName}/{Utility.PluginName}.db";
	    private readonly string _pathToRepo;

		public StorageHandler(ILogging logging,string dbPath=null)
	    {
		    _logging = logging;
		    var fullPathToDb = CreateDbPath();
		    if (!string.IsNullOrEmpty(dbPath))
		    {
			    fullPathToDb = dbPath;
		    }

		    _pathToRepo = fullPathToDb;

			if (File.Exists(fullPathToDb)) return;
		    _logging.LogDebug("Creating folder for liteDb database");
		    var dbDirectory = Path.GetDirectoryName(fullPathToDb);
		    if (dbDirectory != null)
			    Directory.CreateDirectory(dbDirectory);
		}

		private string CreateDbPath()
		{
			var baseDirectory = Directory.GetCurrentDirectory();
			return Path.Combine(baseDirectory, _repoFilename);
		}

		public void AddDeviceValueToDatabase(double value, DateTime dateTimeOfMeasurement, int referenceId)
		{
			//lock (_lockObject)
			//{
				using (var db = new LiteDatabase(_pathToRepo))
				{
					var deviceValues = db.GetCollection<DeviceValue>(DeviceValuesTable);
					_logging.LogDebug("LitDbRepo: inserting value into liteDb");
					deviceValues.Insert(new DeviceValue() {DeviceId = referenceId, Value = value,DateTimeOfMeasurment = dateTimeOfMeasurement });
					deviceValues.EnsureIndex(x => x.DateTimeOfMeasurment);
					deviceValues.EnsureIndex(x => x.DeviceId);
			}
			//}
		}

		public void RemoveFromDatabase(int deviceId)
		{
			//lock (_lockObject)
			//{
				using (var db = new LiteDatabase(_pathToRepo))
				{
					var deviceValues = db.GetCollection<DeviceValue>(DeviceValuesTable);
					_logging.LogDebug($"LitDbRepo: deleting values for deviceId {deviceId}");
					deviceValues.Delete(x => x.DeviceId == deviceId);
				}
			//}
		}

		public List<DeviceValue> GetValuesForDevice(int deviceId, DateTime? fromDateTime, DateTime? toDateTime)
		{
			var startDateTime=DateTime.Now.AddHours(2);
			var endDateTime = DateTime.Now;
			if (fromDateTime.HasValue)
			{
				startDateTime = fromDateTime.Value;
			}
			if (toDateTime.HasValue)
			{
				endDateTime = toDateTime.Value;
			}

			//lock (_lockObject)
			//{
				using (var db = new LiteRepository(_pathToRepo))
				{
					_logging.LogDebug($"LitDbRepo: selecting values for deviceId {deviceId} with starDateTime {startDateTime.ToString()} and endDateTime {endDateTime.ToString()}");

					var foundValues = db.Query<DeviceValue>(DeviceValuesTable)
						.Where(x => x.DeviceId == deviceId && 
								x.DateTimeOfMeasurment >= startDateTime &&
								x.DateTimeOfMeasurment <= endDateTime)
						
						.ToList();
					return foundValues;
				}
			//}

			//return new List<DeviceValue>();
		}

		public void Dispose()
		{
			Dispose(true);

			// Use SupressFinalize in case a subclass 
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// Indicate that the instance has been disposed.
				_disposed = true;
			}
		}
	}

	
}
