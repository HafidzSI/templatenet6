using FluentAssertions;
using NetCa.Application.Common.Exceptions;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Exceptions
{
    public class BadRequestExceptionTests
    {
        [Test]
        [TestCase("Data Not Found")]
        public void DefaultConstructorCreatesAnExceptionWithMessage(string message)
        {
            var actual = new BadRequestException(message);

            actual.Message.Should().BeEquivalentTo(message);
        }
    }
}
