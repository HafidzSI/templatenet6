using FluentAssertions;
using NetCa.Application.Common.Exceptions;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Exceptions
{
    public class ThrowExceptionTests
    {
        [Test]
        [TestCase("Data Not Found")]
        public void DefaultConstructorCreatesAnExceptionWithMessage(string message)
        {
            var actual = new ThrowException(message);

            actual.Message.Should().BeEquivalentTo(message);
        }
    }
}
