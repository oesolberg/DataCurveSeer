using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.DataCurveComputation;
using HomeSeerAPI;

namespace DataCurveSeer.TriggerHandling.Triggers.DataCurveTriggerB
{
    public class DataCurveTriggerB : ITrigger
    {
        private bool _isCondition;
        private IPlugInAPI.strTrigActInfo _triggerInfo;
        private IHsCollectionFactory _collectionFactory;
        private ILogging _logging;
        private DataCurveTriggerBSettings _triggerSettings;
        private ReformatCopiedAction _reformatCopiedAction;
        private DataCurveTriggerBUi _dataCurveUi;
        private IHomeSeerHandler _homeSeerHandler;
        private IHSApplication _hs;
        private IDataCurveComputationHandlerB _dataCurveComputationHandler;

        public string GetTriggerName() => Utility.PluginName + ": A threshold value has been reached and the curve is ...";
        public int TriggerNumber { get; } = 2;
        public int UID => _triggerSettings?.UID ?? -1;
        public int EvRef => _triggerSettings?.EvRef ?? -1;
        public bool IsCondition => _isCondition;

        public int? DeviceId => _triggerSettings?.DeviceIdChosen;

        public DataCurveTriggerB(IHSApplication hs, ILogging logging, IHsCollectionFactory collectionFactory,
            IHomeSeerHandler homeSeerHandler, IReformatCopiedAction reformatCopiedAction = null,
            IDataCurveTriggerUi dataCurveUi = null,
            IDataCurveComputationHandlerB dataCurveComputationHandler = null)
        {
            _collectionFactory = collectionFactory;
            _logging = logging;
            _homeSeerHandler = homeSeerHandler;
            _hs = hs;
            _dataCurveComputationHandler = dataCurveComputationHandler;
            if (reformatCopiedAction == null)
            {
                _reformatCopiedAction = new ReformatCopiedAction(_logging);
            }

            if (dataCurveUi == null)
            {
                _dataCurveUi = new DataCurveTriggerBUi(_homeSeerHandler, _hs);
            }

            if (_dataCurveComputationHandler == null)
            {
                _dataCurveComputationHandler = new ComputationTriggerBHandler(_logging);
            }
        }

        public string GetSubTriggerName(int subTriggerNumber)
        {
            return "";
        }

        public bool GetHasConditions()
        {
            return true;
        }

        public int GetSubTriggerCount()
        {
            return 0;
        }

        public bool ContainsSubTriggerActionNumber(int subTriggerNumber)
        {
            if (subTriggerNumber == -1) return true;
            return false;
        }

        public void AddSettingsFromTrigActionInfo(IPlugInAPI.strTrigActInfo trigActInfo)
        {
            GetSettingsFromTriggerInfo(trigActInfo);
        }

        public IPlugInAPI.strTrigActInfo GetTriggerActionInfo()
        {
            return _triggerInfo; //_trigActInfo = trigActInfo;
        }

        public void SetCondition(IPlugInAPI.strTrigActInfo actionInfo, bool value)
        {
            _isCondition = value;
        }

        public bool GetCondition(IPlugInAPI.strTrigActInfo actionInfo)
        {
            return _isCondition;
        }



        public bool GetTriggerConfigured(IPlugInAPI.strTrigActInfo actionInfo)
        {
            _triggerSettings = GetSettingsFromTriggerInfo(actionInfo);
            if (_triggerSettings.DeviceIdChosen.HasValue && _triggerSettings.DeviceIdChosen.Value > -1)
            {
                OnDeviceIdSet(_triggerSettings.DeviceIdChosen.Value);
            }
            return _triggerSettings.GetTriggerConfigured();
        }

        private void OnDeviceIdSet(int deviceId)
        {
            var deviceIdEventArgs = new DeviceIdEventArgs() { DeviceId = deviceId };
            DeviceIdSet?.Invoke(this, deviceIdEventArgs);
        }

        public event DeviceIdSetEventHandler DeviceIdSet;

        public bool TriggerTrue(IPlugInAPI.strTrigActInfo actionInfo, IStorageHandler storageHandler)
        {
            _triggerSettings = GetSettingsFromTriggerInfo(actionInfo);
            if (_triggerSettings != null && _triggerSettings.GetTriggerConfigured())
            {
                var thresholdValue = _triggerSettings.ThresholdValue;
                var numberOfLastMeasurements = _triggerSettings.NumberOfLastMeasurements;
                var dataPoints = storageHandler.GetValuesForDevice(_triggerSettings.DeviceIdChosen.Value, SystemDateTime.Now().AddHours(-3),
                    SystemDateTime.Now());
                var lastValue = dataPoints.Last();
                if (!ThresholdReached(lastValue.Value, thresholdValue, _triggerSettings.AscendingOrDescending))
                    return false;


                _logging.LogDebug($"calling trigger for computation _dataCurveComputationHandler==null={_dataCurveComputationHandler == null}");

                return _dataCurveComputationHandler.TriggerTrue(dataPoints, _triggerSettings.AscendingOrDescending, thresholdValue.Value, numberOfLastMeasurements.Value);
            }
            return false;
        }

