using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeSeerAPI;

namespace DataCurveSeer.Common.Interfaces
{
    public interface IMainPlugin
    {
	    string InitIO(string port);
	    void ShutDownIO();
	    void Dispose();
	    bool GetHasTriggers();
	    int GetTriggerCount();
	    string TriggerBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo);
	    string TriggerFormatUI(IPlugInAPI.strTrigActInfo actionInfo);

	    IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection postData,
		    IPlugInAPI.strTrigActInfo actionInfo);

	    bool TriggerTrue(IPlugInAPI.strTrigActInfo triggerInfo);
	    int get_SubTriggerCount(int triggerNumber);

	    string get_SubTriggerName(int triggerNumber, int subTriggerNumber);
	    bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo);
	    string get_TriggerName(int triggerNumber);



	    bool get_Condition(IPlugInAPI.strTrigActInfo actionInfo);
	    bool get_HasConditions(int triggerNumber);
	    
	    void set_Condition(IPlugInAPI.strTrigActInfo actionInfo, bool value);

	    string GetPagePlugin(string page, string user, int userRights, string queryString);
	    string PostBackProc(string page, string data, string user, int userRights);






    }
}
