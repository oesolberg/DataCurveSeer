using System;
using System.Collections.Generic;
using HomeSeerAPI;

namespace DataCurveSeer.Common
{

	public delegate void DeviceIdSetEventHandler(Object sender, EventArgs eventArgs);
	public delegate void IniSettingsChangedEventHandler(Object sender, EventArgs eventArgs);
    public delegate void TriggerDataReadyEventHandler(Object sender, TriggersInHomeSeerDataEventArgs eventArgs);

    public class DelegateDummy
	{

	}

    public class TriggersInHomeSeerDataEventArgs : EventArgs
    {
        private List<IPlugInAPI.strTrigActInfo> _triggersInPlugin;
        public TriggersInHomeSeerDataEventArgs(List<IPlugInAPI.strTrigActInfo> triggersInPlugin)
        {
            _triggersInPlugin = triggersInPlugin;
        }

        public List<IPlugInAPI.strTrigActInfo> TriggersInPlugin => _triggersInPlugin;
    }
    public class DeviceIdEventArgs : EventArgs
	{
		public int DeviceId { get; set; }
	}

}