        private bool ThresholdReached(double lastValue, double? thresholdValue, AscDescEnum ascendingOrDescending)
        {
            if (!thresholdValue.HasValue) 
                return false;

            if (lastValue >= thresholdValue.Value && ascendingOrDescending == AscDescEnum.Ascending)
                return true;
            
            if (lastValue <= thresholdValue.Value && ascendingOrDescending == AscDescEnum.Descending)
                return true;

            return false;

        }

        public IPlugInAPI.strMultiReturn TriggerProcessPostUi(NameValueCollection postData,
            IPlugInAPI.strTrigActInfo trigActInfo)
        {
            var returnData = new IPlugInAPI.strMultiReturn();
            returnData.DataOut = trigActInfo.DataIn;
            returnData.TrigActInfo = trigActInfo;
            returnData.sResult = string.Empty;

            if (postData == null || postData.Count < 1) return returnData;

            object action = new Classes.action();
            var formattedAction = _collectionFactory.GetActionsIfPossible(trigActInfo);

            formattedAction = _reformatCopiedAction.Run(formattedAction, trigActInfo.UID, trigActInfo.evRef);

            postData.Add(Constants.TriggerTypeKey,
                this.ToString()); //this.GetType().Name);// typeof(this).Name;//this.ToString());

            postData.Add(Constants.IsConditionKey, _isCondition.ToString());

            //postData.Add(Constants.Uid, trigActInfo.UID.ToString());
            //postData.Add(Constants.EvRef, trigActInfo.evRef.ToString());

            foreach (string dataKey in postData)
            {
                if (string.IsNullOrEmpty(dataKey)) continue;

                var strippedKey = StripKeyOfUidStuff(dataKey);

                formattedAction.AddObject(strippedKey, (object)postData[dataKey]);
            }

            object objAction = (object)formattedAction;
            Utility.SerializeObject(ref objAction, ref returnData.DataOut);
            returnData.sResult = string.Empty;

            return returnData;
        }

        private string StripKeyOfUidStuff(string dataKey)
        {
            var indexOfUnderScore = dataKey.IndexOf('_', 2);
            if (indexOfUnderScore > 0)
            {
                var maxLength = GetLengthToUse(dataKey, indexOfUnderScore);
                dataKey = dataKey.Substring(0, maxLength);
            }

            return dataKey;
        }

        private int GetLengthToUse(string currentKey, int indexOfUnderScore)
        {
            var indexOfNextUnderscore = currentKey.IndexOf('_', indexOfUnderScore + 1);
            while (indexOfNextUnderscore > 0 && indexOfNextUnderscore == indexOfUnderScore + 1)
            {
                indexOfUnderScore = indexOfNextUnderscore;
                indexOfNextUnderscore = currentKey.IndexOf('_', indexOfUnderScore + 1);
            }

            return indexOfUnderScore + 1;
        }




        //Summary of format info on collapsed trigger/condition
        public string TriggerFormatUi(IPlugInAPI.strTrigActInfo actionInfo)
        {
            var infoInHeader = string.Empty;
            if (!_isCondition)
                return "This can never be a trigger, only a condition";

            _triggerSettings = GetSettingsFromTriggerInfo(actionInfo);
            var deviceInfo = GetDeviceInfoString();
            var ascDescCurve = GetAscendingDescendingCurveInfoString();
            var thresholdValue = "";
            if (_triggerSettings.ThresholdValue.HasValue)
                thresholdValue = _triggerSettings.ThresholdValue.Value.ToString();

            var numberOfLastMeasurements = "";
            if (_triggerSettings.NumberOfLastMeasurements.HasValue)
            {
                numberOfLastMeasurements = _triggerSettings.NumberOfLastMeasurements.Value.ToString();
            }
            return $" The threshold value of {thresholdValue} has been reached for the device <font class=\"event_Txt_Option\">{deviceInfo}</font> " +
                   $"and it has had {ascDescCurve} slope for the {numberOfLastMeasurements} last measurements";
        }

        private bool ChangeValueTrigger(int evRef)
        {
            return true;
            //return _homeSeerHandler.IsEventOfChangeValueType(evRef);
        }

