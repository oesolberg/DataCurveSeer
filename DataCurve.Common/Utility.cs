using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using HomeSeerAPI;

namespace DataCurve.Common
{
	public class Utility
	{
		public const string PluginName = "DataCurve";
		public const string IniFile = PluginName+".ini";
		public static string ExePath = Directory.GetCurrentDirectory();
		public static string InstanceFriendlyName => "";
		public static bool IsRunningOnMono => (Type.GetType("Mono.Runtime") != null);

		public static bool StringToBool(string stringToCheck)
		{
			bool returnValue = !string.IsNullOrEmpty(stringToCheck) && stringToCheck.Trim().ToUpper() == "TRUE";
			return returnValue;
		}

	
		public static bool DeSerializeObject(ref byte[] bteIn, ref object objOut, IHSApplication hs = null)
		{
			bool result;
			if (bteIn == null)
			{
				result = false;
			}
			else if (bteIn.Length < 1)
			{
				result = false;
			}
			else if (objOut == null)
			{
				result = false;
			}
			else
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				try
				{
					objOut.GetType();
					objOut = null;
					MemoryStream serializationStream = new MemoryStream(bteIn);
					object objectValue = RuntimeHelpers.GetObjectValue(binaryFormatter.Deserialize(serializationStream));
					if (objectValue == null)
					{
						result = false;
					}
					else
					{
						objectValue.GetType();
						objOut = RuntimeHelpers.GetObjectValue(objectValue);
						if (objOut == null)
						{
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
				catch (InvalidCastException invalidCastException)
				{
					//ProjectData.SetProjectError(expr_6C);
					result = false;
					Console.WriteLine(invalidCastException.Message);
					//ProjectData.ClearProjectError();
				}
				catch (Exception exception)
				{
					//ProjectData.SetProjectError(expr_7D);
					//Exception ex = expr_7D;
					//Utils._log.Error(ex,  + " Error: DeSerializing object: " + ex.Message, new object[0]);
					result = false;
					Console.WriteLine(exception.Message);
					if (hs != null)
						hs.WriteLog(Utility.PluginName, "General exception error when casting in deserializer: " + exception.Message + Environment.NewLine + exception.StackTrace);
					//ProjectData.ClearProjectError();
				}
			}
			return result;
		}

		public static bool SerializeObject(ref object objIn, ref byte[] bteOut, IHSApplication hs = null)
		{
			bool result;
			if (objIn == null)
			{
				result = false;
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				try
				{
					binaryFormatter.Serialize(memoryStream, RuntimeHelpers.GetObjectValue(objIn));
					bteOut = new byte[checked((int)(memoryStream.Length - 1L) + 1)];
					bteOut = memoryStream.ToArray();
					result = true;
				}
				catch (Exception exception)
				{
					//Hs.WriteLog(Utils.IfaceName, "Error when serializing object");
					//Utils._log.Error(ex, string.Concat(new string[]
					//{
					//    Utils.IfaceName,
					//    " Error: Serializing object ",
					//    objIn.ToString(),
					//    " :",
					//    ex.Message
					//}), new object[0]);
					result = false;
					if (hs != null)
						hs.WriteLog(Utility.PluginName, "General exception error when casting in serializer: " + exception.Message + Environment.NewLine + exception.StackTrace);
				}
			}
			return result;
		}
	}
}