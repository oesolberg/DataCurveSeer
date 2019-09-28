using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;
using DataCurveSeer.Common.Repository;
using DataCurveSeer.DataCurveComputation;
using NUnit.Framework;
using Should;

namespace UnitTests
{
    [TestFixture]
    public class ComputationTriggerBHandlerTests
    {
        private ILogging _logging;

        [SetUp]
        public void SetUp()
        {
            _logging = NSubstitute.Substitute.For<ILogging>();
        }

        [Test]
        public void TriggerTrue_AllNullableDataNull_ShouldReturnFalse()
        {
            var sut=new ComputationTriggerBHandler(_logging);

            var result = sut.TriggerTrue(null, AscDescEnum.Descending, 0, 0);

            result.ShouldBeFalse();
        }

        [Test]
        public void TriggerTrue_AllNullableDataInitializedButNoData_ShouldReturnFalse()
        {
            var sut = new ComputationTriggerBHandler(_logging);

            var result = sut.TriggerTrue(new List<DeviceValue>(), AscDescEnum.Descending, 0, 0);

            result.ShouldBeFalse();
        }

        [Test]
        public void TriggerTrue_OnlyOneDataPoint_ShouldReturnFalse()
        {
            var sut = new ComputationTriggerBHandler(_logging);
            var dataPoints = new List<DeviceValue>() {new DeviceValue()};

            var result = sut.TriggerTrue(dataPoints, AscDescEnum.Descending, 0, 0);

            result.ShouldBeFalse();
        }
    }
}