        private string GetAscendingDescendingCurveInfoString()
        {
            if (_triggerSettings.AscendingOrDescending == AscDescEnum.Ascending)
                return "an ascending";
            return "a descending";
        }

        private string GetTimespanInfoString(TimeSpan? timeSpanToConvert, string prefix = "")
        {
            if (timeSpanToConvert.HasValue)
            {
                var hoursString = string.Empty;
                var minutesString = string.Empty;
                var andString = string.Empty;
                var timeSpanChosen = timeSpanToConvert.Value;
                if (timeSpanChosen.Hours > 0)
                {

                    hoursString = $"{timeSpanChosen.Hours.ToString()} hour";
                    if (timeSpanChosen.Hours > 1)
                    {
                        hoursString += "s";
                    }
                }

                if (timeSpanChosen.Hours > 0 && timeSpanChosen.Minutes > 0)
                {
                    andString = " and ";
                }

                if (timeSpanChosen.Minutes > 0)
                {
                    minutesString = $"{timeSpanChosen.Minutes} minute";
                    if (timeSpanChosen.Minutes > 1)
                    {
                        minutesString += "s";
                    }
                }
                return $"{prefix} {hoursString}{andString}{minutesString}";
            }

            return "an unknown time span (this should not happen)";

        }

        private string GetDeviceInfoString()
        {
            if (_triggerSettings.DeviceIdChosen.HasValue)
                return _homeSeerHandler.GetDeviceInfoString(_triggerSettings.DeviceIdChosen.Value);
            return "no device set";
        }

        //Trigger settings
        public string TriggerBuildUi(string uniqueControlId, IPlugInAPI.strTrigActInfo triggerInfo)
        {
            var uid = triggerInfo.UID.ToString();
            _triggerSettings = GetSettingsFromTriggerInfo(triggerInfo);

            if (!_isCondition)
                return "This can never be a trigger, only a condition";
            _triggerSettings.UidString = triggerInfo.UID.ToString();

            _triggerSettings.UniqueControllerId = uniqueControlId;
            return _dataCurveUi.Build(_triggerSettings);
        }

        private DataCurveTriggerBSettings GetSettingsFromTriggerInfo(IPlugInAPI.strTrigActInfo triggerInfo)
        {

            _triggerInfo = triggerInfo;
            var formattedAction = _collectionFactory.GetActionsIfPossible(triggerInfo);

            if (formattedAction != null) //&& formattedAction.Keys.Count > 0)
            {
                var uidAndEvRef = $"{triggerInfo.UID.ToString()}_{triggerInfo.evRef.ToString()}_";

                formattedAction = _reformatCopiedAction.Run(formattedAction, triggerInfo.UID, triggerInfo.evRef);
                var triggerSettings = new DataCurveTriggerBSettings();
                triggerSettings.UID = triggerInfo.UID;
                triggerSettings.EvRef = triggerInfo.evRef;
                foreach (var dataKey in formattedAction.Keys)
                {

                    if (dataKey.Contains(Constants.DeviceDropdownKey))
                    {
                        triggerSettings.DeviceIdChosen =
                            ParameterExtraction.GetIntOrNullFromObject(formattedAction[dataKey]);
                    }

                    if (dataKey.Contains(Constants.RoomKey))
                    {
                        triggerSettings.RoomChosen = (string)(formattedAction[dataKey]);
                    }

                    if (dataKey.Contains(Constants.FloorKey))
                    {
                        triggerSettings.FloorChosen = (string)(formattedAction[dataKey]);
                    }

                    if (dataKey.Contains(Constants.IsConditionKey))
                    {
                        triggerSettings.IsCondition = ParameterExtraction.GetBoolFromObject(formattedAction[dataKey]);
                    }


                    if (dataKey.Contains(Constants.AscDescKey))
                    {
                        triggerSettings.AscendingOrDescending = ParameterExtraction.GetAscDescEnumFromObject(formattedAction[dataKey]);
                    }

                    if (dataKey.Contains(Constants.ThresholdValueKey))
                    {
                        triggerSettings.ThresholdValue = ParameterExtraction.GetDoubleOrNull(formattedAction[dataKey]);
                    }

                    if (dataKey.Contains(Constants.NumberOfLastMeasurementsKey))
                    {
                        var numberOfLastMeasurements =
                            ParameterExtraction.GetIntOrNullFromObject(formattedAction[dataKey]);
                        if (numberOfLastMeasurements >= 2)
                        {
                            triggerSettings.NumberOfLastMeasurements = numberOfLastMeasurements;
                        }

                    }
                }

                _triggerSettings = triggerSettings;

                return triggerSettings;
            }

            return null;
        }

    }
}