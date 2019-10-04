using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.TriggerHandling;
using HomeSeerAPI;
using NSubstitute;
using NUnit.Framework;
using Should;

namespace UnitTests
{
    [TestFixture]
    public class GetTriggersFromHomeSeerHandlerTests
    {
        private IIniSettings _iniSettings;
        private ILogging _logging;
        private IAppCallbackAPI _callBack;

        [SetUp]
        public void SetUp()
        {
            _iniSettings = NSubstitute.Substitute.For<IIniSettings>();
            _logging = NSubstitute.Substitute.For<ILogging>();
            _callBack = NSubstitute.Substitute.For<IAppCallbackAPI>();
        }

        [Test]
        public void StartWork_NoTriggersFound_ShouldNotTriggerButDieWithinSetTime()
        {
            _callBack.GetTriggers(Arg.Any<string>()).Returns(l=>null);
            var sut = new GetTriggersFromHomeSeerHandler(_iniSettings, _logging, _callBack, 1);
            
            sut.StartWork();

            Thread.Sleep(1600);
            _callBack.Received(1).GetTriggers(Arg.Any<string>());
            sut.IsRunning.ShouldBeFalse();
        }

        [Test]
        public void StartWork_OneTriggerFound_ShouldReturnTriggerAndDieWithinSetTime()
        {
            var triggerArray = new IPlugInAPI.strTrigActInfo[1]{new IPlugInAPI.strTrigActInfo()};
            _callBack.GetTriggers(Arg.Any<string>()).Returns(triggerArray);
            var sut = new GetTriggersFromHomeSeerHandler(_iniSettings, _logging, _callBack, 1);
            List<IPlugInAPI.strTrigActInfo> expectedData=null;
            sut.TriggerDataReady+= (sender, arg) =>
            {
                expectedData = arg.TriggersInPlugin;
            };

            sut.StartWork();

            Thread.Sleep(1600);
            _callBack.Received(1).GetTriggers(Arg.Any<string>());
            sut.IsRunning.ShouldBeFalse();
            expectedData.ShouldNotBeEmpty();
            expectedData.Count.ShouldEqual(1);
        }

        [Test]
        public void StartWork_TwoTriggersFound_ShouldReturnTriggerAndDieWithinSetTime()
        {
            var triggerArray = new IPlugInAPI.strTrigActInfo[2] { new IPlugInAPI.strTrigActInfo() , new IPlugInAPI.strTrigActInfo() };
            _callBack.GetTriggers(Arg.Any<string>()).Returns(triggerArray);
            var sut = new GetTriggersFromHomeSeerHandler(_iniSettings, _logging, _callBack, 1);
            List<IPlugInAPI.strTrigActInfo> expectedData = null;
            sut.TriggerDataReady += (sender, arg) =>
            {
                expectedData = arg.TriggersInPlugin;
            };

            sut.StartWork();

            Thread.Sleep(1600);
            _callBack.Received(1).GetTriggers(Arg.Any<string>());
            sut.IsRunning.ShouldBeFalse();
            expectedData.ShouldNotBeEmpty();
            expectedData.Count.ShouldEqual(2);
        }

        [Test]
        public void Delay_TwoTriggersFound_ShouldReturnTriggerAfterDelayAndDieWithinSetTime()
        {
            var triggerArray = new IPlugInAPI.strTrigActInfo[2] { new IPlugInAPI.strTrigActInfo(), new IPlugInAPI.strTrigActInfo() };
            _callBack.GetTriggers(Arg.Any<string>()).Returns(triggerArray);
            var sut = new GetTriggersFromHomeSeerHandler(_iniSettings, _logging, _callBack, 1);
            List<IPlugInAPI.strTrigActInfo> expectedData = null;
            sut.TriggerDataReady += (sender, arg) =>
            {
                expectedData = arg.TriggersInPlugin;
            };

            sut.StartWork();
            Thread.Sleep(600);
            _callBack.Received(0).GetTriggers(Arg.Any<string>());
            sut.DelayFetching(2);
            sut.IsRunning.ShouldBeTrue();
            Thread.Sleep(1600);
            _callBack.Received(0).GetTriggers(Arg.Any<string>());
            sut.IsRunning.ShouldBeTrue();
            Thread.Sleep(600);

            _callBack.Received(1).GetTriggers(Arg.Any<string>());
            sut.IsRunning.ShouldBeFalse();
            expectedData.ShouldNotBeEmpty();
            expectedData.Count.ShouldEqual(2);
        }
    }
}